using System.ComponentModel.DataAnnotations;

namespace Mi_Task_Api.ModelDto
{
    public class FriendShipDto
    {        
        public int Id { get; set; } 
        [Required]
        public string UserId { get; set; } = null!;
        [Required]
        public string FriendId { get; set; } = null!;
        [Required]  
        public string Status { get; set; } = null!;
    }
}
