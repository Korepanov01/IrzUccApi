using IrzUccApi.Models.PagingOptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IrzUccApi.Models.GetOptions
{
    public class EventsGetParameters : PagingParameters
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
    }
}
