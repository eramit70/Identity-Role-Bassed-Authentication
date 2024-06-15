using Microsoft.AspNetCore.Identity;
using RoleBassedAuthentication.DTO;
using RoleBassedAuthentication.interfaces;
using RoleBassedAuthentication.Models;
using System.Security.Claims;

namespace RoleBassedAuthentication.services
{
    public class AuthenticationUser : IAuthencticationUser
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthenticationUser(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;

        }
        public async Task<Status> ChangePasswordAsync(ChangePassword model, string username)
        {
           Status status = new Status();
            var user = await _userManager.FindByNameAsync(username);
            if(user == null)
            {
                status.StatusCode = 0;
                status.Message = "User Does Not Exists";
            }
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                status.StatusCode = 1;
                status.Message = "Password Has Updated Successfuly";
                
            }
            else
            {
                status.StatusCode =0;
                status.Message = "Some Error Occured";
            }
            return status;
        }

        public async Task<Status> LoginAsync(LoginUser model)
        {
            Status status = new Status();
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                status.StatusCode = 0;
                status.Message = "UserName is Invalid";
                return status;
            }
            if (!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                status.StatusCode = 0;
                status.Message = "Password is Invalid";
                return status;
            }
            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, false, true);

            if (signInResult.Succeeded)
            {

                var userRoles = await _userManager.GetRolesAsync(user);
                var authClains = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                };
                foreach (var userRole in userRoles)
                {
                    authClains.Add(new Claim(ClaimTypes.Role, userRole));
                }

                status.StatusCode = 1;
                status.Message = "User Login Successfully";

            }
            else if (signInResult.IsLockedOut)
            {
                status.StatusCode = 0;
                status.Message = "User is Locked out";
            }
            else
            {
                status.StatusCode = 0;
                status.Message = "Error on Logging in";
            }
            return status;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<Status> RegisterAsync(RegisterUser model)
        {
            Status status = new Status();

            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                status.StatusCode = 0;
                status.Message = "User Aleady Have Account";
                return status;
            }
            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true

            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "User Account Creation Failed";
                return status;
            }
            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole(model.Role));
            }
            if (await _roleManager.RoleExistsAsync(model.Role))
            {
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            status.StatusCode = 1;
            status.Message = "You Have Created Account SuccessFully";
            return status;
        }
    }
}
