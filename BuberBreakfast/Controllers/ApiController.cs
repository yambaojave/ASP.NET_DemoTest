using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BuberBreakfast.Controllers;

[ApiController]
// [Route] - this is the starting point of each HTTP Request
// [Route("breakfasts")] or 
[Route("[controller]")] // remove the word controller, starting point will be breakfasts
public class ApiController : ControllerBase {
    protected IActionResult Problem (List<Error> errors)
    {
        if (errors.All(e => e.Type == ErrorType.Validation)){
            var modelStateDictionary = new ModelStateDictionary();

            foreach(var error in errors){
                modelStateDictionary.AddModelError(error.Code, error.Description);
            }
            return ValidationProblem(modelStateDictionary);
        }

        if(errors.Any(e => e.Type == ErrorType.Unexpected)){
            return Problem();
        }
        var FirstError = errors[0];
        
        var statusCode = FirstError.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        return Problem(statusCode: statusCode, title: FirstError.Description);
    }

    
}