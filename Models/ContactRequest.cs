using System.ComponentModel.DataAnnotations;

namespace PortfolioBackend.Api.Models
{
    public class ContactRequest
    {
        [Required, MinLength(2)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(5)]
        public string Subject { get; set; } = string.Empty;

        [Required, MinLength(10)]
        public string Message { get; set; } = string.Empty;
    }
}
