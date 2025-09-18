using System.Net.Sockets;

public class TcpClientAudioPlayer : Player
{
    public TcpClient TcpClient { get; }

    public event Action? ClientDisconnected;


    public TcpClientAudioPlayer(TcpClient tcpClient, string clientName, CancellationToken token) : base(clientName, tcpClient.Client.RemoteEndPoint?.ToString() ?? "UnknownClient")
    {
        TcpClient = tcpClient;
        Task.Run(() => ReceiveAudioAsync(token));
    }

    private async Task ReceiveAudioAsync(CancellationToken token)
    {
        var buffer = new byte[4096];
        var stream = TcpClient.GetStream();

        try
        {
            while (!token.IsCancellationRequested)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);
                if (bytesRead == 0)
                {
                    ClientDisconnected?.Invoke();
                    break; // Client disconnected
                }

                play(buffer);
            }
        }
        catch { /* Handle exceptions as needed */ }
    }

    public override void Dispose()
    {
        base.Dispose();
        TcpClient.Close();
    }
}
