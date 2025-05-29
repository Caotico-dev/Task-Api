using Mi_Task_Api.Authentication;
using Mi_Task_Api.Managers;
using Mi_Task_Api.Model;
using Mi_Task_Api.ModelDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Mi_Task_Api.Controllers
{
    [ApiController]
    public class SessionController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IControllerAuthentication _controllerAuthentication;
        private readonly INoteBook _noteBook;
        private readonly ILogger<SessionController> _logger;
        private readonly IAddBlackList _addBlackList;
        public SessionController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IControllerAuthentication controllerAuthentication,
            ILogger<SessionController> logger,
            IAddBlackList addBlackList,
            INoteBook noteBook)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _controllerAuthentication = controllerAuthentication;
            _logger = logger;
            _addBlackList = addBlackList;
            _noteBook = noteBook;
        }
        [HttpPost("/registar")]
        public async Task<IActionResult> RegisterUser([FromBody] Register register)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new User
                    {
                        UserName = register.Name,
                        Email = register.Email
                    };

                    var result = await _userManager.CreateAsync(user, register.Password);
                    if (result.Succeeded)
                    {
                        return Ok("User Created Successfully");
                    }
                    else
                    {
                        return BadRequest(result.Errors);
                    }
                }
                return BadRequest("Invalid Data");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RegisterUser");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in RegisterUser");
            }

        }
        [HttpPost("/sign")]
        public async Task<IActionResult> SignUser([FromBody] Login login)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var user = await _userManager.FindByEmailAsync(login.Email);
                    if (user == null)
                    {
                        return Unauthorized("unauthorized user");
                    }


                    var result = this._signInManager.PasswordSignInAsync(user.UserName!, login.Password, isPersistent: false, lockoutOnFailure: false);
                    if (result.Result.Succeeded)
                    {
                        var token = _controllerAuthentication.GenerateJwtToken(user!);
                        var note = await _noteBook.GetNotebook(user.Id);
                        note!.Token = token;
                        return Ok(note);

                    }
                }
                return Unauthorized("unauthorized user");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SignUser");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in SignUser");
            }

        }
        [Authorize]
        [HttpPost("/signout")]
        public IActionResult SignOut()
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrEmpty(token))
                {
                    token = token.Replace("Bearer", "").Trim();
                    _addBlackList.AddToBlackList(token);

                    return Ok("Session Closed Successfully");
                }
                return BadRequest("Session not Closed Successfully");

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
    }
}
