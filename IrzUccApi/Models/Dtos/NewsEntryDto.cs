﻿namespace IrzUccApi.Models.Dtos
{
    public class NewsEntryDto
    {
        public NewsEntryDto(
            int id,
            string title,
            string text,
            string? image,
            DateTime dateTime,
            bool isLiked,
            int likesCount,
            UserHeaderDto author,
            bool isPublic)
        {
            Id = id;
            Title = title;
            Text = text;
            Image = image;
            DateTime = dateTime;
            IsLiked = isLiked;
            LikesCount = likesCount;
            Author = author;
            IsPublic = isPublic;
        }

        public int Id { get; }
        public string Title { get; }
        public string Text { get; }
        public string? Image { get; }
        public DateTime DateTime { get; }
        public bool IsLiked { get; }
        public int LikesCount { get; }
        public UserHeaderDto Author { get; }
        public bool IsPublic { get; }
    }
}
