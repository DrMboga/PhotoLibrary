using MediatR;
using PhotoLibraryBackend.Common;

namespace PhotoLibrary.Ml.LabelPredictor;

public record PredictLabelRequest(string ImagePath): IRequest<LabelPredictionResult>;
