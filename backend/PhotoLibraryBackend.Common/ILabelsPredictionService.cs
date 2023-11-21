namespace PhotoLibraryBackend.Common;

public interface ILabelsPredictionService
{
    /// <summary>
    /// Method predicts a photo label according learned ML model
    /// </summary>
    LabelPredictionResult PredictLabel(string imagePath);
}
