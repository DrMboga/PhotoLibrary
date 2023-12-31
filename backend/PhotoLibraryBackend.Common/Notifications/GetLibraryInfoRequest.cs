using MediatR;

namespace PhotoLibraryBackend.Common;

public record class GetLibraryInfoRequest(): IRequest<LibraryInfo?>;
