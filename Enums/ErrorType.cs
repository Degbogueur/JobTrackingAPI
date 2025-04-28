namespace JobTrackingAPI.Enums;

public enum ErrorType
{
    None,
    NotFound,
    Conflict,
    ValidationError,
    BadRequest,
    FileUploadError,
    Unauthorized,
    Forbidden,
    InternalServerError
}
