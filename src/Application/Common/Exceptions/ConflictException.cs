namespace FocusPocuss.Application.Common.Exceptions;

public sealed class ConflictException : Exception
{
    public ConflictException() : base("The session changed. Reload its current state before trying again.") { }
}
