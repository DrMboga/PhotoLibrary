namespace PhotoLibrary.Ml.LabelPredictor;

/// <summary>
/// According to learned ML model, there can be 3 labels: "Other", "People" and "Document"
/// </summary>
public record LabelPredictionResult (string Label, decimal OtherLabelScore, decimal PeopleLabelScore, decimal DocumentLabelScore);
