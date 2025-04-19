using Mi_Task_Api.Authentication;
using Mi_Task_Api.Managers;
using Mi_Task_Api.Model;
using Mi_Task_Api.ModelDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Mi_Task_Api.Controllers
{
    [Route("/task")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IControllerAuthentication _controllerAuthentication;
        private readonly IFriends _friends;
        private readonly ITasks _task;
        private readonly INoteBook _noteBook;
        private readonly ILogger<TaskController> _logger;
        private readonly IAddBlackList _addBlackList;

        public TaskController(UserManager<User> userManager,
                              SignInManager<User> signInManager,
                              IControllerAuthentication controllerAuthentication,
                              IFriends friends,
                              ITasks tasks,
                              INoteBook book,
                              ILogger<TaskController> logger,
                              IAddBlackList addBlackList)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _controllerAuthentication = controllerAuthentication;
            _friends = friends;
            _task = tasks;
            _noteBook = book;
            _logger = logger;
            _addBlackList = addBlackList;
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

            }catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
           
        } 
        [Authorize()]
        [HttpPost("/addfriend")]
        public async Task<IActionResult> ReceivingAddFriend([FromBody] FriendsDto friends)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var friendShip = new Friends
                    {
                        IdUser = friends.UserId,
                        IdFriendShip = friends.FriendId,
                    };

                    bool result = await _friends.AddFriend(friendShip);
                    if (result)
                    {
                        return Ok("Friend Added Successfully");
                    }
                    else
                    {
                        return StatusCode(500, "Failed to Add Friend");
                    }
                }
                return BadRequest("Invalid Data");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ReceivingAddFriend");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in ReceivingAddFriend");
            }

        }
        [Authorize]
        [HttpPatch("/assignedstatus={id},{status}")]
        public async Task<IActionResult> ReceivingAssignedStatus(int id, string status)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    bool result = await _friends.AssignedFriendStatus(id, status);
                    if (result)
                    {
                        return Ok("Status Assigned Successfully");
                    }
                    else
                    {
                        return StatusCode(500, "Failed to Assign Status");
                    }
                }
                return BadRequest("Invalid Data");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ReceivingAssignedStatus");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in ReceivingAssignedStatus");
            }

        }
        [Authorize]
        [HttpPost("/addtask")]
        public async Task<IActionResult> ReceivingTask([FromBody] MiTaskDto addtask)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var task = new MiTasks
                    {
                        IdUser = addtask.UserId,
                        Description = addtask.Description,
                        Prioritis = addtask.Prioritis,
                        Term = addtask.Term,
                        Resource = addtask.Resource,
                        Status = addtask.Status,
                        Dependecy = addtask.Dependecy,
                        SubTasks = addtask.SubTasks,
                        Comments = addtask.Comments,
                        ExpectedResults = addtask.ExpectedResults
                    };
                    bool result = await _task.AddTask(task);
                    if (result)
                    {
                        return Ok("Task Added Successfully");
                    }
                    return StatusCode(500, "Failed to add task");

                }
                return BadRequest("Invalid Data");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ReceivingTask");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in ReceivingTask");
            }

        }
        [Authorize]
        [HttpPost("/addscoredtask")]
        public async Task<IActionResult> ReceivingScoreTask([FromBody] ScoredTaskDto addScoredTask)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var scoredtask = new ScoredTasks
                    {
                        IdUser = addScoredTask.IdUser,
                        IdTask = addScoredTask.IdTask,
                        Details = addScoredTask.Details,

                    };
                    bool result = await _task.AssignedTask(scoredtask);
                    if (result)
                    {
                        return Ok("Task assigned Successfully");
                    }
                    return StatusCode(500, "Failed to add score task.");
                }
                return BadRequest("Invalid Data");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ReceivingScoreTask");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in ReceivingScoreTask");
            }


        }
        [Authorize]
        [HttpPatch("/scoretaskstatus={scoredtaskid},{status}")]
        public async Task<IActionResult> ReceivingScoredTaskStatus(int scoredtaskid, string status)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool result = await _task.AssignedScoreTaskStatus(scoredtaskid, status);
                    if (result)
                    {
                        return Ok("Successfully assigned status");
                    }
                    return NotFound("Failed to assigned status");
                }
                return BadRequest("Invalid Data");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ReceivingScoredTaskStatus");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in ReceivingScoredTaskStatus");
            }

        }
        [Authorize]
        [HttpPatch("/taskstatus={taskId},{status}")]
        public async Task<IActionResult> ReceivingTaskStatus(int taskId, string status)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool result = await _task.AssignedTaskStatus(taskId, status);
                    if (result)
                    {
                        return Ok("Successfully assigned status");
                    }
                    return NotFound("Failed to assigned status");
                }
                return BadRequest("Invalid Data");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ReceivingTaskStatus");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in ReceivingTaskStatus");
            }


        }
        [Authorize]
        [HttpDelete("/taskremove={taskid}")]
        public async Task<IActionResult> ReceivingRemoveTask(int taskid)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool result = await _task.RemoveTask(taskid);
                    if (result)
                    {
                        return NoContent();
                    }
                    return NotFound("Failed to remove task");
                }
                return BadRequest("Invalid Data");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ReceivingRemoveTask");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in ReceivingRemoveTask");
            }

        }
        [Authorize]
        [HttpDelete("/removescoretask={taskid}")]
        public async Task<IActionResult> ReceivingRemoveScoreTask(int taskid)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool result = await _task.RemoveScoreTask(taskid);
                    if (result)
                    {
                        return NoContent();
                    }
                    return NotFound("Failed to remove score task");
                }
                return BadRequest("Invalid Data");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ReceivingRemoveScoreTask");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in ReceivingRemoveScoreTask");
            }

        }
    }
}


