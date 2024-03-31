using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoLibraryBackend.Common.Messages;

namespace PhotoLibraryBackend;

[ApiController]
[Route("worker/[action]")]
public class WorkerController: ControllerBase
{
    private readonly WorkerDispatcher _workerDispatcher;
    private readonly IMediator _mediator;

    public WorkerController(WorkerDispatcher workerDispatcher, IMediator mediator)
    {
        _workerDispatcher = workerDispatcher;
        _mediator = mediator;
    }

    [HttpPost()]
    [Authorize("EmailShouldBeConfirmed")]
    public IActionResult TriggerMediaImport()
    {
        var result = _workerDispatcher.StatNewProcess(
                typeof(ImportMediaBackgroundOperationType), 
                new ImportMediaBackgroundOperationContext());
        if (result.WorkflowSuccessfullyStarted)
        {
            return Ok();
        }
        return BadRequest(result);
    }

    [HttpPost()]
    public IActionResult TriggerVideoDatesFix()
    {
        var result = _workerDispatcher.StatNewProcess(
                typeof(FixVideoDatesBackgroundOperationType), 
                new FixVideoDatesBackgroundOperationContext());
        if (result.WorkflowSuccessfullyStarted)
        {
            return Ok();
        }
        return BadRequest(result);
    }

    // /triggerGeocodingDataCollect?requestsLimit=15
    [HttpPost()]
    public IActionResult TriggerGeocodingDataCollect(int requestsLimit)
    {
        var result = _workerDispatcher.StatNewProcess(
                typeof(GeocodingCollectBackgroundOperationType), 
                new GeocodingCollectBackgroundOperationContext(requestsLimit));
        if (result.WorkflowSuccessfullyStarted)
        {
            return Ok();
        }
        return BadRequest(result);
    }

    [HttpPost()]
    public IActionResult TriggerQuickTimeVideosConversion()
    {
        var result = _workerDispatcher.StatNewProcess(
                typeof(ConvertQuickTimeVideosOperationType), 
                new ConvertQuickTimeVideosOperationContext());
        if (result.WorkflowSuccessfullyStarted)
        {
            return Ok();
        }
        return BadRequest(result);
    }

    [HttpGet()]
    [Authorize("EmailShouldBeConfirmed")]
    public IActionResult MediaImportStatus()
    {
        return _workerDispatcher.IsInProgress ? Ok("InProgress") : Ok("Idle");
    }

    [HttpGet()]
    [Authorize("EmailShouldBeConfirmed")]
    public async Task<IActionResult> GeocodingStatus()
    {
        var emptyAddresses = await _mediator.Send(new GetMediaAddressesCountRequest(true));
        var filledAddresses = await _mediator.Send(new GetMediaAddressesCountRequest(false));
        return Ok(new {EmptyAddressesCount = emptyAddresses, FilledAddressesCount = filledAddresses});
    }

    [HttpGet()]
    [Authorize("EmailShouldBeConfirmed")]
    public async Task<ImportStepReport[]> ImporterLogs(int? pageSize)
    {
        var logs = await _mediator.Send(new GetImporterLogsRequest(pageSize ?? 100));
        return logs.ToStepReports();
    }
}
