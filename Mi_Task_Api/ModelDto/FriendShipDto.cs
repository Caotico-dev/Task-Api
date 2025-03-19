using System.ComponentModel.DataAnnotations;

namespace Mi_Task_Api.ModelDto
{
    public class FriendShipDto
    {
        public int Id { get; set; }
        [Required]
        public string FriendX { get; set; } = null!;
        [Required]
        public string FriendY { get; set; } = null!;
        [Required]
        public string Status { get; set; } = null!;
    }
}
