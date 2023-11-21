namespace PhotoLibraryBackend.MediaReader;

public class LabelPredictionService : ILabelsPredictionService
{
    /// <inheritdoc />
    public LabelPredictionResult PredictLabel(string imagePath)
    {
        var imageBytes = File.ReadAllBytes(imagePath);
        var sampleData = new PhotoLibraryModel.ModelInput()
            {
                ImageSource = imageBytes,
            };

        var predictionResult = PhotoLibraryModel.Predict(sampleData);
        return new LabelPredictionResult(
            predictionResult.PredictedLabel, 
            Convert.ToDecimal(predictionResult.Score[0]),
            Convert.ToDecimal(predictionResult.Score[1]),
            Convert.ToDecimal(predictionResult.Score[2])
            );
    }
}
