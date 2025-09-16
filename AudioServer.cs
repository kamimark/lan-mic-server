using System.Net;
using System.Net.Sockets;

public class AudioServer
{
    private TcpListener _listener = new(IPAddress.Any, 0);
    public event Action<TcpClient> ClientConnected;
    public int Port { get; private set; }
    private readonly CancellationTokenSource cancellationTokenSource;
    private Task task;

    public AudioServer(CancellationTokenSource cancellationTokenSource)
    {
        this.cancellationTokenSource = cancellationTokenSource;
    }

    public void Start()
    {
        _listener.Start();

        Port = ((IPEndPoint)_listener.LocalEndpoint).Port;

        Console.WriteLine($"Audio server listening on port: {((IPEndPoint)_listener.LocalEndpoint).Port}");

        task = Task.Run(AcceptClientsAsync);
    }

    private async Task AcceptClientsAsync()
    {
        while (!cancellationTokenSource.Token.IsCancellationRequested)
        {
            var tcpClient = await _listener.AcceptTcpClientAsync();
            try
            {
                ClientConnected?.Invoke(tcpClient);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ClientConnected threw exception {e}");
            }
        }

        Console.WriteLine($"Audio server stopped listening");
        _listener?.Stop();
    }

    public void Stop()
    {
        _listener?.Stop();
        task.GetAwaiter().GetResult();
    }
}
