using MediatR;
using PhotoLibraryBackend.Common.Messages;

namespace PhotoLibraryBackend.Common;

public record class GetMediasByLabelRequest(long DateFrom, long DateTo, string LabelName): IRequest<MediaInfo[]>;
