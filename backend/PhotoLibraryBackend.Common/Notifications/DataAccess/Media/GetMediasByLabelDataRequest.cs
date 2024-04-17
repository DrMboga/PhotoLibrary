using MediatR;

namespace PhotoLibraryBackend.Common;

public record class GetMediasByLabelDataRequest(DateTime DateFrom, DateTime DateTo, string LabelName): IRequest<MediaFileInfo[]>;
