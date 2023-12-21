namespace TestWithEF.Exceptions;

public class EmailException: Exception
{
    public static Exception InValidEmailException()
    {
        return new Exception("Invalid Email");
    }
}
