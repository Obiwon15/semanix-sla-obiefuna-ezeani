namespace FormsService.Domain.Exceptions;

public class InvalidTransitionException : DomainException
{
    public InvalidTransitionException(string message) : base(message) { }
}