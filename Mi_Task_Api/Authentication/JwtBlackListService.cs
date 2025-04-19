namespace Mi_Task_Api.Authentication
{
    public interface ITokenBlackList
    {
        int CountBlackList { get; }
    }

    public interface IAddBlackList : ITokenBlackList
    {
        void AddToBlackList(string token);
    }

    public interface ICheckBlackList : ITokenBlackList
    {
        bool IsBlackList(string token);
    }

    public interface IClearBlackList : ITokenBlackList
    {
        void ClearBlackList();
        void RemoveThousandItems();
    }

    public class JwtBlackListService : IAddBlackList, ICheckBlackList, IClearBlackList
    {
        private readonly HashSet<string> _blacklist = new();

        public int CountBlackList => _blacklist.Count;

        public void AddToBlackList(string token)
        {
            if(_blacklist.Contains(token))
            {
                throw new ArgumentException("This token has already been added");
            }
            _blacklist.Add(token);  
        }

        public bool IsBlackList(string token) => _blacklist.Contains(token);

        public void ClearBlackList() => _blacklist.Clear();
        public void  RemoveThousandItems()
        {
            var ToRemove = _blacklist.Take(1000).ToList();
            foreach (var item in ToRemove)
            {
                _blacklist.Remove(item);
            }
        }
    }

}
