namespace PhotoLibraryBackend.Common;

public class UnknownFileFormatException: ApplicationException
{
    public string UnknownFileExtension { get; }

    public UnknownFileFormatException(string extension): base($"Unknown file extension '{extension}'")
    {
        UnknownFileExtension = extension;
    }
}
