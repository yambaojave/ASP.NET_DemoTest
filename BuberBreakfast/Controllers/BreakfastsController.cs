using BuberBreakfast.Models;
using BuberBreakFast.Contracts.Breakfast;
using BuberBreakfast.ServiceErrors;
using BuberBreakfast.Services.Breakfasts;
using Microsoft.AspNetCore.Mvc;
using ErrorOr;

namespace BuberBreakfast.Controllers;



public class BreakfastsController : ApiController
{

    private readonly IBreakfastService _breakfastService;

    public BreakfastsController(IBreakfastService breakfastService)
    {
        _breakfastService = breakfastService;
    }

    // old HttpPost("/breakfasts") - because we add [Route] we don't need to add a starting point everytime we create a new request
    [HttpPost]

    public IActionResult CreateBreakfast(CreateBreakfastRequest request)
    {
        ErrorOr<Breakfast> requestToBreakfastResult = Breakfast.From(request);

        if(requestToBreakfastResult.IsError){
            return Problem(requestToBreakfastResult.Errors);
        }

        var breakfast = requestToBreakfastResult.Value;

        // TODO: save breakfast to database
        ErrorOr<Created> createBreakfastResult = _breakfastService.CreateBreakfast(breakfast);


        // var response = new BreakfastResponse(
        //     breakfast.Id,
        //     breakfast.Name,
        //     breakfast.Description,
        //     breakfast.StartDateTime,
        //     breakfast.EndDateTime,
        //     breakfast.LastModifiedDateTime,
        //     breakfast.Savory,
        //     breakfast.Sweet
        // );

        // if (createBreakfastResult.IsError)
        // {
        //     return Problem(createBreakfastResult.Errors);
        // }
        // return CreatedAtGetBreakfast(breakfast);

        return createBreakfastResult.Match(
            created => CreatedAtGetBreakfast(breakfast),
            errors => Problem(errors)
        );
    }



    // old HttpGet("/breakfasts/{{id:guid}}")
    [HttpGet("{id:guid}")]

    public IActionResult GetBreakfast(Guid id)
    {
        ErrorOr<Breakfast> getBreakfastResult = _breakfastService.GetBreakfast(id);

        return getBreakfastResult.Match(
            breakfast => Ok(MapBreakfastResponse(breakfast)),
            errors => Problem(errors)
        );
        // if (getBreakfastResult.IsError && getBreakfastResult.FirstError == Errors.Breakfast.NotFound)
        // {
        //     return NotFound();
        // }

        // var breakfast = getBreakfastResult.Value;

        // BreakfastResponse response = MapBreakfastResponse(breakfast);

        // return Ok(response);
    }


    [HttpPut("{id:guid}")]

    public IActionResult UpsertBreakfast(Guid id, UpsertBreakfastRequest request)
    {
        ErrorOr<Breakfast> requestToBreakfastResult = Breakfast.Create(
            request.Name,
            request.Description,
            request.StartDateTime,
            request.EndDateTime,
            request.Savory,
            request.Sweet,
            id
        );

        if(requestToBreakfastResult.IsError){
            return Problem(requestToBreakfastResult.Errors);
        }

        var breakfast = requestToBreakfastResult.Value;
        ErrorOr<UpsertBreakfast> upsertBreakfastResult = _breakfastService.UpsertBreakfast(breakfast);

        // TODO: return 201 if a new breakfast was created
        return upsertBreakfastResult.Match(
            upserted => upserted.IsNewlyCreated ? CreatedAtGetBreakfast(breakfast) : NoContent(),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{id:guid}")]

    public IActionResult DeleteBreakfast(Guid id)
    {
        ErrorOr<Deleted> deletedBreakfastResult = _breakfastService.DeleteBreakfast(id);

        return deletedBreakfastResult.Match(
            deleted => NoContent(),
            errors => Problem(errors)
        );

    }

    private static BreakfastResponse MapBreakfastResponse(Breakfast breakfast)
    {
        return new BreakfastResponse(
            breakfast.Id,
            breakfast.Name,
            breakfast.Description,
            breakfast.StartDateTime,
            breakfast.EndDateTime,
            breakfast.LastModifiedDateTime,
            breakfast.Savory,
            breakfast.Sweet
        );
    }

        private CreatedAtActionResult CreatedAtGetBreakfast(Breakfast breakfast)
    {
        return CreatedAtAction(
            nameof(GetBreakfast),
            new { id = breakfast.Id },
            value: MapBreakfastResponse(breakfast));
    }
}