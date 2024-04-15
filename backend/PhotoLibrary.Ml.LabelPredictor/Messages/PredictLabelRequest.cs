using MediatR;

namespace PhotoLibrary.Ml.LabelPredictor;

public record PredictLabelRequest(string ImagePath): IRequest<LabelPredictionResult>;
