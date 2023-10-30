export interface MediaInfo {
  id: string; //uuid
  thumbnailUrl: string;
  fullPath: string;
  fileName: string;
  fileExtension: string;
  mediaType: 'image' | 'video';
  fileSizeKb: number;
  dateTimeOriginal: number;
  width: number;
  height: number;
  videoDurationSec?: number;
  tag?: string;
  latitude?: number;
  longitude?: number;
  country?: string;
  region?: string;
  locality?: string;
  address?: string;
  venue?: string;
}
