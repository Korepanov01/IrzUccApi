﻿namespace IrzUccApi.Models.Dtos
{
    public record EventListItemDto
    {
        public EventListItemDto(
            Guid id,
            string title,
            DateTime start,
            DateTime end,
            string? cabinetName)
        {
            Id = id;
            Title = title;
            Start = start;
            End = end;
            CabinetName = cabinetName;
        }

        public Guid Id { get; }
        public string Title { get; }
        public DateTime Start { get; }
        public DateTime End { get; }
        public string? CabinetName { get; }
    }
}
