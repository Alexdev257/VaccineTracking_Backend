using Azure.Core;
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

        [HttpGet("getUserById/{id}")]
        //[Authorize]
        public async Task<ActionResult> GetUserById(int? id)
        {
            try
            {
                //if (!int.TryParse(HttpContext.GetRouteValue("id")?.ToString(), out int id))
                //{
                //    return BadRequest(new { message = "Invalid ID format" });
                //}
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

        [HttpGet("get-refresh-token/{userId}")]
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

        /*[HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp(string request)
        {
            if (string.IsNullOrEmpty(request))
                return BadRequest("Số điện thoại không được để trống");

            var verificationId = await _userService.sendOtpAsync(request);
            return Ok(new { VerificationId = verificationId });
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return BadRequest("Số điện thoại không được để trống");
            try
            {
                var sessionInfo = await _userService.SendOtpAsync(phoneNumber);
                return Ok(new { sessionInfo });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> verifyOtp([FromBody] VerifyOtpRequest request)
        {
            if (string.IsNullOrEmpty(request.PhoneNumber) || string.IsNullOrEmpty(request.IdToken))
                return BadRequest("Số điện thoại và mã OTP không được để trống");
            try
            {
                var loginResponse = await _userService.VerifyOtpAsync(request);
                return Ok(new { msg = "OTP verified successfully", loginResponse });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }*/

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp(string phoneNumber)
        {

            if (string.IsNullOrEmpty(phoneNumber))
                return BadRequest("Số điện thoại không được để trống");

            try
            {
                var sessionInfo = await _userService.SendOtpAsync(phoneNumber);
                return Ok(new { sessionInfo });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            if (string.IsNullOrEmpty(request.SessionInfo) || string.IsNullOrEmpty(request.Otp))
                return BadRequest("SessionInfo và OTP không được để trống");

            try
            {
                var idToken = await _userService.VerifyOtpAsync(request.SessionInfo, request.Otp);
                return Ok(new { msg = "OTP verified successfully", idToken });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpPut("update-user/{id}")]
        public async Task<IActionResult> updateUserController(int id, [FromBody] UpdateUserRequest request)
        {
            var rs = await _userService.updateUserAsync(id, request);
            if (!rs)
            {
                return BadRequest(new { message = "Update user failed" });
            }
            return Ok(new { message = "Update user successfully" });
        }

        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> deleteUserController(int id)
        {
            var rs = await _userService.deleteUserAsync(id);
            if (!rs)
            {
                return BadRequest(new { message = "Delete user failed" });
            }
            return Ok(new { message = "Delete user successfully" });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> changePasswordController([FromBody] ForgotPasswordRequest request)
        {
            var rs = await _userService.forgotPasswordAsync(request);
            if (!rs)
            {
                return BadRequest(new { message = "Change password failed" });
            }
            return Ok(new { message = "Send verify code successfully" });
        }

        [HttpPost("verify-forgot-password")]
        public async Task<IActionResult> verifyForgotPasswordCode([FromBody] VerifyForgotPasswordRequest request)
        {
             if(string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.VerifyCode))
            {
                return BadRequest("Phone number or verify code are not be blank");
            }
            var rs = await _userService.verifyForgotPasswordCodeAsync(request);
            if (!rs)
            {
                return BadRequest("Invalid or expired OTP");
            }

            return Ok("Change password successfully");

        }
    }
}
