export const blobToImage = (imageAsBlob: Uint8Array): string => {
  let output = '';
  for (const element of imageAsBlob) {
    output += String.fromCharCode(element);
  }
  return `data:image/jpeg;base64,${btoa(output)}`;
};
