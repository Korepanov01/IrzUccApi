using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Models.PagingOptions
{
    public class SearchStringParameters : PagingParameters
    {
        public string? SearchString {  get; set; }
    }
}
