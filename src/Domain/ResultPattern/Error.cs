namespace Domain.ResultPattern;

public enum ErrorType { NotFound, Conflict }

public record Error(string Code, ErrorType Type, string Description)
{
    public static Error NotFound(string Code) => new(code, ErrorType.NotFound, "Not Found");
}

