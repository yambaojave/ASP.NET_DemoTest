using ErrorOr;
using BuberBreakfast.Models;
using BuberBreakFast.Contracts.Breakfast;

namespace BuberBreakfast.Services.Breakfasts;

public interface IBreakfastService
{
    ErrorOr<Created> CreateBreakfast(Breakfast breakfast);
    ErrorOr<Breakfast> GetBreakfast(Guid id);

    ErrorOr<Deleted> DeleteBreakfast(Guid id);
    ErrorOr<UpsertBreakfast> UpsertBreakfast(Breakfast breakfast);
}