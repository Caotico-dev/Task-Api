using Mi_Task_Api.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Mi_Task_Api.Managers
{
    public interface ITasks
    {

        Task<bool> AddTask(MiTasks task);
        Task<bool> AssignedTask(ScoredTasks scoredTasks);
        Task<bool> AssignedTaskStatus(int TaskId, string status);
        Task<bool> AssignedScoreTaskStatus(int ScoredTaskId, string status);
        Task<bool> RemoveTask(int Taskid);
        Task<bool> RemoveScoreTask(int taskId);
    }

    public class ManagerTask : ITasks
    {
        private readonly UserDbContext _context;
        private readonly IVerifyTask _VerifyTask;
        private readonly IStatus _status;
        private readonly ILogger<ManagerTask> _logger;
        private readonly string _userId;

        public ManagerTask(UserDbContext context, IVerifyTask verifyTask, IStatus status, ILogger<ManagerTask> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _VerifyTask = verifyTask;
            _status = status;
            _logger = logger;

            _userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

        }

        public async Task<bool> AddTask(MiTasks task)
        {
            try
            {
                if (task!.IdUser != _userId) task.IdUser = _userId;

                if (task != null)
                {
                    task.Prioritis = _VerifyTask.VerifyTaskPrioritis(task.Prioritis);
                    task.Status = _VerifyTask.VerifyTaskStatus(task.Status);
                    await _context.Tasks.AddAsync(task);
                    var save = await _context.SaveChangesAsync();
                    if (save > 0) return true;

                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddTask");
                return false;
            }
        }
        public async Task<bool> AssignedTask(ScoredTasks scoredTasks)
        {
            try
            {
                var task = await _context.Tasks.FindAsync(scoredTasks.IdTask);

                if (task!.IdUser != _userId) return false;

                if (task != null)
                {
                    scoredTasks.Status = Status.Pending.ToString();
                    await _context.ScoredTasks.AddAsync(scoredTasks);
                    var save = await _context.SaveChangesAsync();
                    if (save > 0) return true;

                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AssignedTask");
                return false;
            }
        }

        public async Task<bool> AssignedTaskStatus(int TaskId, string status)
        {
            try
            {
                if (TaskId > 0 && status != null)
                {
                    var task = await _context.Tasks.FindAsync(TaskId);

                    if (task!.IdUser != _userId) return false;

                    if (task != null)
                    {
                        task.Status = _VerifyTask.VerifyTaskStatus(status);
                        _context.Tasks.Update(task);
                        var save = await _context.SaveChangesAsync();
                        if (save > 0) return true;

                    }
                }
                return false;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AssignedTaskStatus");
                return false;
            }
        }

        public async Task<bool> RemoveTask(int Taskid)
        {
            try
            {
                if (Taskid > 0)
                {
                    var task = await _context.Tasks.FindAsync(Taskid);

                    if (task!.IdUser != _userId) return false;

                    if (task != null)
                    {
                        _context.Tasks.Remove(task);
                        var save = await _context.SaveChangesAsync();
                        if (save > 0) return true;

                    };
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RemoveTask");
                return false;
            }
        }
        public async Task<bool> RemoveScoreTask(int taskId)
        {
            try
            {
                if (taskId > 0)
                {
                    var taskscore = await _context.ScoredTasks.Include(s => s.MiTasks).FirstOrDefaultAsync(b => b.IdTask == taskId);

                    if (taskscore!.MiTasks.IdUser != _userId) return false;

                    if (taskscore != null) _context.ScoredTasks.Remove(taskscore);

                    var save = await _context.SaveChangesAsync();
                    if (save > 0) return true;

                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RemoveScoreTask");
                return false;
            }
        }

        public async Task<bool> AssignedScoreTaskStatus(int ScoredTaskId, string status)
        {
            try
            {
                if (ScoredTaskId > 0)
                {
                    var scoredtask = await _context.ScoredTasks.Include(t => t.MiTasks).FirstOrDefaultAsync(sc => sc.IdTask == ScoredTaskId);
                    if (scoredtask == null) return false;

                    if (scoredtask!.IdUser != _userId && scoredtask.MiTasks.IdUser != _userId) return false;                  

                    if (scoredtask.MiTasks.IdUser != _userId && status == Status.Block.ToString()) return false;

                    scoredtask!.Status = _status.VerifyStatus(status);
                    _context.ScoredTasks.Update(scoredtask);
                    int save = await _context.SaveChangesAsync();

                    if (save > 0) return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AssignedScoreTaskStatus");
                return false;
            }
        }
    }
}
