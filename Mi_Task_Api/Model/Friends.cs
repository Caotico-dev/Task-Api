namespace Mi_Task_Api.Model
{
    public class Friends
    {
        public int Id { get; set; }
        public string IdUser { get; set; } = null!;
        public string IdFriendShip { get; set; } = null!;  
        public string Status { get; set; } = null!;
        public DateOnly Date { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
