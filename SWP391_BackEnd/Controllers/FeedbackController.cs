using ClassLib.DTO.Feedback;
using ClassLib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SWP391_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly FeedbackService _feedbackService;

        public FeedbackController(FeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpGet("get-all-feedback")]
        public async Task<IActionResult> getAllFeedback()
        {
            try
            {
                var rs = await _feedbackService.GetAllFeedback();
                return Ok(rs);
            }
            catch(ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-feedback-by-id/{id}")]
        public async Task<IActionResult> getFeedbackById(int id)
        {
            try
            {
                var rs = await _feedbackService.GetFeedbackById(id);
                return Ok(rs);
            }
            catch(ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-feedback-by-user-id/{userId}")]
        public async Task<IActionResult> getFeedbackByUserId(int userId)
        {
            try
            {
                var rs = await _feedbackService.GetFeedbackByUserId(userId);
                return Ok(rs);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-all-feedback-admin")]
        public async Task<IActionResult> getAllFeedbackAdmin()
        {
            try
            {
                var rs = await _feedbackService.GetAllFeedbackAdmin();
                return Ok(rs);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-feedback-by-id-admin/{id}")]
        public async Task<IActionResult> getFeedbackByIdAdmin(int id)
        {
            try
            {
                var rs = await _feedbackService.GetFeedbackByIdAdmin(id);
                return Ok(rs);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-feedback-by-user-id-admin/{userId}")]
        public async Task<IActionResult> getFeedbackByUserIdAdmin(int userId)
        {
            try
            {
                var rs = await _feedbackService.GetFeedbackByUserIdAdmin(userId);
                return Ok(rs);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create-feedback")]
        public async Task<IActionResult> CreateFeedback([FromBody] CreateFeedbackRequest request)
        {
            try
            {
                var rs = await _feedbackService.CreateFeedback(request);
                return Ok("Create successfully");
            }
            catch(ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update-feedback/{id}")]
        public async Task<IActionResult> UpdateFeedback(int id, [FromBody] UpdateFeedbackRequest request)
        {
            try
            {
                var rs = await _feedbackService.UpdateFeedback(id, request);
                return Ok("Update feedback successful");
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("hard-delete-feedback/{id}")]
        public async Task<IActionResult> HardDeleteFeedBack(int id)
        {
            try
            {
                var rs = await _feedbackService.HardDeleteFeedBack(id);
                return Ok("Delete successfully");
            }
            catch(ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPatch("soft-delete-feedback/{id}")]
        public async Task<IActionResult> SoftDeleteFeedback(int id)
        {
            try
            {
                var rs = await _feedbackService.SoftDeleteFeedback(id);
                return Ok("Delete successfully");
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
