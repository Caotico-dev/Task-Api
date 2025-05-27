using Mi_Task_Api.Model;
using Mi_Task_Api.ModelDto;
using Microsoft.EntityFrameworkCore;

namespace Mi_Task_Api.Managers
{
    public interface INoteBook
    {
        Task<NoteBook?> GetNotebook(string userId);
    }
    public interface ISharedTask
    {
        Task<MiTasks> GetTask(int TaskId);
    }
    public class ManagerBook : INoteBook, ISharedTask
    {
        private readonly UserDbContext _db;
        private readonly ILogger<ManagerBook> _logger;
        private NoteBook _notebook;
        public ManagerBook(UserDbContext userDbContext, ILogger<ManagerBook> logger)
        {
            _db = userDbContext;
            _logger = logger;
            _notebook = new NoteBook();
        }
        public async Task<NoteBook?> GetNotebook(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;


            this._notebook.Name = user.UserName;
            this._notebook.FriendsDtos = await this.GetFriends(userId);

            var (ListTask, ListScored) = await GetTaskAndScored(userId);

            this._notebook.ScoredTaskDtos = ListScored;
            this._notebook.MiTaskDtos = ListTask;

            this._notebook.ScoredTaskDtos.AddRange(await this.GetTaskScored(userId));
            this._notebook.TaskNoteds = await this.GetTaskNoteds(userId);

            return _notebook;
        }
        private async Task<List<ScoreTasksDto>> GetTaskScored(string UserId)
        {
            if (!string.IsNullOrWhiteSpace(UserId))
            {
                var scoredtask = await _db.ScoredTasks.Where(sd => sd.IdUser == UserId && (sd.Status != Status.Rejected.ToString() && sd.Status != Status.Block.ToString())).Select(sd => new ScoreTasksDto
                {
                    Id = sd.Id,
                    IdTask = sd.IdTask,
                    IdUser = sd.IdUser,
                    Details = sd.Details,
                    Status = sd.Status
                }).ToListAsync();

                return scoredtask;
            }
            return Array.Empty<ScoreTasksDto>().ToList();
        }
        private async Task<List<TaskNoted>> GetTaskNoteds(string UserId)
        {
            var tasknoted = await _db.ScoredTasks
                      .Where(sd => sd.IdUser == UserId && sd.Status == Status.Accepted.ToString())
                      .Select(sd => new TaskNoted
                      {
                          UserId = sd.MiTasks.IdUser,
                          TaskId = sd.MiTasks.TaskId,
                          Description = sd.MiTasks.Description,
                          Prioritis = sd.MiTasks.Prioritis,
                          Term = sd.MiTasks.Term,
                          Resource = sd.MiTasks.Resource,
                          Status = sd.MiTasks.Status,
                          Dependecy = sd.MiTasks.Dependecy,
                          SubTasks = sd.MiTasks.SubTasks,
                          Comments = sd.MiTasks.Comments,
                          ExpectedResults = sd.MiTasks.ExpectedResults

                      }) // Proyecta y combina las colecciones relacionadas
                      .ToListAsync(); // Trae todas las entidades relacionadas en forma de lista

            return tasknoted;
        }
        private async Task<(List<MiTaskDto>, List<ScoreTasksDto>)> GetTaskAndScored(string idUser)
        {
            if (!string.IsNullOrWhiteSpace(idUser))
            {
                var TaskAndScored = await _db.Tasks.Include(u => u.ScoredTasks).Where(u => u.IdUser == idUser).ToListAsync();

                var Task = new List<MiTaskDto>();
                var Scored = new List<ScoreTasksDto>();

                foreach (var item in TaskAndScored)
                {
                    Task.Add(new MiTaskDto
                    {
                        UserId = item.IdUser,
                        TaskId = item.TaskId,
                        Description = item.Description,
                        Prioritis = item.Prioritis,
                        Term = item.Term,
                        Resource = item.Resource,
                        Status = item.Status,
                        Dependecy = item.Dependecy,
                        SubTasks = item.SubTasks,
                        Comments = item.Comments,
                        ExpectedResults = item.ExpectedResults
                    });

                    foreach (var scored in item.ScoredTasks)
                    {
                        Scored.Add(new ScoreTasksDto
                        {
                            Id = scored.Id,
                            IdTask = scored.IdTask,
                            IdUser = scored.IdUser,
                            Details = scored.Details,
                            Status = scored.Status
                        });
                    }
                }

                return (Task, Scored);
            }
            return (Array.Empty<MiTaskDto>().ToList(), Array.Empty<ScoreTasksDto>().ToList());
        }
        private async Task<List<FriendShipDto>> GetFriends(string userid)
        {
            if (!string.IsNullOrWhiteSpace(userid))
            {
                var ListFriends = await _db.Friends.Where(s => (s.IdUser == userid || s.IdFriendShip == userid) &&
                                                               (s.Status == Status.Accepted.ToString() || s.Status == Status.Pending.ToString() || s.Status == Status.Block.ToString())).ToListAsync();

                if (ListFriends.Count > 0) return ListFriends.Select(s => new FriendShipDto
                {
                    Id = s.Id,
                    FriendX = s.IdUser,
                    FriendY = s.IdFriendShip,
                    Status = s.Status,
                }).ToList();

            }
            return Array.Empty<FriendShipDto>().ToList();
        }
        public async Task<MiTasks> GetTask(int TaskId)
        {

            if (TaskId > 0)
            {
                var task = await _db.Tasks.FindAsync(TaskId);
                if (task != null) return task;
            }
            return null;
        }
    }
}
