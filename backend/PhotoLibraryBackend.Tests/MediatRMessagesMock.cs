namespace PhotoLibraryBackend.Tests;

public static class MediatRMessagesMock
{
    public static LibraryInfo LibraryInfoRequestMock ()
    {
        return new LibraryInfo(5, new DateTime(2000, 1, 1, 15, 0, 0), new DateTime(2024, 3, 14, 17, 29, 15));
    }
}
