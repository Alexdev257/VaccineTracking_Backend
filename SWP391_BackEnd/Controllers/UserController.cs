using ClassLib.DTO.User;
using ClassLib.Models;
using ClassLib.Service;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("getAllUser")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _userService.getAllService();
        }

        [HttpGet("getUserById")]
        //[Authorize]
        public async Task<ActionResult> GetUserById(int? id)
        {
            try
            {
                var userRes = await _userService.getUserByIdService(id);
                return Ok(new { msg = "Get user successfully", user = userRes });
            }
            catch (UnauthorizedAccessException e)
            {
                return NotFound(new { error = e.Message });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> register([FromBody] RegisterRequest registerRequest)
        {
            try
            {
                var registerResponse = await _userService.registerAsync(registerRequest);
                return Ok(new { msg = "Register successfully", user = registerResponse });
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = e.Message });
            }

        }

        [HttpPost("login-by-account")]
        public async Task<IActionResult> login([FromBody] LoginRequest loginRequest) //IActionResult
        {
            try
            {
                var LoginResponse = await _userService.loginAsync(loginRequest);
                return Ok(new { msg = "Login successfully", loginResponse = LoginResponse });
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

        //[HttpPost("refresh")]
        //public async Task<IActionResult> refresh([FromBody] LoginResponse refreshRequest)
        //{
        //    try
        //    {
        //        var loginResponse = await _userService.refreshTokenAsync(refreshRequest);
        //        return Ok(new { msg = "Refresh token successfully", loginResponse = loginResponse });
        //    }
        //    catch (UnauthorizedAccessException e)
        //    {
        //        return NotFound(new { error = e.Message });
        //    }
        //    catch (Exception e)
        //    {
        //        return StatusCode(500, new { error = "unauthenticated error", details = e });
        //    }
        //}

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] LoginResponse refreshRequest)
        {
            try
            {
                var loginResponse = await _userService.RefreshTokenService(refreshRequest);
                return Ok(new { msg = "Token refreshed successfully", loginResponse });
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(new { error = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = "Internal server error", details = e.Message });
            }
        }

        [HttpGet("get-refresh-token")]
        public async Task<IActionResult> GetRefreshToken(int? userId)
        {
            try
            {
                var refreshToken = await _userService.getRefreshTokenByUserIdService(userId);
                return Ok(new { msg = "Get refresh token successfully", refreshToken });
            }
            catch (UnauthorizedAccessException e)
            {
                return NotFound(new { error = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = "Internal server error", details = e.Message });
            }
        }

    }
}
