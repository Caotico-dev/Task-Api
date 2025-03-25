using Mi_Task_Api.Model;
using System.Security.Claims;
namespace Mi_Task_Api.Managers
{
    public interface IFriends
    {
        Task<bool> AddFriend(Friends friends);
        Task<bool> AssignedFriendStatus(int id, string status);
    }


    public class ManagerFriends : IFriends
    {

        private readonly UserDbContext _db;
        private readonly IStatus _status;
        private readonly ILogger<ManagerFriends> _logger;
        private readonly string _userId;
        public ManagerFriends(UserDbContext db, IStatus status, ILogger<ManagerFriends> logger, IHttpContextAccessor IHttpContextAccessor)
        {
            _db = db;
            _status = status;
            _logger = logger;
            _userId = IHttpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        }

        public async Task<bool> AddFriend(Friends friends)
        {
            try
            {
                if (friends != null)
                {
                    if (friends.IdUser != _userId) friends.IdUser = _userId;

                    friends.Status = Status.Pending.ToString();
                    friends.Date = DateOnly.FromDateTime(DateTime.Now);
                    await _db.Friends.AddAsync(friends);

                    var Save = await _db.SaveChangesAsync();

                    if (Save > 0) return true;


                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddFriend");
                return false;
            }

        }

        public async Task<bool> AssignedFriendStatus(int id, string status)
        {
            try
            {
                if (id > 0 && string.IsNullOrWhiteSpace(status))
                {

                    var FriendShip = await _db.Friends.FindAsync(id);

                    if (FriendShip == null) return false;

                    if (FriendShip!.IdUser != _userId && FriendShip!.IdFriendShip != _userId) return false;

                    if (FriendShip.IdUser != _userId && status == Status.Block.ToString()) return false;

                    FriendShip!.Status = _status.VerifyStatus(status);
                    _db.Friends.Update(FriendShip);
                    var save = await _db.SaveChangesAsync();

                    if (save > 0) return true;

                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AssignedFriendStatus");
                return false;
            }
        }

    }
}
