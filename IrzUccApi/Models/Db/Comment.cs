using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Models.Db
{
    [Table("Comment")]
    public class Comment
    {
        public int Id { get; set; }
        [MaxLength(250)]
        public string Text { get; set; } = string.Empty;
        public DateTime DateTime { get; set; } = DateTime.UtcNow;

        public virtual NewsEntry NewsEntry { get; set; } = new NewsEntry();        
        public virtual AppUser Author { get; set; } = new AppUser();
    }
}
