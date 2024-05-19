using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using OnlineLibrary.DataLayer.DBContext;
using OnlineLibrary.DataLayer.Entiteties;
using OnlineLibrary.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineLibrary.Controllers
{
    public class AccountController : Controller
    {
        private readonly OnlineLibraryDbContext _onlineLibraryDbContext;
        //Identity
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        public AccountController(OnlineLibraryDbContext onlineLibraryDbContext,
           UserManager<User> userManager,
           RoleManager<IdentityRole> roleManager,
           IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
           SignInManager<User> signManager)
           
        {
            _onlineLibraryDbContext = onlineLibraryDbContext;


            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _signManager = signManager;
            _config = configuration;
        }
		public async Task<IActionResult> Index(string filterTerm)
		{
			var users = _onlineLibraryDbContext.Users
		.Where(p => (p.IsDelete == false || p.IsDelete == null))
		.OrderBy(p => p.FullName)
		.ToList();

			var userModels = new List<UserModel>();
			foreach (var user in users)
			{
				var userModel = new UserModel()
				{
					Id = user.Id,
                    FullName = user.FullName,
					Email = user.Email,
					IsActive = user.IsActive,
					IsAdmin = user.IsAdmin,
					PhoneNumber = user.PhoneNumber,
					UserName = user.UserName,
					CreatedDate = user.CreatedDate,
                    Roles = new List<string>()
				};

				var roles = await _userManager.GetRolesAsync(user);
				foreach (var role in roles)
				{
					userModel.Roles.Add(role);
				}

				userModels.Add(userModel);
			}

			if (!string.IsNullOrEmpty(filterTerm))
			{
				userModels = userModels.Where(p => p.FullName.Contains(filterTerm)|| p.Email.Contains(filterTerm))
								  .ToList();
			}
			return View(userModels);
		}
		public IActionResult Register()
        {
            return View();
        }
        [HttpPost, ActionName("Register")]
        public async Task<IActionResult> Register([Bind("FirstName", "LastName", "Email", "Password", "Phone")] AccountRegisterModel accountRegisterModel)
        {
            try
            {
                var user = new User()
                {
                    FullName = accountRegisterModel.FirstName + " " + accountRegisterModel.LastName,
                    Email = accountRegisterModel.Email,
                    NormalizedEmail = accountRegisterModel.Email,
                    EmailConfirmed = true,
                    PhoneNumber = accountRegisterModel.Phone,
                    UserName = accountRegisterModel.FirstName[0] + accountRegisterModel.LastName,
                };

                //Check if user exists wiht same email.
                if (await _userManager.FindByEmailAsync(accountRegisterModel.Email) != null)
                {
                    throw new Exception("User with exists with this email");
                }

                //Create user async
                var result = await _userManager.CreateAsync(user, accountRegisterModel.Password);
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.ToString());
                }

                var viewrRoleID = _onlineLibraryDbContext.Roles.Where(p => p.Name == "Viewr")
                    .FirstOrDefault().Id;
                _onlineLibraryDbContext.UserRoles.Add(new IdentityUserRole<string>()
                {
                    RoleId = viewrRoleID,
                    UserId = user.Id
                });

                var userProfileModel = new UserProfileModel()
                {
                    FullName = user.FullName,
                    Email = user.FullName,
                    IsAdmin = user.IsAdmin.HasValue && user.IsAdmin.Value == true ? "Yes" : "No",
                    Phone = user.PhoneNumber,
                    PhotoPath = user.PhotoPath
                };
                return View("MyProfile", userProfileModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error in registering");
            }

        }
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost, ActionName("LogIn")]
        public async Task<IActionResult> Login([Bind("Email", "Password")] LogInModel logInModel)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(logInModel.Email);
                if (user != null && await _userManager.CheckPasswordAsync(user, logInModel.Password))
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }
                    authClaims.Add(new Claim(ClaimTypes.PrimarySid, user.Id));

                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["SecretKey"]));//Secret key

                    var token = new JwtSecurityToken(
                        issuer: _config["Issuer"], //Your App URL
                        audience: _config["Audience"], //Your App URL
                        expires: DateTime.Now.AddDays(3),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                        );

                    authClaims.Add(new Claim("token", token.Payload.ToString()));

                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(authClaims, JwtBearerDefaults.AuthenticationScheme);
                    AuthenticationProperties properties = new AuthenticationProperties()
                    {
                        AllowRefresh = true,
                        IsPersistent = true
                    };
                    //Sign in on the session
                    await _signManager.SignInAsync(user, true);

                    //check if user is authenticated
                    if (_onlineLibraryDbContext.UserLogins.Where(p => p.UserId == user.Id).FirstOrDefault() == null)
                    {
                        await _onlineLibraryDbContext.UserLogins.AddAsync(new IdentityUserLogin<string>()
                        {
                            LoginProvider = "Simple Web Application Log in" + user.Id,
                            ProviderDisplayName = "Application Log in",
                            UserId = user.Id,
                            ProviderKey = "User Password",
                        });
                        await _onlineLibraryDbContext.SaveChangesAsync();
                    }


                    var userProfileModel = new UserProfileModel()
                    {
                        FullName = user.FullName,
                        Email = user.FullName,
                        IsAdmin = user.IsAdmin.HasValue && user.IsAdmin.Value == true ? "Yes" : "No",
                        Phone = user.PhoneNumber,
                        PhotoPath = user.PhotoPath
                    };
                    return View("MyProfile", userProfileModel);
                }
                else
                {
                    throw new Exception("Error in logging in. Check your credentials");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"{ex.Message}";
                return View();
            }
        }
        public async Task<IActionResult> MyProfile()
        {
            try
            {
                var sessionIdentity = User.Identity;
                //check if user is authenticated
                if (sessionIdentity.IsAuthenticated == false)
                {
                    return RedirectToAction("Login");
                }
                var userID = User.Claims.Where(p => p.Type == ClaimTypes.Email).FirstOrDefault().Value;
                var user = await _userManager.FindByEmailAsync(userID);
                if (user == null)
                {
                    throw new Exception("User not found");
                }
                var UserProfileModel = new UserProfileModel()
                {
                    FullName = user.FullName,
                    Email = user.FullName,
                    IsAdmin = user.IsAdmin.HasValue && user.IsAdmin.Value == true ? "Yes" : "No",
                    Phone = user.PhoneNumber,
                    PhotoPath = user.PhotoPath
                };
                return View(UserProfileModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error in getting profile");
            }
        }
        public async Task<IActionResult> Details(string id)
        {
			var user = _onlineLibraryDbContext.Users
									  .Where(p => p.Id == id.ToString())
                                      .FirstOrDefault();

			var userModel = new UserModel()
			{
				Id = user.Id,
				FullName = user.FullName,
				Email = user.Email,
				IsActive = user.IsActive,
				IsAdmin = user.IsAdmin,
				PhoneNumber = user.PhoneNumber,
				UserName = user.UserName,
				CreatedDate = user.CreatedDate,
				Roles = new List<string>(),
                PhotoPath= user.PhotoPath
			};

			var roles = await _userManager.GetRolesAsync(user);
			foreach (var role in roles)
			{
				userModel.Roles.Add(role);
			}
			
			return View(userModel);
        }

        [HttpPost, ActionName("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    await _signManager.SignOutAsync();
                }
                return View("LogIn");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error in logging out");
            }
        }
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateUserRole(string id)
		{
			try
			{
				var user = await _userManager.FindByIdAsync(id);
				if (user == null)
				{
					throw new Exception("User not found");
				}
				var userRoleDisplayModel = new UserRoleDisplayModel()
				{
					FullName = user.FullName,
					Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault(),
                    Roles = _onlineLibraryDbContext.Roles.ToList()
				};
				return View(userRoleDisplayModel);
			}
			catch (Exception ex)
			{
				ViewBag.ErrorMessage = $"{ex.Message}";
				return View();
			}
		}
		[Authorize(Roles = "Admin")]
		[HttpPost, ActionName("UpdateUserRole")]
		public async Task<IActionResult> UpdateUserRole([Bind("", "")] UserRoleDisplayModel userRolUpdateModel)
		{
			try
			{
				var user = _onlineLibraryDbContext.Users.Where(p => p.FullName.Contains(userRolUpdateModel.FullName))
					.FirstOrDefault();
				if (user == null)
				{
					throw new Exception("User not found");
				}
				var role = _onlineLibraryDbContext.Roles.Where(p => p.Id == userRolUpdateModel.Role)
					.FirstOrDefault();

				if (role == null)
				{
					throw new Exception("Role not found");
				}

				var userRole = _onlineLibraryDbContext.UserRoles.Where(p => p.UserId == user.Id).FirstOrDefault();
				_onlineLibraryDbContext.UserRoles.Remove(userRole);
				_onlineLibraryDbContext.UserRoles.Add(new IdentityUserRole<string>()
				{
					RoleId = role.Id,
					UserId = user.Id
				});
				_onlineLibraryDbContext.SaveChanges();

				var userRoleDisplayModel = new UserRoleDisplayModel()
				{
					FullName = user.FullName,
					Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault(),
					Roles = _onlineLibraryDbContext.Roles.ToList()
				};
				return View("UpdateUserRole", userRoleDisplayModel);
			}
			catch (Exception ex)
			{
				ViewBag.ErrorMessage = $"{ex.Message}";
				return View();
			}
		}
	}
}

