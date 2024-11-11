using CourseWebsite.DTOs.Account;
using CourseWebsite.Models;
using CourseWebsite.Repositories.AccountRepository;
using HospAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CourseWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepoistory _accountRepo;
        public AccountController(IAccountRepoistory accountRepo)
        {
            _accountRepo = accountRepo;
        }


        // create account // register "post"
        [HttpPost("register")]
        [Produces("application/json")]
        //  [Authorize(Roles = "SuperAdmin")]

        public async Task<IActionResult> registration([FromForm] RegisterUserDTO userDto)
        {

            if (ModelState.IsValid)
            {
                string res = await _accountRepo.register(userDto, userDto.Role);
                if (res == "user Created Successfully") return Ok("user Created Successfully");
                else return BadRequest(res);
            }
            else
            {
                return BadRequest(ModelState);
            }

        }

        [HttpPost("registerNewUser")]
        [Produces("application/json")]

        public async Task<IActionResult> registerNewUser([FromForm] RegisterUserDTO userDto)
        {

            if (ModelState.IsValid)
            {
                string res = await _accountRepo.register(userDto, "Normal");
                if (res == "user Created Successfully") return Ok("user Created Successfully");
                else return BadRequest(res);
            }
            else
            {
                return BadRequest(ModelState);
            }

        }

        [HttpPost("UpdateUser")]
        [Produces("application/json")]
        // [Authorize(Roles = "SuperAdmin")]

        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserDTO Updateduser)
        {

            if (ModelState.IsValid)
            {
                string res = await _accountRepo.UpdateUser(Updateduser);
                if (res == "User updated successfully") return Ok("User updated successfully");
                else return BadRequest(res);
            }
            else
            {
                return BadRequest(ModelState);
            }

        }



        //check account // login   "post"
        [HttpPost("login")]
        [Produces("application/json")]

        public async Task<IActionResult> login(LoginUserDTO loginUser)
        {
            if (ModelState.IsValid)
            {

                ResponseTokenLoginDTO res = await _accountRepo.login(loginUser);
                return res.SuccessorNot == 0 ? Unauthorized(res.Token) : (IActionResult)Ok(res);
            }
            else return Unauthorized(ModelState);

        }


        [HttpGet]
        // [Authorize]
        //  [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<ResponseService<List<GetUserDTO>>>> GetAllUsers()
        {
            return Ok(await _accountRepo.GetAllUsers());
        }


        [HttpGet("GetAllRoles")]
        //  [Authorize]
        public async Task<ActionResult<ResponseService<List<string>>>> GetALLRoles()
        {
            return Ok(await _accountRepo.GetAllRoles());
        }

        [HttpGet("GetAllRolesWithId")]
        public async Task<ActionResult<ResponseService<List<Role>>>> GetAllRolesWithId()
        {
            return Ok(await _accountRepo.GetAllRolesWithId());
        }



        [HttpGet("GetUserById/{id}")]
        // [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<ResponseService<GetUserDTO>>> GetUserById(string id)
        {
            return Ok(await _accountRepo.GetUserById(id));
        }

        [HttpDelete]
        //  [Authorize]
        [Produces("application/json")]
        //  [Authorize(Roles = "SuperAdmin")]

        public async Task<ActionResult<ResponseService<List<string>>>> DeleteUser(string userID)
        {
            var response = await _accountRepo.DeleteUser(userID);
            return response != "User deleted successfully" ? NotFound(response) : Ok(response);
        }


        [HttpPost("resetPassword")]
        //   [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ResetAndChangePassword([FromBody] UpdatePasswordDTO UpdatePassDto)
        {
            var result = await _accountRepo.ResetAndChangePassword(UpdatePassDto.userId, UpdatePassDto.newPassword);

            if (result.success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }





    }
}
