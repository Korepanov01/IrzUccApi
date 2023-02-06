﻿using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Requests.Users
{
    public class UpdateRegInfoRequest
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        [Required(AllowEmptyStrings = false)]
        [MaxLength(50)]
        public string Surname { get; set; } = string.Empty;
        [MaxLength(50)]
        public string? Patronymic { get; set; }
        [Required]
        public DateTime Birthday { get; set; }
    }
}
