using Grpc.Core;

namespace GrpcExecutor.Services;

public class ExecutorService : Executor.ExecutorBase
{
    private readonly ILogger<ExecutorService> _logger;
    
    public ExecutorService(ILogger<ExecutorService> logger)
    {
        _logger = logger;
    }
    
    public override async Task<ExecuteCommandResponse> ExecuteCommand(
        ExecuteCommandRequest request,
        ServerCallContext context
    )
    {
        _logger.LogInformation("Received request to load and execute with {ExecutionPath}", request.ExecutionPath);

        await Task.Delay(1000);

        return new ExecuteCommandResponse
            {
                Result = $"Calculated with {request.ExecutionPath} path"
            };
    }
}