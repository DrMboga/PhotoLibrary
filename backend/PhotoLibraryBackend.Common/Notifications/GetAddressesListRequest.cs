using MediatR;

namespace PhotoLibraryBackend.Common;

public record GetAddressesListRequest(int TopRows, bool? ReadDateIsEmpty): IRequest<MediaAddress[]>;
