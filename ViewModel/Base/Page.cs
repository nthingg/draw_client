namespace ViewModel.Base
{
    public class Page<T>
    {
        public int TotalItemsCount { get; set; }
        public int PageSize { get; set; }
        public int TotalPagesCount
        {
            get
            {
                var temp = TotalItemsCount / PageSize;
                if (TotalItemsCount % PageSize == 0)
                {
                    return temp;
                }
                return temp + 1;
            }
        }
        public int PageIndex { get; set; }
        public bool Next => PageIndex + 1 < TotalPagesCount;
        public bool Previous => PageIndex > 0;
        public ICollection<T> Items { get; set; }
    }
}
