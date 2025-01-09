using MediatR;

namespace PhotoLibraryBackend.Common;

public record class ConvertHeicImageNotification(string HeicFileFullPath, string JpegFileFullPath): INotification;
