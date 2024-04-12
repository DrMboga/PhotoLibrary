using MediatR;

namespace PhotoLibrary.Ml.LabelPredictor;

public record class GetConnectionStringRequest(): IRequest<string>;
