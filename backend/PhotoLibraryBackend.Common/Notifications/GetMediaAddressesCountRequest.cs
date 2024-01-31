using MediatR;

namespace PhotoLibraryBackend.Common;

public record GetMediaAddressesCountRequest(bool EmptyAddresses): IRequest<int>;

