namespace PhotoLibraryBackend;

public interface IBackgroundOperationType
{
    Task StartOperation(IBackgroundOperationContext context);
}
