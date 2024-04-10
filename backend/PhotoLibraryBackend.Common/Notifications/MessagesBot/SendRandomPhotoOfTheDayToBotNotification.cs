using MediatR;

namespace PhotoLibraryBackend.Common;

public record SendRandomPhotoOfTheDayToBotNotification(int Month, int Day): INotification;
