using System;
using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    public abstract class DBdocument
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime LastChangedAt { get; set; }
    }
}
