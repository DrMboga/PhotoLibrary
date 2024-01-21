export const backendUrl = process.env.REACT_APP_BACKEND_URL;

const translateStatusToErrorMessage = (status: number) => {
  switch (status) {
    case 401:
      return 'Please login again.';
    case 403:
      return 'You do not have permission to view the project(s).';
    default:
      return 'There was an error retrieving backend data. Please try again.';
  }
};

export const checkStatus = (response: Response) => {
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
};

export const parseJSON = (response: Response) => {
  return response.json();
};
