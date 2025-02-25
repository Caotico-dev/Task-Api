using System.ComponentModel.DataAnnotations;

namespace Mi_Task_Api.ModelDto
{
    public class MiTaskDto
    {
        [Required]
        public string UserId { get; set; } = null!;        
        public int TaskId { get; set; } 
        [Required]
        [MaxLength(400)]
        public string Description { get; set; } = null!;
        [Required]
        [MaxLength(100)] 
        public string Prioritis { get; set; } = null!;
        [Required]
        public DateOnly Term { get; set; }
        [Required]
        [MaxLength(100)]
        public string Resource { get; set; } = null!;
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string Dependecy { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string SubTasks { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string Comments { get; set; } = null!;
        [Required]
        [MaxLength(100)]    
        public string ExpectedResults { get; set; } = null!;
    }
}
