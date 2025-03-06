using ClassLib.DTO.Child;
using ClassLib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SWP391_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChildController : ControllerBase
    {
        private readonly ChildService _childService;
        public ChildController(ChildService childService)
        {
            _childService = childService;
        }

        [HttpGet("get-all-child")]
        public async Task<IActionResult> getAllChild()
        {
            var child = await _childService.GetAllChildAsync();
            if(child == null)
            {
                return NotFound("No child in the system");
            }
            return Ok(child);
        }

        [HttpGet("get-all-child-admin")]
        public async Task<IActionResult> getAllChildAdmin()
        {
            var child = await _childService.GetAllChildForAdminAsync();
            if (child == null)
            {
                return NotFound("No child in the system");
            }
            return Ok(child);
        }

        [HttpGet("get-child-by-id/{id}")]
        public async Task<IActionResult> getChildById(int id)
        {
            try
            {
                var child = await _childService.GetChildByIdAsync(id);
                return Ok(child);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet("get-child-by-parents-id/{parentsId}")]
        public async Task<IActionResult> getChildByParentsId(int parentsId)
        {
            try
            {
                var child = await _childService.GetAllChildByParentIdAsync(parentsId);
                return Ok(child);
            }catch(ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost("create-child")]
        public async Task<IActionResult> createChild([FromBody] CreateChildRequest request)
        {
            try
            {
                var child = await _childService.CreateChildAsync(request);
                return Ok("Create child successfully");
            }catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("update-child/{id}")]
        public async Task<IActionResult> updateChild(int id, [FromBody] UpdateChildRequest request)
        {
            try
            {
                var child = await _childService.UpdateChildAsync(id, request);
                return Ok("Updated successfully");
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpDelete("delete-child/{id}")]
        public async Task<IActionResult> deleteChild(int id)
        {
            try
            {
                var result = await _childService.HardDeleteChildAsync(id);
                return Ok("Deleted successfully");
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPatch("soft-delete-child/{id}")]
        public async Task<IActionResult> softDeleteChild(int id)
        {
            try
            {
                var result = await _childService.SoftDeleteChildAsync(id);
                return Ok("Deleted successfully");
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
