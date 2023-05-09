namespace IrzUccApi.Models.PagingOptions
{
    public class PagingParameters
    {
        private int _pageIndex = 0;
        private int _pageSize = 10;

        public int PageIndex
        {
            get => _pageIndex;
            set => _pageIndex = value < 0 ? 0 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 0 ? 0 : (value > 50 ? 50 : value);
        }
    }
}
