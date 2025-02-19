using Microsoft.AspNetCore.Identity;

namespace Mi_Task_Api.Model
{
    public class User:IdentityUser
    {
        public virtual ICollection<ScoredTasks> ScoredTasks { get; set; } = new List<ScoredTasks>(); 
        public virtual ICollection<Friends> Friends { get; set; } = new List<Friends>();
        public  virtual ICollection<MiTasks> MiTasks { get; set; } = new List<MiTasks>();
    }
}
