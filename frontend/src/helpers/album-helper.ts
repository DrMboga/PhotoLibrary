const favoriteAlbumName = 'Favorite';
const importantAlbumName = 'Important';
const printAlbumName = 'ToPrint';

export const checkIsFavorite = (albumName?: string) =>
  albumName?.includes(favoriteAlbumName) ?? false;
export const checkIsImportant = (albumName?: string) =>
  albumName?.includes(importantAlbumName) ?? false;
export const checkIsPrint = (albumName?: string) => albumName?.includes(printAlbumName) ?? false;

export const buildAlbumName = (isFavorite?: boolean, isImportant?: boolean, isPrint?: boolean) => {
  let album = '';
  if (isFavorite) {
    album = `${album}${favoriteAlbumName};`;
  }
  if (isImportant) {
    album = `${album}${importantAlbumName};`;
  }
  if (isPrint) {
    album = `${album}${printAlbumName};`;
  }

  return album;
};
