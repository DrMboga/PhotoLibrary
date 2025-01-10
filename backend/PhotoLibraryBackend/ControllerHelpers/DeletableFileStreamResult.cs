using System;
using Microsoft.AspNetCore.Mvc;

namespace PhotoLibraryBackend.ControllerHelpers;

public class DeletableFileStreamResult : FileStreamResult
{
    readonly string? _originalFileFullPath;
    public DeletableFileStreamResult(Stream fileStream, string contentType, string? fileDownloadName, string? originalFilePath) : base(fileStream, contentType)
    {
        FileDownloadName = fileDownloadName;
        EnableRangeProcessing = true;
        _originalFileFullPath = originalFilePath;
    }

    public override async Task ExecuteResultAsync(ActionContext context)
    {
        await base.ExecuteResultAsync(context);

        // if(_originalFileFullPath != null && File.Exists(_originalFileFullPath))
        // {
        //     File.Delete(_originalFileFullPath);
        // }
    }
}
