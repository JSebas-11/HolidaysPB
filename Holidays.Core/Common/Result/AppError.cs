using HolidaysPB.Core.Common.Enums;

namespace HolidaysPB.Core.Common.Result;

public sealed record AppError(ErrorKind Kind, string Message) {
    public static AppError NotFound(string entity, object identifier) 
        => new(ErrorKind.NotFound, $"{entity} ({identifier}) was not found.");
    public static AppError NotFound(string message) => new(ErrorKind.NotFound, message);

    public static AppError Validation(string message) => new(ErrorKind.Validation, message);

    public static AppError Conflict(string message) => new(ErrorKind.Conflict, message);

    public static AppError Unauthorized(string message) => new(ErrorKind.Unauthorized, message);
    public static AppError Forbidden(string message) => new(ErrorKind.Forbidden, message);
    
    public static AppError Unexpected(string message) => new(ErrorKind.Unexpected, message);
}