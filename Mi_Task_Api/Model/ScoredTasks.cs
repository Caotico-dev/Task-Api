namespace Mi_Task_Api.Model
{
    public class ScoredTasks
    {
        public int Id { get; set; }
        public string IdUser { get; set; } = null!;
        public int IdTask { get; set; }
        public string Details { get; set; } = null!;    
        public string Status { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual MiTasks MiTasks { get; set; } = null!;
    }
}
