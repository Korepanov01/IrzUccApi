using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.PagingOptions
{
    public class PagingParameters
    {
        private int _pageIndex = 1;
        private int _pageSize = 10;

        public int PageIndex 
        { 
            get => _pageIndex; 
            set => _pageIndex = value < 1 ? 1 : value; 
        }

        public int PageSize 
        { 
            get => _pageSize; 
            set => _pageSize = value < 10 ? 10 : (value > 50 ? 50 : value); 
        }
    }
}
