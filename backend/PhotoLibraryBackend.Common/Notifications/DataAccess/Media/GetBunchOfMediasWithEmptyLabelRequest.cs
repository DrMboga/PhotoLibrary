using MediatR;

namespace PhotoLibraryBackend.Common;

public record GetBunchOfMediasWithEmptyLabelRequest(int BunchSize): IRequest<(long MediaId, string FullFileName)[]>;

