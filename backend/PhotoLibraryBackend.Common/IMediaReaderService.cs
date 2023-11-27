﻿namespace PhotoLibraryBackend.Common;

/// <summary>
/// Service for reading media metadata and thumbnail creation
/// </summary>
public interface IMediaReaderService
{
    /// <summary>
    /// Makes a thumbnail from a photo.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    byte[]? MakePhotoThumbnail(string filePath);

    /// <summary>
    /// Makes a thumbnail from video file
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    Task<byte[]?> MakeVideoThumbnail(string filePath);
}