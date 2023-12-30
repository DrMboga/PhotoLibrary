using MediatR;

namespace PhotoLibraryBackend.Common;

public record SaveNewFolderInfoRequest(string FullPath, string Name, long? ParentId): IRequest<FolderInfo>;
