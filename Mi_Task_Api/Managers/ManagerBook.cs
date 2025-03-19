using Mi_Task_Api.Model;
using Mi_Task_Api.ModelDto;
using Microsoft.EntityFrameworkCore;

namespace Mi_Task_Api.Managers
{
    public interface INoteBook
    {
        Task<NoteBook?> GetNotebook(string userId);
    }
    public class ManagerBook : INoteBook
    {
        private readonly UserDbContext _db;
        private readonly ILogger<ManagerBook> _logger;

        public ManagerBook(UserDbContext userDbContext, ILogger<ManagerBook> logger)
        {
            _db = userDbContext;
            _logger = logger;
        }


        public async Task<NoteBook?> GetNotebook(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }
            try
            {
                var user = await _db.Users
                .Include(u => u.MiTasks)
                .Include(u => u.ScoredTasks)
                .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return null;
                }


                var notebook = new NoteBook
                {
                    Name = user.UserName,
                    FriendsDtos = await this.GetFriends(user.Id),
                    ScoredTaskDtos = user.ScoredTasks.Select(st => new ScoreTasksDto
                    {
                        Id = st.Id,
                        IdTask = st.IdTask,
                        IdUser = st.IdUser,
                        Status = st.Status
                    }).ToList(),
                    MiTaskDtos = user.MiTasks.Select(mt => new MiTaskDto
                    {
                        UserId = mt.IdUser,
                        TaskId = mt.TaskId,
                        Description = mt.Description,
                        Prioritis = mt.Prioritis,
                        Term = mt.Term,
                        Resource = mt.Resource,
                        Status = mt.Status,
                        Dependecy = mt.Dependecy,
                        SubTasks = mt.SubTasks,
                        Comments = mt.Comments,
                        ExpectedResults = mt.ExpectedResults
                    }).ToList(),
                    TaskNoteds = await this.GetTaskNoteds(user.Id)
                };

                return notebook;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetNotebook");
                return Array.Empty<NoteBook>().FirstOrDefault();
            }


        }
        private async Task<List<TaskNoted>> GetTaskNoteds(string UserId)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetTaskNoteds");
                return Array.Empty<TaskNoted>().ToList();
            }

        }
        private async Task<List<FriendShipDto>> GetFriends(string userid)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(userid))
                {
                    var ListFriends = await _db.Friends.Where(s => (s.IdUser == userid || s.IdFriendShip == userid) && (s.Status == Status.Accepted.ToString() || s.Status == Status.Pending.ToString())).ToListAsync();


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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetFriends");
                return Array.Empty<FriendShipDto>().ToList();
            }
        }

    }
}
