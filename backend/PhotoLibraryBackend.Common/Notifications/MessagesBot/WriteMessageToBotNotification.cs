using MediatR;

namespace PhotoLibraryBackend.Common;

public record WriteMessageToBotNotification(string MessageAsMarkdown): INotification;
