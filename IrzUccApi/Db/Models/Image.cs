using IrzUccApi.Enums;
using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Db.Models
{
    public class Image : BaseDbModel, IEntity
    {
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(10)]
        public string ContentType { get; set; } = string.Empty;
        public byte[] Content { get; set; } = Array.Empty<byte>();

        public ImageSources Source { get; set; }
        public Guid SourceId { get; set; }
    }
}
