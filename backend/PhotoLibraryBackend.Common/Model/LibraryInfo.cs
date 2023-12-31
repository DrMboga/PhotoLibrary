namespace PhotoLibraryBackend.Common;

public record class LibraryInfo(long MediaFilesCount, DateTime? DateOfEarliestPhoto, DateTime? DateOfNewestPhoto);