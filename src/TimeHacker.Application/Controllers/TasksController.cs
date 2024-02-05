using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TimeHacker.Application.Controllers
{
    [Authorize]
    public class TasksController : ControllerBase
    {
        public TasksController()
        {
            
        }

        [HttpGet]
        public IActionResult GetTasks()
        {
            return Ok();
        }
    }
}
