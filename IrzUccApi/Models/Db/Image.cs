using IrzUccApi.Enums;
using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Db
{
    public class Image : BaseDbModel, IEntity
    {
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(10)]
        public string Extension { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;

        public ImageSources Source { get; set; }
        public Guid SourceId { get; set; }
    }
}
