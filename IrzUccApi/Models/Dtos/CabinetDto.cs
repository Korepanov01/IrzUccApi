﻿namespace IrzUccApi.Models.Dtos
{
    public record CabinetDto
    {
        public CabinetDto(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; }
        public string Name { get; }
    }
}
