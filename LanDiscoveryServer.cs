using System.Net;
using System.Net.Sockets;
using System.Text;


public class LanDiscoveryServer
{
    private const int DiscoveryPort = 50763;
    private readonly AudioServer audioServer;
    private readonly Echoer echoer;
    private readonly UdpClient udpServer;
    private const string DISCOVERY_RECEIVE = "LANMIC_CLIENT_DISCOVERY-";
    private const string DISCOVERY_SEND = "LANMIC_SERVER_RESPONSE-";
    private readonly CancellationTokenSource cancellationTokenSource;

    public event Action<IPEndPoint, string>? ClientDiscovered;

    public LanDiscoveryServer(AudioServer audioServer, Echoer echoer, CancellationTokenSource cancellationTokenSource)
    {
        this.audioServer = audioServer;
        this.echoer = echoer;
        this.cancellationTokenSource = cancellationTokenSource;
        udpServer = new UdpClient(DiscoveryPort);
        udpServer.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        udpServer.Client.ReceiveBufferSize = 8 * 1024 * 1024;
        int actualSize = udpServer.Client.ReceiveBufferSize;
        Console.WriteLine($"ReceiveBufferSize: {actualSize}");
        var remoteEP = new IPEndPoint(IPAddress.Any, 0);
        while (udpServer.Available > 0)
        {
            var result = udpServer.Receive(ref remoteEP);
            Console.WriteLine(result);
        }
    }

    public void Start()
    {
        Task.Run(StartAsync);
    }

    private async ValueTask StartAsync()
    {
        Console.WriteLine($"[Discovery] Listening on UDP port {DiscoveryPort}");
        var cancellationToken = cancellationTokenSource.Token;

        while (!cancellationToken.IsCancellationRequested)
        {
            await ProccessDiscoveryRequest(cancellationToken);
        }

        udpServer.Close();
    }

    private async ValueTask ProccessDiscoveryRequest(CancellationToken cancellationToken)
    {
        UdpReceiveResult result;
        try
        {
            result = await udpServer.ReceiveAsync(cancellationToken);
            string received = Encoding.UTF8.GetString(result.Buffer);
            Console.WriteLine($"[Discovery] Received: {received} from {result.RemoteEndPoint}");

            if (received.StartsWith(DISCOVERY_RECEIVE, StringComparison.Ordinal))
            {
                ClientDiscovered?.Invoke(result.RemoteEndPoint, received.Substring(DISCOVERY_RECEIVE.Length));
                string response = $"{DISCOVERY_SEND}{audioServer.Port}-{echoer.Port}";
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                await udpServer.SendAsync(responseBytes, responseBytes.Length, result.RemoteEndPoint);

                Console.WriteLine($"[Discovery] Sent: {response}");
            }
        }
        catch (OperationCanceledException e)
        {
            Console.WriteLine(e);
        }
    }
}