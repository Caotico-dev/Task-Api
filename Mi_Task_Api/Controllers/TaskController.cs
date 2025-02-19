using Mi_Task_Api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mi_Task_Api.ModelDto;
using Mi_Task_Api.Authentication;
using Mi_Task_Api.Managers;

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
        public TaskController(UserManager<User> userManager,
                              SignInManager<User> signInManager,
                              IControllerAuthentication controllerAuthentication,
                              IFriends friends,
                              ITasks tasks,
                              INoteBook book)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _controllerAuthentication = controllerAuthentication;
            _friends = friends;
            _task = tasks;
            _noteBook = book;

        }

        [HttpPost("/registar")]
        public async Task<IActionResult> RegisterUser([FromBody] Register register)
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
        [HttpPost("/sign")]
        public async Task<IActionResult> SignUser([FromBody] Login login)
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
        [HttpPost("/addfriend")]
        public async Task<IActionResult> ReceivingAddFriend([FromBody] FriendsDto friends)
        {
            if (ModelState.IsValid)
            {
                var friendShip = new Friends
                {
                    IdUser = friends.UserId,
                    IdFriendShip = friends.FriendId,
                };

                var result = await _friends.AddFriend(friendShip);
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
        [HttpPatch("/assignedstatus={id},{status}")]
        public async Task<IActionResult> ReceivingAssignedStatus(int id, string status)
        {
            if (ModelState.IsValid)
            {               

                var result = await _friends.AssignedFriendStatus(id,status);
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
        [HttpPost("/addtask")]
        public async Task<IActionResult> ReceivingTask([FromBody] MiTaskDto addtask)
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
                var result = await _task.AddTask(task);
                if (result)
                {
                    return Ok("Task Added Successfully");
                }
                return StatusCode(500, "Failed to add task");

            }
            return BadRequest("Invalid Data");
        }
        [HttpPost("/addscoredtask")]
        public async Task<IActionResult> ReceivingScoreTask([FromBody] ScoredTaskDto addScoredTask)
        {
            if (ModelState.IsValid)
            {
                var scoredtask = new ScoredTasks
                {
                    IdUser = addScoredTask.IdUser,
                    IdTask = addScoredTask.IdTask,

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
        [HttpPatch("/taskstatus={taskId},{status}")]
        public async Task<IActionResult> ReceivingTaskStatus(int taskId,string status)
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
        [HttpDelete("/taskremove={taskid}")]
        public async Task<IActionResult> ReceivingRemoveTask(int taskid)
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
        [HttpDelete("/removescoretask={taskid}")]
        public async Task<IActionResult> ReceivingRemoveScoreTask(int taskid)
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
    }
}


