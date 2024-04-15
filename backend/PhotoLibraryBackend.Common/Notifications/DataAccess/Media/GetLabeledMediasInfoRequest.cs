using MediatR;

namespace PhotoLibraryBackend.Common;

public record GetLabeledMediasInfoRequest(): IRequest<(long labeledMediaCount, long totalMediaCount)>;
