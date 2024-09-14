using Microsoft.AspNetCore.Mvc;

namespace IMS.Presentation.Controllers
{
    [Route("")]
    [ApiController]
    public class TestControllers : ControllerBase
    {
        [HttpGet("")]
        public IActionResult RedirectToTestAPI()
        {
            return RedirectToAction(nameof(TestAPI));
        }

        [HttpGet("api/test")]
        public async Task<IActionResult> TestAPI()
        {
            try
            {
                var welcomeText = new
                {
                    message = "Welcome to the Test API",
                    status = "success",
                    system = new
                    {
                        version = "1.0.0",
                        name = "Inventory Management System for Computer Labourataries",
                        description = "This is a test API for the Inventory Management System for Computer Labourataries",
                        author = "Group 22",
                    },
                };
                return Ok(welcomeText);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
