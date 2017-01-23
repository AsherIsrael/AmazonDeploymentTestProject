using System;
using System.ComponentModel.DataAnnotations;

namespace QuotingDojoRedux.Models
{
    public class Quote : BaseEntity
    {
        [Key]
        public int QuoteId { get; set; }

        public int UserId { get; set; }

        public User Quoter { get; set; }

        [Required]
        public string QuoteContent { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}