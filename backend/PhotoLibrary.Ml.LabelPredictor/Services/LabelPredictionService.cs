using System.Net;
using MediatR;
using PhotoLibraryBackend.Common;
using SMBLibrary;
using SMBLibrary.Client;

namespace PhotoLibrary.Ml.LabelPredictor;

public class LabelPredictionService: IRequestHandler<PredictLabelRequest, LabelPredictionResult>
{
    private readonly SambaCredentials _sambaCredentials;

    public LabelPredictionService(SambaCredentials sambaCredentials)
    {
        _sambaCredentials = sambaCredentials;
    }

    public Task<LabelPredictionResult> Handle(PredictLabelRequest request, CancellationToken cancellationToken)
    {
        var imageBytes = ReadFileFromSamba(request.ImagePath);
        var sampleData = new PhotoLibraryModel.ModelInput()
            {
                ImageSource = imageBytes,
            };

        var predictionResult = PhotoLibraryModel.Predict(sampleData);
        return Task.FromResult(
            new LabelPredictionResult(
                predictionResult.PredictedLabel, 
                Convert.ToDecimal(predictionResult?.Score?[0] ?? -1),
                Convert.ToDecimal(predictionResult?.Score?[1] ?? -1),
                Convert.ToDecimal(predictionResult?.Score?[2] ?? -1)
                )
            );
    }

    private byte[] ReadFileFromSamba(string path)
    {
        byte[]? data = null;
        SMB1Client client = new SMB1Client(); 
        bool isConnected = client.Connect(IPAddress.Parse(_sambaCredentials.Address), SMBTransportType.DirectTCPTransport);

        Console.WriteLine($"Smb connection status {isConnected}");

        if (isConnected)
        {
            NTStatus status = client.Login(String.Empty, _sambaCredentials.Login, _sambaCredentials.Password);

            Console.WriteLine($"Smb login status {status}");

            if (status == NTStatus.STATUS_SUCCESS)
            {
                ISMBFileStore fileStore = client.TreeConnect("Shared", out status);
                object fileHandle;
                FileStatus fileStatus;
                status = fileStore.CreateFile(
                    out fileHandle, 
                    out fileStatus, 
                    path, 
                    AccessMask.GENERIC_READ | AccessMask.SYNCHRONIZE, 
                    SMBLibrary.FileAttributes.Normal, 
                    ShareAccess.Read, 
                    CreateDisposition.FILE_OPEN, 
                    CreateOptions.FILE_NON_DIRECTORY_FILE | CreateOptions.FILE_SYNCHRONOUS_IO_ALERT, 
                    null);
                
                Console.WriteLine($"Smb fileStore status {status}");

                if (status == NTStatus.STATUS_SUCCESS)
                {
                    var stream = new MemoryStream();
                    long bytesRead = 0;
                    while (true)
                    {
                        status = fileStore.ReadFile(out data, fileHandle, bytesRead, (int)client.MaxReadSize);
                        if (status != NTStatus.STATUS_SUCCESS && status != NTStatus.STATUS_END_OF_FILE)
                        {
                            throw new Exception("Failed to read from file");
                        }

                        if (status == NTStatus.STATUS_END_OF_FILE || data.Length == 0)
                        {
                            break;
                        }
                        bytesRead += data.Length;
                        stream.Write(data, 0, data.Length);
                    }
                }
                status = fileStore.CloseFile(fileHandle);
                status = fileStore.Disconnect();

                
                client.Logoff();
            }
            client.Disconnect();
        }

        return data ?? []; //File.ReadAllBytes(path);
    }
}
