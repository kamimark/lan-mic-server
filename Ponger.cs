using System.Net;
using System.Net.Sockets;

public class Echoer
{
    public event Action<TcpClient> ClientConnected;
    public int Port { get; private set; }
    private readonly CancellationTokenSource cancellationTokenSource;
    private readonly UdpClient udpServer;

    public Echoer(CancellationTokenSource cancellationTokenSource)
    {
        this.cancellationTokenSource = cancellationTokenSource;
        udpServer = new UdpClient(0);
        udpServer.Client.ReceiveBufferSize = 8 * 1024 * 1024;
        var remoteEP = new IPEndPoint(IPAddress.Any, 0);
        while (udpServer.Available > 0)
        {
            var result = udpServer.Receive(ref remoteEP);
            Console.WriteLine(result);
        }
        IPEndPoint localEndPoint = (IPEndPoint)udpServer.Client.LocalEndPoint!;
        Port = localEndPoint.Port;
        Console.WriteLine($"[Echoer] Listening on UDP port {Port}");
    }

    public void Start()
    {
        Task.Run(StartAsync);
    }

    private async ValueTask StartAsync()
    {
        var cancellationToken = cancellationTokenSource.Token;

        while (!cancellationToken.IsCancellationRequested)
        {
            await ProccessPing(cancellationToken);
        }

        udpServer.Close();
    }

    private async ValueTask ProccessPing(CancellationToken cancellationToken)
    {
        UdpReceiveResult result;
        try
        {
            result = await udpServer.ReceiveAsync(cancellationToken);
            await udpServer.SendAsync(result.Buffer, result.Buffer.Length, result.RemoteEndPoint);
        }
        catch (OperationCanceledException e)
        {
            Console.WriteLine(e);
        }
    }
}
