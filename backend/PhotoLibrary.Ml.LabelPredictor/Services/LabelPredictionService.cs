using MediatR;
using PhotoLibraryBackend.Common;

namespace PhotoLibrary.Ml.LabelPredictor;

public class LabelPredictionService: IRequestHandler<PredictLabelRequest, LabelPredictionResult>
{
    public Task<LabelPredictionResult> Handle(PredictLabelRequest request, CancellationToken cancellationToken)
    {
        var imageBytes = File.ReadAllBytes(request.ImagePath);
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
}
