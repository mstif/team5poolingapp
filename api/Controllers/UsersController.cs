using api.Helpers;
using api.Models;
using ApplicationUsers;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using Services.Abstractions;
using Services.Contracts;
using Services.Implementation;
using Services.Implementation.Exceptions;
using System.Security.Claims;

namespace SecureWebSite.Server.Controllers
{
    [Route("api/securewebsite")]
    [ApiController]
    public class SecureWebsiteController(SignInManager<ApplicationUser> sm,
        IMapper mapper,
        UserManager<ApplicationUser> um,
        RoleManager<IdentityRole> roleManager,
        IContragentService contragentService) : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> signInManager = sm;
        private readonly UserManager<ApplicationUser> userManager = um;
        private readonly IMapper _mapper = mapper;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IContragentService _contragentService=contragentService;
        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser(ApplicationUser user)
        {

            IdentityResult result = new();

            try
            {
                ApplicationUser user_ = new ApplicationUser()
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    UserName = user.UserName,
                };

                result = await userManager.CreateAsync(user_, user.PasswordHash);

                if (!result.Succeeded)
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong, please try again. " + ex.Message);
            }

            return Ok(new { message = "Registered Successfully.", result = result });
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginUser(Login login)
        {

            try
            {
                ApplicationUser user_ = await userManager.FindByEmailAsync(login.Email);
                if (user_ != null)
                {
                    login.Username = user_.UserName;

                    if (!user_.EmailConfirmed)
                    {
                        user_.EmailConfirmed = true;
                    }

                    var result = await signInManager.PasswordSignInAsync(user_, login.Password, login.Remember, false);

                    if (!result.Succeeded)
                    {
                        return Unauthorized(new { message = "Check your login credentials and try again" });
                    }
                    var isAdmin = await userManager.IsInRoleAsync(user_, "Administrator");
                    //user_.LastLogin = DateTime.Now;
                    var updateResult = await userManager.UpdateAsync(user_);
                }
                else
                {
                    return BadRequest(new { message = "Please check your credentials and try again. " });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Something went wrong, please try again. " + ex.Message });
            }

            return Ok(new { message = "Login Successful." });
        }

        [HttpGet("logout"), Authorize]
        public async Task<ActionResult> LogoutUser()
        {

            try
            {
                await signInManager.SignOutAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Someting went wrong, please try again. " + ex.Message });
            }

            return Ok(new { message = "You are free to go!" });
        }

        [HttpGet("admin"), Authorize]
        public ActionResult AdminPage()
        {
            string[] partners = { "Raja", "Bill Gates", "Elon Musk", "Taylor Swift", "Jeff Bezoss",
                                        "Mark Zuckerberg", "Joe Biden", "Putin"};

            return Ok(new { trustedPartners = partners });
        }

        [HttpGet("userlist")]
        public async Task<IActionResult> UserList()
        {
            var users = await userManager.Users.ToListAsync();
            var model=users.Select( (f)=>   setRole(f).Result ).ToList();
            return Ok(model);
        }

        [HttpGet("{id}"), Authorize]
        public async Task<IActionResult> CreateEditUser(string? id)
        {
            if (id == null) return NotFound();

            ApplicationUser? user;
            if (id == "new")
            {
                user = new ApplicationUser();

            }
            else
            {
                user = await userManager.FindByIdAsync(id);
            }
            if(user == null) return NotFound();
            string[] majorRoles = [UserRoles.Administrator, UserRoles.Customer, UserRoles.Logist];
            var roles = (await userManager.GetRolesAsync(user)).ToList();
            var allRoles = await _roleManager.Roles.ToListAsync();
            var adminRole = roles.Contains(majorRoles[0]);
            var customerRole = roles.Contains(majorRoles[1]);
            var logistRole = roles.Contains(majorRoles[2]);
            var currentuser = await userManager.GetUserAsync(User);
            if (!(await userManager.IsInRoleAsync(currentuser, "Administrator")) && id != currentuser?.Id) { throw new AccessDeniedException("Profile user"); }
            //список всех доп ролей, с пометкой есть ли эта роль у юзера
            var RolesUser = allRoles.Where(role => !majorRoles.Contains(role.Name)).Select(r => new IsInRole
            {
                InRole = roles.Contains(r.Name!),
                Role = r.Name!
            }).ToList();


            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            var settings = SettingsUser.GetFromJson(user.SettingsUser);
   
            var companyId = settings?.CompanyId;
            var company = await contragentService.GetBookByIdAsync(companyId);
            return Ok(new UsersModel
            {
                UserName = user.UserName!,
                Email = user.Email!,
                FullName = user.FullName,
                Company = company?.Name,
                CompanyId = companyId,
                SecondRoles = RolesUser,
                MajorRole = adminRole ? "Administrator" : (logistRole ? "Logist" : (customerRole ? "Customer" : "")),
                Id = id,
                Approved = user.Approved

            });



        }


        [HttpPost("save-user-full")]
        public async Task<ActionResult> SaveProfileAdmin([FromBody] UsersModel input)
        {
            string StatusMessage = "";
            //if (!ModelState.IsValid)
            //{
            //    StatusMessage = "Профиль не сохранен";
            //    foreach (var error in ModelState)
            //    {
            //        StatusMessage += '\n' + error.Value.ToString();
            //    }
            //    if (input.Id == "new") return Content("Error model");

            //    return RedirectToAction(nameof(CreateEditUser), "Admin", new { id = input.Id });
            //}
            var user = new ApplicationUser();
            if (input.Id != "new")
            {
                user = await userManager.FindByIdAsync(input.Id);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
                }
                if (input.Password != null && input.Password.Trim() != string.Empty && input.Password == input.ConfirmPassword)
                {
                    var _passwordValidator = HttpContext.RequestServices.GetService(typeof(IPasswordValidator<ApplicationUser>)) as IPasswordValidator<ApplicationUser>;
                    var _passwordHasher =
                        HttpContext.RequestServices.GetService(typeof(IPasswordHasher<ApplicationUser>)) as IPasswordHasher<ApplicationUser>;

                    IdentityResult result = await _passwordValidator?.ValidateAsync(userManager, user, input.Password);
                    if (result.Succeeded)
                    {
                        user.PasswordHash = _passwordHasher?.HashPassword(user, input.Password);
                        await userManager.UpdateAsync(user);

                    }
                    else
                    {
                        StatusMessage = "Пароль не изменен (треб. строчные, прописные, цифры,символы, длина 6)";// + result.Errors.Aggregate(e=>e.Description;
                    }
                    //await _userManager.RemovePasswordAsync(user);
                    //await _userManager.AddPasswordAsync(user, input.Password);
                }
                else
                {
                    StatusMessage = "Пароль не изменен (треб. строчные, прописные, цифры, символы, длина 6) ";
                }
                var email = await userManager.GetEmailAsync(user);
                if (input.Email != email)
                {
                    var setEmailResult = await userManager.SetEmailAsync(user, input.Email);
                    if (!setEmailResult.Succeeded)
                    {
                        foreach (var error in setEmailResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            else
            {
                user.UserName = input.UserName;
                user.Email = input.Email;
                user.EmailConfirmed = true;
                if (input.Password != null && input.ConfirmPassword != null && input.Password == input.ConfirmPassword) { }
                await userManager.AddPasswordAsync(user, input.Password);
            }





            // Model state might not be valid anymore if we weren't able to change the e-mail address
            // so we need to check for that before proceeding
            if (ModelState.IsValid)
            {
                if (input.FullName != user.FullName)
                {
                    // If we receive an empty string, set a null full name instead
                    user.FullName = string.IsNullOrWhiteSpace(input.FullName) ? null : input.FullName;

                }
                user.Approved = input.Approved;
                var settings = SettingsUser.GetFromJson(user.SettingsUser);
                if (settings == null)
                {
                    settings = new SettingsUser();
                }

                

                if (input?.CompanyId != null || input?.CompanyId != 0)
                {
                    settings.CompanyId = input?.CompanyId;
                }
                user.SettingsUser = settings.ConvertToJson();




                List<KeyValuePair<string, bool>> mRoles = new List<KeyValuePair<string, bool>>()
                {
                    new KeyValuePair<string, bool>(UserRoles.Administrator, input.MajorRole=="Administrator"),
                    new KeyValuePair<string, bool>(UserRoles.Logist, input.MajorRole=="Logist"),
                    new KeyValuePair<string, bool>(UserRoles.Customer, input.MajorRole=="Customer")
                };

                foreach (var mRole in mRoles)
                {
                    bool UserHasRole = await userManager.IsInRoleAsync(user, mRole.Key);
                    if (mRole.Value)
                    {
                        if (!UserHasRole)
                        {
                            await userManager.AddToRoleAsync(user, mRole.Key);
                        }

                    }
                    else
                    {
                        if (UserHasRole)
                        {
                            if(await userManager.IsInRoleAsync(user, "Administrator"))
                            {
                                var adminUsers = await userManager.GetUsersInRoleAsync("Administrator");
                                if (adminUsers.Count() == 1)
                                {
                                    StatusMessage += " После этой операции не  останется ни одного администратора. Отменено";
                                    input.StatusMessage= StatusMessage;
                                    return BadRequest(input);
                                }
                            }
                            await userManager.RemoveFromRoleAsync(user, mRole.Key);
                        }

                    }
                }




                foreach (var dopRole in input.SecondRoles)
                {
                    bool UserHasRole = await userManager.IsInRoleAsync(user, dopRole.Role);
                    if (dopRole.InRole)
                    {
                        if (!UserHasRole)
                        {
                            await userManager.AddToRoleAsync(user, dopRole.Role);
                        }

                    }
                    else
                    {
                        if (UserHasRole)
                        {
                            await userManager.RemoveFromRoleAsync(user, dopRole.Role);
                        }

                    }
                }

                await userManager.UpdateAsync(user);



                StatusMessage = StatusMessage + " Профиль сохранен";
            }
            else
            {
                StatusMessage = "Профиль не сохранен";
                foreach (var error in ModelState)
                {
                    StatusMessage += '\n' + error.Value.ToString();
                }
            }
            var model = await setRole(user);
            model.StatusMessage = StatusMessage;
            return Ok(model);
 

        }




        [HttpGet("home/{email}"), Authorize]
        public async Task<ActionResult> HomePage(string email)
        {
            ApplicationUser userInfo = await userManager.FindByEmailAsync(email);
            if (userInfo == null)
            {
                return BadRequest(new { message = "Something went wrong, please try again." });
            }

            return Ok(new { userInfo = userInfo });
        }

        [HttpGet("xhtlekd")]
        public async Task<ActionResult> CheckUser()
        {
            ApplicationUser currentuser = new();
            var roles = new List<string>();
            try
            {
                var user_ = HttpContext.User;
                var principals = new ClaimsPrincipal(user_);
                var result = signInManager.IsSignedIn(principals);
               
                if (result)
                {
                    currentuser = await signInManager.UserManager.GetUserAsync(principals);
                    roles = (await userManager.GetRolesAsync(currentuser)).ToList();
                }
                else
                {
                    return Forbid();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Something went wrong please try again. " + ex.Message });
            }

            return Ok(new { message = "Logged in", user = new { user = currentuser, roles = roles } });
        }

        private async Task<UsersModel> setRole(ApplicationUser user)
        {
            string[] majorRoles = [UserRoles.Administrator, UserRoles.Customer, UserRoles.Logist];
            var roles = (await userManager.GetRolesAsync(user)).ToList();
            var allRoles = await _roleManager.Roles.ToListAsync();
            var adminRole = roles.Contains(majorRoles[0]);
            var customerRole = roles.Contains(majorRoles[1]);
            var logistRole = roles.Contains(majorRoles[2]);
            //список всех доп ролей, с пометкой есть ли эта роль у юзера
            var RolesUser = allRoles.Where(role => !majorRoles.Contains(role.Name)).Select(r => new IsInRole
            {
                InRole = roles.Contains(r.Name!),
                Role = r.Name!
            }).ToList();

            var model = DataHelper.Map<ApplicationUser, UsersModel>(user);
            model.SecondRoles = RolesUser;
            model.MajorRole = adminRole ? "Administrator" : (logistRole ? "Logist" : (customerRole ? "Customer" : ""));
            var userInfo = new UserInfo(user, userManager);
            var companyId = userInfo?.Settings?.CompanyId;
            if (companyId != null)
            {
                model.CompanyId = companyId;
                model.Company= (await _contragentService.GetBookByIdAsync(companyId))?.Name;
            }
            return model;

        }

    }
}
