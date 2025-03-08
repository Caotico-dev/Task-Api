using Mi_Task_Api.Model;
using Mi_Task_Api.ModelDto;
using System.Collections;
namespace Mi_Task_Api.Managers
{
    public interface IFriends
    {
        Task<bool> AddFriend(Friends friends);
        Task<bool> AssignedFriendStatus(int id, string status);
        Task<List<Friends>> GetFriends(Friends friends);
    }   
  

    public class ManagerFriends : IFriends
    {
        
        private readonly UserDbContext _db;
        private readonly IStatus _status;
        private readonly ILogger<ManagerFriends> _logger;   
        public ManagerFriends(UserDbContext db,IStatus status,ILogger<ManagerFriends> logger)
        {
            _db = db;          
            _status = status;
            _logger = logger;
        }

        public async Task<bool> AddFriend(Friends friends)
        {
            try
            {
                if (friends != null)
                {
                    friends.Status = Status.Pending.ToString();
                    friends.Date = DateOnly.FromDateTime(DateTime.Now);
                    await _db.Friends.AddAsync(friends);

                    var Save = await _db.SaveChangesAsync();

                    if (Save > 0)
                    {
                        return true;
                    }

                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error in AddFriend");   
                return false;
            }

        }

        public async Task<bool> AssignedFriendStatus(int id,string status)
        {
            try
            {
                if (id > 0 && string.IsNullOrWhiteSpace(status))
                {
                    var FriendShip = await _db.Friends.FindAsync(id);
                    if (FriendShip == null)
                    {
                        return false;
                    }
                    FriendShip!.Status = _status.VerifyStatus(status);
                    _db.Friends.Update(FriendShip);
                    var save = await _db.SaveChangesAsync();

                    if (save > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AssignedFriendStatus");
                return false;
            }
        }

        public async Task<List<Friends>> GetFriends(Friends friends)
        {
            try
            {
                if (friends != null)
                {
                    var ListFriends = await Task.Run(() => { return _db.Friends.Where(f => f.IdUser == friends.IdUser && f.Status == Status.Accepted.ToString()).ToList(); });
                    if (ListFriends.Count > 0)
                    {
                        return ListFriends;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetFriends");    
                return null;
            }   
        }
        
    }
}
