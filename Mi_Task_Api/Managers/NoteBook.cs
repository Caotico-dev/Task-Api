using Mi_Task_Api.ModelDto;

namespace Mi_Task_Api.Managers
{
    public class NoteBook
    {
        public string? Name { get; set; }
        public string? Token { get; set; }
        public List<ScoreTasksDto>? ScoredTaskDtos { get; set; }
        public List<FriendShipDto>? FriendsDtos { get; set; }
        public List<MiTaskDto>? MiTaskDtos { get; set; }
        public List<TaskNoted>? TaskNoteds { get; set; }

    }
}
