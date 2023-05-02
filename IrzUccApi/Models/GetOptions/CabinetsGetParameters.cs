using IrzUccApi.Models.PagingOptions;

namespace IrzUccApi.Models.GetOptions
{
    public class CabinetsGetParameters : SearchStringParameters
    {
        public TimeRangeGetParameters? TimeRange { get; set; }
    }
}
