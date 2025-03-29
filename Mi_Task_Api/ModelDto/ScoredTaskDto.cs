using System.ComponentModel.DataAnnotations;

namespace Mi_Task_Api.ModelDto
{
    public class ScoredTaskDto
    {
        [Required]
        public string IdUser { get; set; } = null!;
        [Required]
        public int IdTask { get; set; }
        [Required]
        [MaxLength(400)]        
        public string Details { get; set; } = null!;    
    }
}
