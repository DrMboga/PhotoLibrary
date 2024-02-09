import { ImportStepReport, MediaInfo } from '../model/media-info';
import { backendUrl, checkStatus, parseJSON } from './ApiHelper';

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
    const response = await fetch(`${backendUrl}/mediaImportStatus`, {
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
    const response = await fetch(`${backendUrl}/importerLogs`, {
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
    const response = await fetch(`${backendUrl}/triggerMediaImport`, {
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
      `${backendUrl}/triggerGeocodingDataCollect?requestsLimit=${requestLimit}`,
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
      `${backendUrl}/mediaDownload?filePath=${fullPath}&useConvertedVideo=${useConvertedVideo}`,
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
    const response = await fetch(`${backendUrl}/mediaEdit?mediaId=${mediaId}`, {
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
    let url = `${backendUrl}/mediaAlbum?mediaId=${mediaId}`;
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
    let url = `${backendUrl}/mediaByAlbum?`;
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
    const response = await fetch(`${backendUrl}/geocodingStatus?`, {
      method: 'get',
      headers: new Headers({
        Authorization: `Bearer ${authToken}`,
      }),
    });
    const responseWithStatus = checkStatus(response);
    const result = await parseJSON(responseWithStatus);
    return result ?? { emptyAddressesCount: 0, filledAddressesCount: 0 };
  },
};

export { backendAPI };
