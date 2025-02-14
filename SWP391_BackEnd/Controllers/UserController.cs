using ClassLib.DTO.Request;
using ClassLib.Models;
using ClassLib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SWP391_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        // Inject UserService vào Controller
        public UserController(UserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _userService.getAllService();
        }

        [HttpPost("login")]
        public async Task<IActionResult> login([FromBody] LoginRequest loginRequest) //IActionResult
        {
            try
            {
                var user = _userService.loginAsync(loginRequest);
                return Ok(new { msg = "Login successfully", user = user });
            }
            catch (ArgumentException e)
            {
                return BadRequest(new { error = e.Message });
            }
            catch (UnauthorizedAccessException e)
            {
                return NotFound(new { error = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = "unauthenticated error", details = e });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> register([FromBody] RegisterRequest registerRequest)
        {
            try
            {
                var user = _userService.registerAsync(registerRequest);
                return Ok(new { msg = "Login successfully", user = user });
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Exist username" });
            }

        }
    }
}
//fewfewfewfewfewf
