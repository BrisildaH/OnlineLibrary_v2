using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using OnlineLibrary.DataLayer.DBContext;
using OnlineLibrary.DataLayer.Entiteties;
using OnlineLibrary.Models;
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
        public AccountController(OnlineLibraryDbContext onlineLibraryDbContext,
           UserManager<User> userManager,
           RoleManager<IdentityRole> roleManager,
           IHttpContextAccessor httpContextAccessor,
           SignInManager<User> signManager)
        {
            _onlineLibraryDbContext = onlineLibraryDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _signManager = signManager;
        }
        public IActionResult Index([FromQuery] string filterTerm)
        {
            var users = _onlineLibraryDbContext.Users
                 //.Include(p => p.)
                 .Where(p => (p.IsDelete == false || p.IsDelete == null))
                 .OrderBy(p => p.FullName)
                 .ToList();

            if (!string.IsNullOrEmpty(filterTerm))
            {
                users = users.Where(p => p.FullName.Contains(filterTerm))
                                  .ToList();

            }
            return View(users);
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
                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("12356fsy74rhdh"));//Secret key
                    var token = new JwtSecurityToken(
                        issuer: "https://localhost:44329/", //Your App URL
                        audience: "https://localhost:44329/", //Your App URL
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
                            LoginProvider = "Simple Web Application Log in",
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
        public IActionResult Details(int id)
        {
            var users = _onlineLibraryDbContext.Users
                                      .Where(p => p.Id == id.ToString())
                                      .FirstOrDefault();
            return View(users);
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
    }
}

