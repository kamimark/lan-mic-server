using System.Net;
using System.Net.Sockets;

public class AudioServer
{
    private TcpListener _listener = new(IPAddress.Any, 0);
    private UdpClient? _udpServer;
    public event Action<TcpClient>? OnClientConnected;
    public event Action<IPEndPoint, byte[]>? OnReceivedData;
    public int Port { get; private set; }
    private readonly CancellationTokenSource cancellationTokenSource;
    private Task? tcpTask;
    private Task? udpTask;

    public AudioServer(CancellationTokenSource cancellationTokenSource)
    {
        this.cancellationTokenSource = cancellationTokenSource;
    }

    public void Start()
    {
        _listener.Start();
        Port = ((IPEndPoint)_listener.LocalEndpoint).Port;

        _udpServer = new UdpClient(Port);
        _udpServer.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        _udpServer.Client.ReceiveBufferSize = 8 * 1024 * 1024;
        var remoteEP = new IPEndPoint(IPAddress.Any, 0);
        while (_udpServer.Available > 0)
        {
            var result = _udpServer.Receive(ref remoteEP);
            Console.WriteLine(result);
        }

        Console.WriteLine($"Audio server listening on port: {Port}");

        tcpTask = Task.Run(AcceptClientsAsync);
        udpTask = Task.Run(StartUdpServerAsync);
    }

    private async Task AcceptClientsAsync()
    {
        while (!cancellationTokenSource.Token.IsCancellationRequested)
        {
            var tcpClient = await _listener.AcceptTcpClientAsync();
            try
            {
                OnClientConnected?.Invoke(tcpClient);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ClientConnected threw exception {e}");
            }
        }

        Console.WriteLine($"Audio server stopped listening");
        _listener?.Stop();
    }

    private async Task StartUdpServerAsync()
    {
        while (!cancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                UdpReceiveResult result = await _udpServer!.ReceiveAsync();
                OnReceivedData?.Invoke(result.RemoteEndPoint, result.Buffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UDP error: {ex}");
            }
        }
        _udpServer?.Close();
    }

    public void Stop()
    {
        _listener?.Stop();
        tcpTask?.GetAwaiter().GetResult();
        udpTask?.GetAwaiter().GetResult();
    }
}
