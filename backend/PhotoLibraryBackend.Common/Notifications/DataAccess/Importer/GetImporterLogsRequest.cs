using MediatR;

namespace PhotoLibraryBackend.Common;

public record class GetImporterLogsRequest(int PageSize): IRequest<ImporterReport[]?>;
