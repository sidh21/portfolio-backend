using Microsoft.AspNetCore.Mvc;
using PortfolioBackend.Api.Models;
using PortfolioBackend.Api.Services;

namespace PortfolioBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly ILogger<ContactController> _logger;

        public ContactController(EmailService emailService, ILogger<ContactController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SubmitContact([FromBody] ContactRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation("Attempting to send contact form email from {Email}", request.Email);

            var success = await _emailService.SendContactFormEmailAsync(request);


            if (success)
                return Ok(new { message = "Message received and sent successfully!" });

            _logger.LogError("Failed to send contact email for subject: {Subject}", request.Subject);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Failed to send message. Please check server logs and email configuration." });
        }
    }
}
