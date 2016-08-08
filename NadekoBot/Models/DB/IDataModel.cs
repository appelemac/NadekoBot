using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace NadekoBot.Models.DB
{
    public abstract class IDataModel
    {
        [Key]
        public int? Id { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;

    }
}