using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;
using NAudio.Wave;

public class ClientAudioPlayer
{
    public string ClientName { get; }
    public TcpClient TcpClient { get; }
    private readonly MMDevice _outputDevice;
    private WasapiOut _waveOut;
    private BufferedWaveProvider _waveProvider;
    private CancellationTokenSource _cts;
    public event Action ClientDisconnected;


    public ClientAudioPlayer(MMDevice outputDevice, TcpClient tcpClient, string clientName)
    {
        ClientName = clientName;
        TcpClient = tcpClient;
        _outputDevice = outputDevice;

        _waveProvider = new BufferedWaveProvider(new WaveFormat(16000, 16, 1))
        {
            DiscardOnBufferOverflow = true
        };

        _waveOut = new WasapiOut(_outputDevice, AudioClientShareMode.Shared, false, 100);
        _waveOut.Init(_waveProvider);
        _waveOut.Play();

        _cts = new CancellationTokenSource();
        Task.Run(() => ReceiveAudioAsync(_cts.Token));
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

                _waveProvider.AddSamples(buffer, 0, bytesRead);
            }
        }
        catch { /* Handle exceptions as needed */ }
    }

    public void UpdateOutputDevice(MMDevice newDevice)
    {
        _waveOut.Stop();
        _waveOut.Dispose();

        _waveProvider = new BufferedWaveProvider(new WaveFormat(16000, 16, 1))
        {
            DiscardOnBufferOverflow = true,
            BufferDuration = TimeSpan.FromSeconds(5)
        };

        _waveOut = new WasapiOut(newDevice, AudioClientShareMode.Shared, false, 100);
        _waveOut.Init(_waveProvider);
        _waveOut.Play();
    }

    public void Dispose()
    {
        _cts.Cancel();
        _waveOut.Stop();
        _waveOut.Dispose();
        TcpClient.Close();
    }
}
