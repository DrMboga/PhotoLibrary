import { ImportStepReport, MediaInfo } from '../model/media-info';
import { backendUrl, checkStatus, parseJSON } from './ApiHelper';
import { MediaGeoLocationRegionsInfo } from '../model/media-geo-location-regions-info';
import { MediaGeoLocationRegionSummary } from '../model/media-geo-location-region-summary';

const backendAPI = {
  async aboutBackend(): Promise<string> {
    if (!backendUrl) {
      throw new Error('Please specify Backend URL in environment settings');
    }
    const response = await fetch(backendUrl);
    const responseWithStatus = checkStatus(response);
    return responseWithStatus.text();
  },
  async isImporterInProgress(authToken?: string): Promise<boolean> {
    if (!backendUrl) {
      throw new Error('Please specify Backend URL in environment settings');
    }
    if (!authToken) {
      throw new Error('Please login');
    }
    const response = await fetch(`${backendUrl}/worker/MediaImportStatus`, {
      method: 'get',
      headers: new Headers({
        Authorization: `Bearer ${authToken}`,
      }),
    });
    const responseWithStatus = checkStatus(response);
    const status = await responseWithStatus.text();
    return status.includes('InProgress');
  },
  async getImporterLogs(authToken?: string): Promise<ImportStepReport[]> {
    if (!backendUrl) {
      throw new Error('Please specify Backend URL in environment settings');
    }
    if (!authToken) {
      throw new Error('Please login');
    }
    const response = await fetch(`${backendUrl}/worker/ImporterLogs`, {
      method: 'get',
      headers: new Headers({
        Authorization: `Bearer ${authToken}`,
      }),
    });
    const responseWithStatus = checkStatus(response);
    return parseJSON(responseWithStatus);
  },
  async triggerMediaImport(authToken?: string): Promise<void> {
    if (!backendUrl) {
      throw new Error('Please specify Backend URL in environment settings');
    }
    if (!authToken) {
      throw new Error('Please login');
    }
    const response = await fetch(`${backendUrl}/worker/TriggerMediaImport`, {
      method: 'post',
      headers: new Headers({
        Authorization: `Bearer ${authToken}`,
      }),
    });
    checkStatus(response);
  },
  async triggerGeocodingCollect(requestLimit: number, authToken?: string): Promise<void> {
    if (!backendUrl) {
      throw new Error('Please specify Backend URL in environment settings');
    }
    if (!authToken) {
      throw new Error('Please login');
    }
    const response = await fetch(
      `${backendUrl}/worker/TriggerGeocodingDataCollect?requestsLimit=${requestLimit}`,
      {
        method: 'post',
        headers: new Headers({
          Authorization: `Bearer ${authToken}`,
        }),
      },
    );
    checkStatus(response);
  },
  async downloadMedia(
    fullPath: string,
    useConvertedVideo: boolean,
    authToken?: string,
  ): Promise<Blob> {
    if (!backendUrl) {
      throw new Error('Please specify Backend URL in environment settings');
    }
    if (!authToken) {
      throw new Error('Please login');
    }
    const response = await fetch(
      `${backendUrl}/media/MediaDownload?filePath=${fullPath}&useConvertedVideo=${useConvertedVideo}`,
      {
        method: 'get',
        headers: new Headers({
          Authorization: `Bearer ${authToken}`,
        }),
      },
    );
    const responseWithStatus = checkStatus(response);
    return responseWithStatus.blob();
  },
  async deleteMedia(mediaId: string, authToken?: string): Promise<void> {
    if (!backendUrl) {
      throw new Error('Please specify Backend URL in environment settings');
    }
    if (!authToken) {
      throw new Error('Please login');
    }
    const response = await fetch(`${backendUrl}/media/DeleteMedia?mediaId=${mediaId}`, {
      method: 'delete',
      headers: new Headers({
        Authorization: `Bearer ${authToken}`,
      }),
    });
    checkStatus(response);
  },
  async setMediaAlbumFlag(
    mediaId: string,
    favorite?: boolean,
    important?: boolean,
    toPrint?: boolean,
    authToken?: string,
  ): Promise<void> {
    if (!backendUrl) {
      throw new Error('Please specify Backend URL in environment settings');
    }
    if (!authToken) {
      throw new Error('Please login');
    }
    let url = `${backendUrl}/media/SetMediaAlbum?mediaId=${mediaId}`;
    if (favorite !== undefined) {
      url = `${url}&isFavorite=${favorite ? 'true' : 'false'}`;
    }
    if (important !== undefined) {
      url = `${url}&isImportant=${important ? 'true' : 'false'}`;
    }
    if (toPrint !== undefined) {
      url = `${url}&isToPrint=${toPrint ? 'true' : 'false'}`;
    }
    const response = await fetch(url, {
      method: 'put',
      headers: new Headers({
        Authorization: `Bearer ${authToken}`,
      }),
    });
    checkStatus(response);
  },
  async getMediasByAlbum(
    favorite?: boolean,
    important?: boolean,
    toPrint?: boolean,
    authToken?: string,
  ): Promise<MediaInfo[]> {
    if (!backendUrl) {
      throw new Error('Please specify Backend URL in environment settings');
    }
    if (!authToken) {
      throw new Error('Please login');
    }
    let url = `${backendUrl}/media/MediaByAlbum?`;
    if (favorite !== undefined && favorite) {
      url = `${url}isFavorite=true`;
    }
    if (important !== undefined && important) {
      url = `${url}isImportant=true`;
    }
    if (toPrint !== undefined && toPrint) {
      url = `${url}isToPrint=true`;
    }
    const response = await fetch(url, {
      method: 'get',
      headers: new Headers({
        Authorization: `Bearer ${authToken}`,
      }),
    });
    const responseWithStatus = checkStatus(response);
    const result = await parseJSON(responseWithStatus);
    return result ?? [];
  },
  async getGeocodingStatus(
    authToken?: string,
  ): Promise<{ emptyAddressesCount: number; filledAddressesCount: number }> {
    if (!backendUrl) {
      throw new Error('Please specify Backend URL in environment settings');
    }
    if (!authToken) {
      throw new Error('Please login');
    }
    const response = await fetch(`${backendUrl}/worker/GeocodingStatus?`, {
      method: 'get',
      headers: new Headers({
        Authorization: `Bearer ${authToken}`,
      }),
    });
    const responseWithStatus = checkStatus(response);
    const result = await parseJSON(responseWithStatus);
    return result ?? { emptyAddressesCount: 0, filledAddressesCount: 0 };
  },
  async getMediasOfTheDay(today: number, authToken?: string): Promise<MediaInfo[]> {
    if (!backendUrl) {
      throw new Error('Please specify Backend URL in environment settings');
    }
    if (!authToken) {
      throw new Error('Please login');
    }
    let url = `${backendUrl}/media/MediasOfTheDay?today=${today}`;
    const response = await fetch(url, {
      method: 'get',
      headers: new Headers({
        Authorization: `Bearer ${authToken}`,
      }),
    });
    const responseWithStatus = checkStatus(response);
    const result = await parseJSON(responseWithStatus);
    return result ?? [];
  },
  async getMediasByLabel(
    dateFrom: number,
    dateTo: number,
    label: string,
    authToken?: string,
  ): Promise<MediaInfo[]> {
    if (!backendUrl) {
      throw new Error('Please specify Backend URL in environment settings');
    }
    if (!authToken) {
      throw new Error('Please login');
    }
    let url = `${backendUrl}/media/MediasByLabel?dateFrom=${dateFrom}&dateTo=${dateTo}&label=${label}`;
    const response = await fetch(url, {
      method: 'get',
      headers: new Headers({
        Authorization: `Bearer ${authToken}`,
      }),
    });
    const responseWithStatus = checkStatus(response);
    const result = await parseJSON(responseWithStatus);
    return result ?? [];
  },
  async getGeoLocationRegionsInfo(authToken?: string): Promise<MediaGeoLocationRegionsInfo[]> {
    if (!backendUrl) {
      throw new Error('Please specify Backend URL in environment settings');
    }
    if (!authToken) {
      throw new Error('Please login');
    }
    let url = `${backendUrl}/mediageolocation/RegionsInfo`;
    const response = await fetch(url, {
      method: 'get',
      headers: new Headers({
        Authorization: `Bearer ${authToken}`,
      }),
    });
    const responseWithStatus = checkStatus(response);
    const result = await parseJSON(responseWithStatus);
    return result ?? [];
  },
  async getGeoLocationRegionSummary(
    region: string,
    authToken?: string,
  ): Promise<MediaGeoLocationRegionSummary[]> {
    if (!backendUrl) {
      throw new Error('Please specify Backend URL in environment settings');
    }
    if (!authToken) {
      throw new Error('Please login');
    }
    let url = `${backendUrl}/mediageolocation/RegionSummary?region=${region}`;
    const response = await fetch(url, {
      method: 'get',
      headers: new Headers({
        Authorization: `Bearer ${authToken}`,
      }),
    });
    const responseWithStatus = checkStatus(response);
    const result = await parseJSON(responseWithStatus);
    return result ?? [];
  },
};

export { backendAPI };
