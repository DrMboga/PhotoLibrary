using MediatR;

namespace PhotoLibraryBackend.Common;

public record class GetMimeTypeRequest(string extension): IRequest<string>;
