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
            if (prioritis == TaskPrioritis.High.ToString())
            {
                return TaskPrioritis.High.ToString();
            }
            else if (prioritis == TaskPrioritis.Medium.ToString())
            {
                return TaskPrioritis.Medium.ToString();
            }
            return TaskPrioritis.Low.ToString();
        }
        public string VerifyTaskStatus(string status)
        {
            if (status == TaskStatus.InProgress.ToString())
            {
                return TaskStatus.InProgress.ToString();
            }
            else if (status == TaskStatus.Completed.ToString())
            {
                return TaskStatus.Completed.ToString();
            }
            return TaskStatus.Pending.ToString();
        }
        public string VerifyStatus(string status)
        {
            if (status == Status.Accepted.ToString())
            {
                return Status.Accepted.ToString();
            }
            else if (status == Status.Rejected.ToString())
            {
                return Status.Rejected.ToString();
            }
            else if(status == Status.Block.ToString())
            {
                return Status.Block.ToString();
            }           
            return Status.Pending.ToString();
        }
    }
}
