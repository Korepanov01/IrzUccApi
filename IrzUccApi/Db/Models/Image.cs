using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Db.Models
{
    public class Image : BaseDbModel, IEntity
    {
        [MaxLength(10)]
        public string ContentType { get; set; } = string.Empty;
        public byte[] Content { get; set; } = Array.Empty<byte>();
    }
}
