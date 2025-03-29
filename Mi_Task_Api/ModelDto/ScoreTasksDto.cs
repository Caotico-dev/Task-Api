namespace Mi_Task_Api.ModelDto
{
    public class ScoreTasksDto
    {
        public int Id { get; set; }
        public string IdUser { get; set; } = null!;
        public int IdTask { get; set; }
        public string Details { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
