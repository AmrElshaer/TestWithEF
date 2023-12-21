namespace TestWithEF.Exceptions;

public class UserException: Exception
{
    private UserException(string error):base(error)
    {
        
    }
    public static UserException UserWithTheSameNameAlreadyExists(string name)
    {
        return new UserException($"User with the same name already exists: {name}");
    }
}
