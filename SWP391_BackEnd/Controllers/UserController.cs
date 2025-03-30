using ClassLib.DTO.User;
using ClassLib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

        [HttpGet("get-all-user")]
        [Authorize(Policy = "StaffOrUser")]
        public async Task<ActionResult<List<GetUserResponse>>> GetUser()
        {
            return await _userService.getAllService();
        }
        [HttpGet("get-all-user-admin")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<List<GetUserResponse>>> GetUserAdmin()
        {
            return await _userService.getAllServiceAdmin();
        }

        [HttpGet("get-user-by-id/{id}")]
        [Authorize(Policy = "UserOnly")]
        public async Task<ActionResult> GetUserById(int id)
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

        [HttpGet("get-user-by-id-admin/{id}")]
        //[Authorize]
        public async Task<ActionResult> GetUserByIdAdmin(int id)
        {
            try
            {
                var userRes = await _userService.getUserByIdServiceAdmin(id);
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
                return Ok("Send otp successfully");
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = e.Message });
            }

        }

        [HttpPost("verify-register")]
        public async Task<IActionResult> verifyRegister([FromBody] VerifyRegisterRequest request)
        {
            try
            {
                var res = await _userService.verifyCodeRegisterAsync(request);
                return Ok(new { msg = "Register successfully", user = res });
            }
            catch(ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch(UnauthorizedAccessException e)
            {
                return Unauthorized(e.Message);
            }
            catch(DbUpdateException e)
            {
                return StatusCode(500, e.Message);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
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

        [HttpPost("login-by-google")]
        public async Task<IActionResult> loginByGoogle([FromBody] LoginGoogleRequest request)
        {
            try
            {
                var res = await _userService.loginByGoogleAsync(request.GoogleToken, request.ClientID);
                return Ok(new { msg = "Login Successfully", res = res });
            }
            catch(UnauthorizedAccessException e)
            {
                return BadRequest(e.Message);
            }
            catch(Exception e)
            {
                return StatusCode(500, new {message = "Something went wrong"});
            }
        }

        [HttpPost("login-by-phone")]
        public async Task<IActionResult> loginByPhone([FromBody] LoginPhoneRequest request)
        {
            try
            {
                var res = await _userService.loginByPhoneAsync(request.PhoneNumber);
                return Ok(new { msg = "Send otp successfully" });
            }
            catch(ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch(Exception e)
            {
                return NotFound("Do not exist user");
            }
        }

        [HttpPost("verify-otp-login")]
        public async Task<IActionResult> verifyOtpLogin([FromBody] VerifyOtpLoginPhoneRequest request)
        {
            try
            {
                var res = await _userService.verifyOtpForLoginAsync(request.PhoneNumber, request.OTP);
                return Ok(new { msg = "login Successfully", res = res });
            }
            catch(UnauthorizedAccessException e)
            {
                return BadRequest(e.Message);
            }
            catch(ArgumentNullException e)
            {
                return NotFound(e.Message);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("refresh")]
        [Authorize(Policy = "AllRole")]
        public async Task<IActionResult> RefreshToken([FromBody] LoginResponse refreshRequest)
        {
            try
            {
                var loginResponse = await _userService.RefreshTokenService(refreshRequest);
                return Ok(new { msg = "Token refreshed successfully", loginResponse });
            }
            catch(SecurityTokenInvalidAlgorithmException e)
            {
                return BadRequest(e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(new { error = e.Message });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("get-refresh-token/{userId}")]
        [Authorize(Policy = "AllRole")]
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
        }*/

        [HttpPut("update-user/{id}")]
        public async Task<IActionResult> updateUserController(int id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                var rs = await _userService.updateUserAsync(id, request);
                if (!rs)
                {
                    return BadRequest(new { message = "Update user failed" });
                }
                return Ok(new { message = "Update user successfully" });
            }
            catch (ArgumentNullException e)
            {
                return NotFound(new { message = e.Message });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
            
        }

        [HttpPut("update-user-admin/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> updateUserAdminController(int id, [FromBody] UpdateUserAdminRequest request)
        {
            try
            {
                var rs = await _userService.UpdateUserAdmin(id, request);
                if (!rs)
                {
                    return BadRequest(new { message = "Update user failed" });
                }
                return Ok(new { message = "Update user successfully" });
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
            catch (ArgumentException e)
            {
                return NotFound(new { message = e.Message });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }

        }

        [HttpGet("get-user-child/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> getThem(int id)
        {
            try
            {
                var rs = await _userService.getUserChildByIdAdmin(id);
                return Ok(rs);
            }
            catch(ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch(ArgumentException e)
            {
                return NotFound(e.Message);
            }
            catch(UnauthorizedAccessException e)
            {
                return BadRequest(e.Message);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("get-all-user-child")]
        public async Task<IActionResult> getAllThem()
        {
            try
            {
                var rs = await _userService.getAllUserChildAdmin();
                return Ok(rs);
            }
            catch(ArgumentException e)
            {
                return NotFound(e.Message);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> deleteUserController(int id)
        {
            try
            {
                var rs = await _userService.deleteUserAsync(id);
                if (!rs)
                {
                    return BadRequest(new { message = "Delete user failed" });
                }
                return Ok(new { message = "Delete user successfully" });
            }
            catch (ArgumentNullException e)
            {
                return NotFound(new { message = e.Message });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> changePasswordController([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                var rs = await _userService.forgotPasswordAsync(request);
                return Ok(new { message = "Send verify code successfully" });
            }
            catch (Exception e)
            {
                return NotFound(new { message = e.Message });
            }
            
        }

            [HttpPost("verify-forgot-password")]
        public async Task<IActionResult> verifyForgotPasswordCode([FromBody] VerifyForgotPasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.VerifyCode))
            {
                return BadRequest("Verify code are not be blank");
            }
            var rs = await _userService.verifyForgotPasswordCodeAsync(request);
            if (!rs)
            {
                return BadRequest("Invalid or expired OTP");
            }

            return Ok("Verify successfully");

        }

        [HttpPost("change-password")]
        public async Task<IActionResult> changePasswordController([FromBody] ChangePasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest("New password are not be blank");
            }
            var rs = await _userService.changePasswordAsync(request);
            if (!rs)
            {
                return BadRequest("Change password failed");
            }
            return Ok("Change password successfully");
        }

        [HttpPatch("disable-user/{id}")]
        public async Task<IActionResult> disableUserController(int id)
        {
            try
            {
                var rs = await _userService.disableUserAsync(id);
                if (!rs)
                {
                    return BadRequest(new { message = "Disable user failed" });
                }
                return Ok(new { message = "Disable user successfully" });
            }
            catch (ArgumentNullException e)
            {
                return NotFound(new { message = e.Message });
            }
            catch(InvalidOperationException e)
            {
                return BadRequest(new { message = e.Message });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPost("create-user")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateUser([FromBody] CreateStaffRequest request)
        {
            try
            {
                var rs = await _userService.CreateUser(request);
                return Ok("Create user successful");
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (ArgumentException e)
            {
                return Conflict(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPost("create-staff")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffRequest request)
        {
            try
            {
                var rs = await _userService.CreateStaff(request);
                return Ok("Create staff successful");
            }
            catch(ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch(ArgumentException e)
            {
                return Conflict(e.Message);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPatch("soft-delete-user/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> SoftDeleteUser(int id)
        {
            try
            {
                var rs = await _userService.SoftDeleteUser(id);
                return Ok("Delete successfully");
            }
            catch(ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch(ArgumentException e)
            {
                return NotFound(e.Message);

            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
