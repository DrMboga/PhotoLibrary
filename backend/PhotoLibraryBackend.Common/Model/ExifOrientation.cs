namespace PhotoLibraryBackend.Common;

    /*
1 = Horizontal (normal) - The 0th row is at the top of the visual image, and the 0th column is the visual left side.
2 = Mirror horizontal - The 0th row is at the visual top of the image, and the 0th column is the visual right side.
3 = Rotate 180 - The 0th row is at the visual bottom of the image, and the 0th column is the visual right side.
4 = Mirror vertical - The 0th row is at the visual bottom of the image, and the 0th column is the visual left side.
5 = Mirror horizontal and rotate 270 CW - The 0th row is the visual left side of the image, and the 0th column is the visual top.
6 = Rotate 90 CW - The 0th row is the visual right side of the image, and the 0th column is the visual top.
7 = Mirror horizontal and rotate 90 CW - The 0th row is the visual right side of the image, and the 0th column is the visual bottom.
8 = Rotate 270 CW - The 0th row is the visual left side of the image, and the 0th column is the visual bottom. 
    */
public enum ExifOrientation: short
{
    Horizontal = 1,
    MirrorHorizontal = 2,
    Rotate180 = 3,
    MirrorVertical = 4,
    MirrorHorizontalAndRotate270CW = 5,
    Rotate90CW = 6,
    MirrorHorizontalAndRotate90CW = 7,
    Rotate270CW = 8,
}
