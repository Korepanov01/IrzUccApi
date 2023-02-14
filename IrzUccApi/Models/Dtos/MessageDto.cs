﻿namespace IrzUccApi.Models.Dtos
{
    public class MessageDto
    {
        public MessageDto(
            Guid id,
            string? text,
            string? image,
            DateTime dateTime,
            Guid senderId)
        {
            Id = id;
            Text = text;
            ImageId = image;
            DateTime = dateTime;
            SenderId = senderId;
        }

        public Guid Id { get; }
        public string? Text { get; }
        public string? ImageId { get; }
        public DateTime DateTime { get; }
        public Guid SenderId { get; }
    }
}
