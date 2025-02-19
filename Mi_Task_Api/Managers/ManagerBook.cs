using Mi_Task_Api.Model;
using Mi_Task_Api.ModelDto;
using Microsoft.EntityFrameworkCore;

namespace Mi_Task_Api.Managers
{
    public interface INoteBook
    {
        Task<NoteBook?> GetNotebook(string userId);
    }
    public class ManagerBook:INoteBook
    {
        private UserDbContext _db;
        public ManagerBook(UserDbContext userDbContext)
        {
            _db = userDbContext;
        }

        public async Task<NoteBook?> GetNotebook(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            
            var user = await _db.Users
                .Include(u => u.MiTasks)
                .Include(u => u.ScoredTasks)
                .Include(u => u.Friends)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }

            
            var notebook = new NoteBook
            {
                Name = user.UserName,
                FriendsDtos = user.Friends.Select(f => new FriendShipDto
                {
                    Id = f.Id,
                    UserId = f.IdUser,
                    FriendId = f.IdFriendShip,
                    Status = f.Status
                }).ToList(),
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
                    Description = mt.Description,
                    Prioritis = mt.Prioritis,
                    Term = mt.Term,
                    Resource = mt.Resource,
                    Status = mt.Status,
                    Dependecy = mt.Dependecy,
                    SubTasks = mt.SubTasks,
                    Comments = mt.Comments,
                    ExpectedResults = mt.ExpectedResults
                }).ToList()
            };

            return notebook;
        }
    }
}
