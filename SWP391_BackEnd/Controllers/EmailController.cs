using ClassLib.DTO.Email;
using ClassLib.Service;
using Microsoft.AspNetCore.Mvc;

namespace SWP391_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly IWebHostEnvironment _env;

        public EmailController(EmailService emailService, IWebHostEnvironment env)
        {
            _emailService = emailService;
            _env = env;
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> sendEmail([FromBody] EmailRequest emailRequest)
        {
            if (string.IsNullOrEmpty(emailRequest.ToEmail) || string.IsNullOrEmpty(emailRequest.Subject))
            {
                return BadRequest(new { message = "Email Information is missing!" });
            }

            //string templatePath = Path.Combine(_env.WebRootPath, "templates", "emailTemplate.html");
            string templatePath = Path.Combine(_env.WebRootPath, "templates", "newEmailTemplate.html");

            var placeholders = new Dictionary<string, string>
            {
                { "UserName", emailRequest.UserName },
                { "VerifyCode", emailRequest.VerifyCode }
            };

            bool rs = await _emailService.sendEmailService(emailRequest.ToEmail, emailRequest.Subject, templatePath, placeholders);

            if (rs)
            {
                return Ok(new { message = "Email sent successfully!" });
            }
            else
            {
                return BadRequest(new { message = "Email sent failed!" });
            }
        }
    }
}
