namespace Mi_Task_Api.Model
{
    public class MiTasks
    {
        public string IdUser { get; set; } = null!;
        public int TaskId { get; set; }
        public string Description { get; set; } = null!;
        public string Prioritis { get; set; } = null!;
        public DateOnly Term { get; set; }
        public string Resource { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string Dependecy { get; set; } = null!;
        public string SubTasks { get; set; } = null!;
        public string Comments { get; set; } = null!;
        public string ExpectedResults { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<ScoredTasks> ScoredTasks { get; set; } = new List<ScoredTasks>();
    }
}
