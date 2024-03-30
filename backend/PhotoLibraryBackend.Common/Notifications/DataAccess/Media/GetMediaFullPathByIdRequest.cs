using MediatR;

namespace PhotoLibraryBackend.Common;

public record class GetMediaFullPathByIdRequest(long MediaId): IRequest<string>;
