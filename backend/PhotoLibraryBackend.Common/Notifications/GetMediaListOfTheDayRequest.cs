using MediatR;
using PhotoLibraryBackend.Common.Messages;

namespace PhotoLibraryBackend.Common;

public record GetMediaListOfTheDayRequest(long Today): IRequest<MediaInfo[]>;
