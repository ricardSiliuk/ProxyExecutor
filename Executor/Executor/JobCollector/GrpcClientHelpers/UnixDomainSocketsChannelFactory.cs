using System.Net.Sockets;
using Grpc.Net.Client;

namespace ProxyExecutor.GrpcClientHelpers;

public static class UnixDomainSocketsChannelFactory
{
    private static readonly string SocketBasePath = Path.GetTempPath();

    public static GrpcChannel CreateChannel(string path)
    {
        var socketPath = Path.Combine(SocketBasePath, $"{path}.socket");

        var udsEndPoint = new UnixDomainSocketEndPoint(socketPath);
        var connectionFactory = new UnixDomainSocketsConnectionFactory(udsEndPoint);
        var socketsHttpHandler = new SocketsHttpHandler
        {
            ConnectCallback = connectionFactory.ConnectAsync
        };

        return GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
        {
            HttpHandler = socketsHttpHandler
        });
    }
}