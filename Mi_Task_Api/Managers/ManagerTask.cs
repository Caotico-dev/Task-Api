using Mi_Task_Api.Model;
using Microsoft.EntityFrameworkCore;

namespace Mi_Task_Api.Managers
{
    public interface ITasks
    {
        Task<bool> AddTask(MiTasks task);
        Task<MiTasks> GetTask(int TaskId);
        Task<bool> AssignedTask(ScoredTasks scoredTasks);
        Task<bool> AssignedTaskStatus(int TaskId,string status);    
        Task<bool> RemoveTask(int Taskid);
        Task<bool> RemoveScoreTask(int taskId);
    }   
    
    public class ManagerTask : ITasks
    {
        private readonly UserDbContext _context;
        private readonly IVerifyTask _VerifyTask;
        public ManagerTask(UserDbContext context,IVerifyTask verifyTask)
        {
            _context = context;
            _VerifyTask = verifyTask;
        }

        public async Task<bool> AddTask(MiTasks task)
        {
            try
            {
                if(task != null)
                {
                    task.Prioritis = _VerifyTask.VerifyTaskPrioritis(task.Prioritis);
                    task.Status = _VerifyTask.VerifyTaskStatus(task.Status);
                    await _context.Tasks.AddAsync(task);
                    var save = await _context.SaveChangesAsync();
                    if (save > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<bool> AssignedTask(ScoredTasks scoredTasks)
        {
            try
            {
                var task = await _context.Tasks.FindAsync(scoredTasks.IdTask);
                if (task != null)
                {
                    scoredTasks.Status = Status.Pending.ToString();
                    await _context.ScoredTasks.AddAsync(scoredTasks);
                    var save = await _context.SaveChangesAsync();
                    if (save > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                    if (task != null)
                    {
                        task.Status = _VerifyTask.VerifyTaskStatus(status);
                        _context.Tasks.Update(task);
                        var save = await _context.SaveChangesAsync();
                        if (save > 0)
                        {
                            return true;
                        }
                    }                    
                }
                return false;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<MiTasks> GetTask(int TaskId)
        {
            try
            {
                if (TaskId > 0)
                {
                    var task = await _context.Tasks.FindAsync(TaskId);
                    if (task != null)
                    {
                        return task;
                    }                    
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }    
        }

        public async Task<bool> RemoveTask(int Taskid)
        {
            try
            {
                if (Taskid > 0)
                {
                    var task = await _context.Tasks.FindAsync(Taskid);
                    if (task != null)
                    {
                        _context.Tasks.Remove(task);
                        var save = await _context.SaveChangesAsync();
                        if (save > 0)
                        {
                            return true;
                        }
                    }                    
                }
                return false;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<bool> RemoveScoreTask(int taskId)
        {
            try
            {
                if(taskId > 0)
                {
                    var taskscore = await _context.ScoredTasks.FindAsync(taskId);
                    if(taskscore != null)
                    {
                       _context.ScoredTasks.Remove(taskscore);                       
                    }
                    var save = await _context.SaveChangesAsync();
                    if(save > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }        
      
    }
}
