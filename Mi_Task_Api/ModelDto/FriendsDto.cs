using System.ComponentModel.DataAnnotations;

namespace Mi_Task_Api.ModelDto
{
    public class FriendsDto
    {        
        [Required]
        public string UserId { get; set; } = null!;
        [Required]
        public string FriendId { get; set; } = null!;
    }
}
