﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CardStorageService.Data;

namespace CardStorageService.Models
{
    public class ClientDto
    {
        public string? Surname { get; set; }
        public string? FirstName { get; set; }
        public string? Patronymic { get; set; }
        public virtual ICollection<CardDto> Cards { get; set; } = new HashSet<CardDto>();
    }
}