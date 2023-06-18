using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProxyExecutor.GrpcClientHelpers;
using ProxyExecutor.Models;

namespace ProxyExecutor.Controllers;

[ApiController]
[Route("[controller]")]
public class ExecutorController : ControllerBase
{
    private readonly ILogger<ExecutorController> _logger;

    public ExecutorController(ILogger<ExecutorController> logger)
    {
        _logger = logger;
    }

    [HttpPost(Name = "/Calculate")]
    public async Task<ExecuteCommandResponse> Calculate([FromBody] ExecuteCommandRequestDto dto)
    {
        _logger.LogInformation("Will request {RequestPath} from {SocketId}", dto.Path, dto.SocketId);
        var myProcess = Process.Start("JobExecutor.exe", dto.SocketId);
        
        // TODO: figure out a way to not do a fixed time delay
        await Task.Delay(3000);
        
        var client = new Executor.ExecutorClient(UnixDomainSocketsChannelFactory.CreateChannel(dto.SocketId));
        // This will die if JobExecutor dies - which is nice.
        var response = await client.ExecuteCommandAsync(new ExecuteCommandRequest{ExecutionPath = dto.Path});

        myProcess.Kill();
        
        _logger.LogInformation("Request served");
        return response;
    }
}