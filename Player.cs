using NAudio.CoreAudioApi;
using NAudio.Wave;

public class Player
{
    public string ClientName { get; }

    public string ClientIpAddress { get; }

    protected readonly MMDevice? _outputDevice;
    protected WasapiOut? _waveOut;
    protected BufferedWaveProvider? _waveProvider;

    public Player(string clientName, string remoteEndPoint)
    {
        ClientName = clientName;
        ClientIpAddress = remoteEndPoint;
    }

    public void UpdateOutputDevice(MMDevice? newDevice)
    {
        _waveOut?.Stop();
        _waveOut?.Dispose();

        if (newDevice != null)
        {
            _waveProvider = new BufferedWaveProvider(new WaveFormat(16000, 16, 1))
            {
                DiscardOnBufferOverflow = true,
                BufferDuration = TimeSpan.FromSeconds(5)
            };

            _waveOut = new WasapiOut(newDevice, AudioClientShareMode.Shared, false, 100);
            _waveOut.Init(_waveProvider);
            _waveOut.Play();
        }
    }

    public void play(byte[] data)
    {
        _waveProvider?.AddSamples(data, 0, data.Length);
    }

    public virtual void Dispose()
    {
        _waveOut?.Stop();
        _waveOut?.Dispose();
    }
}