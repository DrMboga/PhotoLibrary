namespace PhotoLibraryBackend.Common;

public class ShellExecuteException: Exception
{
    public int ErrorCode { get; }

    public ShellExecuteException(int errorCode, string message): base(message) 
    {
        ErrorCode = errorCode;
    }
}
