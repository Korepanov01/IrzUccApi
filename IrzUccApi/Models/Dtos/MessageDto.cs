using IrzUccApi.Models.Db;

namespace IrzUccApi.Models.Dtos
{
    public class MessageDto
    {
        public MessageDto(
            int id, 
            string? text, 
            bool isReaded, 
            string? image, 
            DateTime dateTime, 
            string senderId)
        {
            Id = id;
            Text = text;
            IsReaded = isReaded;
            Image = image;
            DateTime = dateTime;
            SenderId = senderId;
        }

        public int Id { get; }
        public string? Text { get; }
        public bool IsReaded { get; }
        public string? Image { get; }
        public DateTime DateTime { get; }
        public string SenderId { get; }
    }
}
