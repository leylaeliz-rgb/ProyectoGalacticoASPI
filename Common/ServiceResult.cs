using Microsoft.AspNetCore.Mvc;
using ProyectoGalactico.Dtos.Responses;

namespace ProyectoGalactico.Common;

public enum ResultStatus { Ok, NotFound, Invalid, Conflict, Forbidden }

public record ServiceResult<T>(ResultStatus Status, T? Value, string? Error)
{
    public static ServiceResult<T> Ok(T value) => new(ResultStatus.Ok, value, null);
    public static ServiceResult<T> NotFound(string error) => new(ResultStatus.NotFound, default, error);
    public static ServiceResult<T> Invalid(string error) => new(ResultStatus.Invalid, default, error);
    public static ServiceResult<T> Conflict(string error) => new(ResultStatus.Conflict, default, error);
    public static ServiceResult<T> Forbidden(string error) => new(ResultStatus.Forbidden, default, error);

}

public static class ServiceResultExtensions
{
    public static IActionResult ToActionResult<T>(this ServiceResult<T> result, ControllerBase controller) =>
        result.Status switch
        {
            ResultStatus.Ok => controller.Ok(result.Value),
            ResultStatus.NotFound => controller.NotFound(new ErrorResponse(result.Error!)),
            ResultStatus.Conflict => controller.Conflict(new ErrorResponse(result.Error!)),
            _ => controller.BadRequest(new ErrorResponse(result.Error!))
        };
}