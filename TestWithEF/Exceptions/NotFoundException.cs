namespace TestWithEF.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException()
        : base() { }

    public NotFoundException(string message)
        : base(message) { }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException) { }

    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.") { }

    public static Exception ForAuthor(Guid authorId)
    {
        return new NotFoundException($"Author with Id {authorId} was not found.");
    }
}
