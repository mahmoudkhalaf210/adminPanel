using AutoMapper;
using CourseWebsite.Constants;
using CourseWebsite.Data;
using CourseWebsite.DTOs.Account;
using CourseWebsite.Models;
using HospAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CourseWebsite.Repositories.AccountRepository
{
    public class AccountRepoistory : IAccountRepoistory
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IPasswordHasher<ApplicationUser> passwordHasher;

        private readonly IMapper mapper;
        private readonly IConfiguration config;

        public AccountRepoistory(UserManager<ApplicationUser> _userManager, IPasswordHasher<ApplicationUser> _passwordHasher, RoleManager<IdentityRole> _roleManager, IMapper _mapper, IConfiguration _configration)
        {
            userManager = _userManager;
            roleManager = _roleManager;
            mapper = _mapper;
            config = _configration;
            passwordHasher = _passwordHasher;
        }

        public async Task<string> UpdateUser(UpdateUserDTO userDTO)
        {
            var user = await userManager.FindByIdAsync(userDTO.userId);

            if (user == null)
            {
                return "User not found";
            }


            if (!string.IsNullOrEmpty(userDTO.NewPassword))
            {
                var newPasswordHash = passwordHasher.HashPassword(user, userDTO.NewPassword);
                user.PasswordHash = newPasswordHash;
            }

            if (await roleManager.RoleExistsAsync(userDTO.Role))
            {
                var currentRoles = await userManager.GetRolesAsync(user);
                await userManager.RemoveFromRolesAsync(user, currentRoles);

                // Add the new role
                await userManager.AddToRoleAsync(user, userDTO.Role);
            }
            else
            {
                return "This Role Doesn't Exist";
            }

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return "User updated successfully";
            }
            else
            {
                return "User failed to update: " + result.Errors.FirstOrDefault().Description;
            }
        }


        public async Task<string> DeleteUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return "User not found";
            }

            IdentityResult result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return "User deleted successfully";
            }
            else
            {
                return "User failed to delete: " + result.Errors.FirstOrDefault().Description;
            }
        }

        public async Task<ResponseService<List<GetUserDTO>>> GetAllUsers()
        {
            var response = new ResponseService<List<GetUserDTO>>();
            try
            {
                var userWithRole = new List<GetUserDTO>();

                // Fetch all users and roles in a single call
                var users = await userManager.Users.ToListAsync();
                var rolesDictionary = new Dictionary<string, List<string>>();

                // Collect roles for all users in one go
                foreach (var user in users)
                {
                    var roles = await userManager.GetRolesAsync(user);
                    rolesDictionary[user.Id] = roles.ToList();
                }

                // Construct the DTOs in memory
                foreach (var user in users.Where(e => e.UserName != "Marwan"))
                {
                    if (rolesDictionary.TryGetValue(user.Id, out var roles))
                    {
                        foreach (var role in roles)
                        {
                            userWithRole.Add(new GetUserDTO
                            {
                                Id = user.Id,
                                UserName = user.UserName,
                                Email = user.Email,
                                Role = role
                            });
                        }
                    }
                }

                response.data = userWithRole;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = "An error occurred while fetching the users. ex: " + ex.Message;
            }

            return response;
        }

        /* public async Task<ResponseService<List<GetUserDTO>>> GetAllUsers()
         {
             var response = new ResponseService<List<GetUserDTO>>();
             try
             {
                 var userWithRole = new List<GetUserDTO>();

                 var users = await userManager.Users.ToListAsync();
                 var roles = roleManager.Roles.ToList();
                 foreach (var item in userManager.Users.Where(e => e.UserName != "Marwan"))
                 {
                     foreach (var role in await userManager.GetRolesAsync(item))
                     {
                         userWithRole.Add(new GetUserDTO
                         {
                             Id = item.Id,
                             UserName = item.UserName,
                             Email = item.Email,
                             Role = role  
                         });
                     }
                 }

                 response.data = userWithRole;
             }
             catch (Exception ex)
             {
                 response.success = false;
                 response.message = "An error occurred while fetching the users. ex: " + ex.Message;

             }

             return response;
         }
 */
        public async Task<ResponseService<List<string>>> GetAllRoles()
        {
            var response = new ResponseService<List<string>>();

            try
            {
                var roles = await roleManager.Roles.Select(r => r.Name).ToListAsync();
                response.data = roles;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = "An error occurred while fetching roles. Error: " + ex.Message;
            }

            return response;
        }

        public async Task<ResponseService<List<Role>>> GetAllRolesWithId()
        {
            var response = new ResponseService<List<Role>>();

            try
            {
                var roles = await roleManager.Roles.Where(ro => ro.Name != "SuperAdmin").Select(r =>
                new Role
                {
                    Roleid = r.Id,
                    RoleName = r.Name
                }
                ).ToListAsync();
                response.data = roles;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = "An error occurred while fetching roles. Error: " + ex.Message;
            }

            return response;
        }


        public async Task<ResponseService<bool>> ResetAndChangePassword(string userId, string newPassword)
        {
            var response = new ResponseService<bool>();

            try
            {
                var user = await userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    response.success = false;
                    response.message = "User not found";
                }
                else
                {
                    // Check if the user has the "SuperAdmin" role
                    var isSuperAdmin = await userManager.IsInRoleAsync(user, Roles.SuperAdminRole);

                    if (!isSuperAdmin)
                    {
                        response.success = false;
                        response.message = "Permission denied. Only SuperAdmins can reset and change passwords.";
                    }
                    else
                    {
                        var token = await userManager.GeneratePasswordResetTokenAsync(user);

                        var result = await userManager.ResetPasswordAsync(user, token, newPassword);

                        if (result.Succeeded)
                        {
                            response.data = true;
                        }
                        else
                        {
                            response.success = false;
                            response.message = "Password reset and change failed.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = "An error occurred while resetting and changing the password. ex: " + ex.Message;
            }

            return response;
        }


        public async Task<ResponseService<GetUserDTO>> GetUserById(string id)
        {
            var response = new ResponseService<GetUserDTO>();
            try
            {
                ApplicationUser user = await userManager.FindByIdAsync(id);

                if (user == null)
                {
                    response.success = false;
                    response.message = "User not found";
                    return response;
                }

                string role = userManager.GetRolesAsync(user).Result.FirstOrDefault();

                var userWithRole = new GetUserDTO
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = role
                };

                response.data = userWithRole;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = "An error occurred while fetching the user. Error: " + ex.Message;
            }

            return response;
        }

        public async Task<ResponseTokenLoginDTO> login(LoginUserDTO userDto)
        {
            // check  -- create token
            ApplicationUser user = await userManager.FindByNameAsync(userDto.UserName);
            if (user != null)
            {
                bool found = await userManager.CheckPasswordAsync(user, userDto.Password);
                if (found)
                {
                    // create token

                    // 1)    claims to send in token
                    var claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Name, userDto.UserName)); // in token add name --> user name 
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id)); // id from application user
                    claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())); // take id different every create new

                    // 2) get roles --> add in token type of roll of user (admin or user)
                    var roles = await userManager.GetRolesAsync(user);
                    foreach (var roleitenm in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, roleitenm));
                    }

                    // signingCredentials  take algoritm(HmacSha256) and key (SecurityKey) ==> symantic system
                    SecurityKey seckey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Secretkey"]));
                    SigningCredentials signingCred = new SigningCredentials(seckey, SecurityAlgorithms.HmacSha256);

                    JwtSecurityToken myToken = new JwtSecurityToken(
                        issuer: config["JWT:ValidIssuer"], // url webapi -- swagger now
                        audience: config["JWT:ValidAudiance"], // uel consumer -- angular 
                        claims: claims,  // created before
                        expires: DateTime.Now.AddDays(1),
                        signingCredentials: signingCred
                        );

                    return new ResponseTokenLoginDTO
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(myToken),
                        expire = myToken.ValidTo,
                        SuccessorNot = 1,
                        Role = roles[0],
                        userName = userDto.UserName,
                        key = user.Id
                    };
                }
                else
                {
                    return
                    new ResponseTokenLoginDTO
                    {
                        Token = "Invalid Password",
                        expire = DateTime.Now,
                        SuccessorNot = 0
                    };
                }
            }
            return
                    new ResponseTokenLoginDTO
                    {
                        Token = "No user With this UserName",
                        expire = DateTime.Now,
                        SuccessorNot = 0
                    };
        }

        public async Task<string> register(RegisterUserDTO userDTO, string role)
        {
            ApplicationUser user = new ApplicationUser();
            user = mapper.Map<ApplicationUser>(userDTO);
            var userExist = await userManager.FindByEmailAsync(userDTO.Email);
            if (userExist != null)
            {
                return "User Already Exists";
            }

            if (await roleManager.RoleExistsAsync(role))
            {

                IdentityResult result = await userManager.CreateAsync(user, userDTO.Password);
                if (!result.Succeeded) return "User Failed To Create : " + result.Errors.FirstOrDefault().Description;

                // add role to user
                await userManager.AddToRoleAsync(user, role);
                return "user Created Successfully";
            }
            else
            {
                return "This Role Doesn't Exist";
            }




        }


    }
}
