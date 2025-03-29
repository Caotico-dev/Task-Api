namespace Mi_Task_Api.Managers
{
    enum Status
    {
        Pending,
        Accepted,
        Rejected,
        Block,
    }
    enum TaskStatus
    {
        Pending,
        InProgress,
        Completed
    }
    enum TaskPrioritis
    {
        High,
        Medium,
        Low
    }
    public interface IVerifyTask
    {
        string VerifyTaskPrioritis(string prioritis);
        string VerifyTaskStatus(string status);
    }
    public interface IStatus
    {
        string VerifyStatus(string status);
    }
    public class VerifyNoteBook : IVerifyTask, IStatus
    {
        public VerifyNoteBook() { }
        public string VerifyTaskPrioritis(string prioritis)
        {
            if (Enum.TryParse(prioritis, out TaskPrioritis parsedPrioritis))
            {
                return parsedPrioritis.ToString();
            }
            return TaskPrioritis.Low.ToString();
        }
        public string VerifyTaskStatus(string status)
        {
            if (Enum.TryParse(status, out TaskStatus parsedStatus))
            {
                return parsedStatus.ToString();
            }
            return TaskStatus.Pending.ToString();
        }
        public string VerifyStatus(string status)
        {
            if (Enum.TryParse(status,out Status parsedStatus) )
            {
                return status;  
            }            
            return Status.Pending.ToString();
        }
    }
}
