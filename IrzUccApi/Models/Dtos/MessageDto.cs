namespace IrzUccApi.Models.Dtos
{
    public class MessageDto
    {
        public MessageDto(
            int id, 
            string? text, 
            string? image, 
            DateTime dateTime, 
            string senderId)
        {
            Id = id;
            Text = text;
            Image = image;
            DateTime = dateTime;
            SenderId = senderId;
        }

        public int Id { get; }
        public string? Text { get; }
        public string? Image { get; }
        public DateTime DateTime { get; }
        public string SenderId { get; }
    }
}
