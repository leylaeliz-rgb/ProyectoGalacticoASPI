using ProyectoGalactico.Dtos.Responses;
namespace ProyectoGalactico.Common
{
    public enum ResultStatus { Ok, NotFound, Invalid, Conflict}
    public record ServiceResult<T>(ResultStatus Status, T? Value, string? Error)
    {
        public static ServiceResult<T> Ok(T value) => new(ResultStatus.Ok, value, null);
        public static ServiceResult<T> NotFound(string error) => new(ResultStatus.NotFound, default, error);
        public static ServiceResult<T> Invalid(string error) => new(ResultStatus.Invalid, default, error);
    }
    public static class ServiceResultExtensions
    {
            public static IResult ToHttp<T>(this ServiceResult<T> r) => r.Status switch
        {
            ResultStatus.Ok => Results.Ok(r.Value),
            ResultStatus.NotFound => Results.NotFound(new ErrorResponse(r.Error!)),
            _ => Results.BadRequest(new ErrorResponse(r.Error!))
        };
    }
    }
