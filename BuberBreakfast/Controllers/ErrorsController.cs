using Microsoft.AspNetCore.Mvc;

namespace BuberBreakfast.Controllers;

// Error handling Logic
public class ErrorController : ControllerBase
{
    [Route("/error")]

    public IActionResult Error(){
        return Problem();
    }
}