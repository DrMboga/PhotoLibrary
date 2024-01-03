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
    switch (await responseWithStatus.text()) {
      case 'InProgress':
        return true;
      default:
        return false;
    }
  },
};

export { backendAPI };
