﻿namespace IrzUccApi.Models.Dtos
{
    public record UserHeaderDto
    {
        public UserHeaderDto(
            Guid id,
            string firstName,
            string surname,
            string? patronymic,
            Guid? imageId)
        {
            Id = id;
            FirstName = firstName;
            Surname = surname;
            Patronymic = patronymic;
            ImageId = imageId;
        }

        public Guid Id { get; }
        public string FirstName { get; }
        public string Surname { get; }
        public string? Patronymic { get; }
        public Guid? ImageId { get; }
    }
}
