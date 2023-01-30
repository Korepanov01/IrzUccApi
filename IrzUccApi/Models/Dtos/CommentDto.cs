﻿namespace IrzUccApi.Models.Dtos
{
    public class CommentDto
    {
        public CommentDto(int id, string text, DateTime dateTime, UserHeaderDto user)
        {
            Id = id;
            Text = text;
            DateTime = dateTime;
            User = user;
        }

        public int Id { get; }
        public string Text { get; }
        public DateTime DateTime { get; }
        public UserHeaderDto User { get; }

    }
}
