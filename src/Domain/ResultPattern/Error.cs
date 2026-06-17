namespace Domain.ResultPattern;

public enum ErrorType { NotFound, Conflict }

public record Error(string Code, ErrorType Type, string Description)                                                          
{                                                                                                                             
    public static Error NotFound(string code, string description) => new(code, ErrorType.NotFound, description);              
    public static Error Conflict(string code, string description) => new(code, ErrorType.Conflict, description);              
}  

