using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public abstract class BaseController : Controller
    {
        public IActionResult CreateOkResponse<T>(T value)
        {
            var result = new ResponseBase<T>(value);
            result.Success = true;

            return Ok(result);
        }

        public IActionResult CreateNotFoundResponse<T>()
        {
            var result = new ResponseBase<T>();
            result.Success = false;
            result.Errors = new List<string> { "Resourse not found" };
            return NotFound(result);
        }
    }
}