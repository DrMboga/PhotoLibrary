import { ImportStepReport } from '../model/media-info';

const backendUrl = process.env.REACT_APP_BACKEND_URL;

function translateStatusToErrorMessage(status: number) {
  switch (status) {
    case 401:
      return 'Please login again.';
    case 403:
      return 'You do not have permission to view the project(s).';
    default:
      return 'There was an error retrieving backend data. Please try again.';
  }
}

function checkStatus(response: Response) {
  if (response.ok) {
    return response;
  } else {
    const httpErrorInfo = {
      status: response.status,
      statusText: response.statusText,
      url: response.url,
    };
    console.log(`log server http error: ${JSON.stringify(httpErrorInfo)}`);

    let errorMessage = translateStatusToErrorMessage(httpErrorInfo.status);
    throw new Error(errorMessage);
  }
}

function parseJSON(response: Response) {
  return response.json();
}

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
  async downloadMedia(fullPath: string, authToken?: string): Promise<Blob> {
    if (!backendUrl) {
      throw new Error('Please specify Backend URL in environment settings');
    }
    if (!authToken) {
      throw new Error('Please login');
    }
    const response = await fetch(`${backendUrl}/mediaDownload?filePath=${fullPath}`, {
      method: 'get',
      headers: new Headers({
        Authorization: `Bearer ${authToken}`,
      }),
    });
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
};

export { backendAPI };
