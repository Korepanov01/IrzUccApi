using IrzUccApi.Models.PagingOptions;

namespace IrzUccApi.Models.GetOptions
{
    public class CabinetsGetParameters : SearchStringParameters
    {
        public bool FreeOnly { get; set; } = false;
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
    }
}
