using GrpcExecutor.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

// Path should probably be known by both ProxyExecutor and JobExecutor (move to shared?)
string socketFile;
if (args.Length == 1)
{
    socketFile = $"{args[0]}.socket";
}
else
{
    Console.WriteLine("Expected a single argument.");
    return 1;
}

var socketPath = Path.Combine(Path.GetTempPath(), socketFile);

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenUnixSocket(socketPath, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ExecutorService>();

Console.WriteLine($"Listening on {socketPath}");

if (File.Exists(socketPath))
{
    // Usually is not required - Kestrel should clean up after itself.
    // In case it doesn't - lets do it ourselves.
    Console.WriteLine("Removing socket");
    File.Delete(socketPath);
}

app.Run();
return 0;