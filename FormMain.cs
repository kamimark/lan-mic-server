using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using NAudio.CoreAudioApi;

public partial class FormMain : Form
{
    private AudioServer _audioServer;
    private Echoer _echoer;
    private LanDiscoveryServer _discoveryServer;
    private readonly MMDeviceEnumerator _deviceEnumerator = new MMDeviceEnumerator();
    private Dictionary<MMDevice, CustomButton> _deviceButtons = new();
    private List<MMDevice> _availableDevices = new();
    private List<Player> _clients = new();
    private List<(Player client, MMDevice device)> _pairedClients = new();
    private Dictionary<string, string> _discoveredClients = new();
    private Dictionary<string, Player> _players = new();

    private MMDevice? _selectedDevice;
    private CustomButton? _selectedDeviceButton;
    private MMDevice _defaultDevice;
    private Player? _selectedClient;
    private CustomButton? _selectedClientButton;

    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private const string ShowOnly = "Virtual Audio Cable";

    public FormMain()
    {
        InitializeComponent();
        LoadPlaybackDevices();

        _audioServer = new AudioServer(cancellationTokenSource);
        _audioServer.OnClientConnected += AudioServer_ClientConnected;
        _audioServer.OnReceivedData += AudioServer_ReceivedData;
        _audioServer.Start();

        _echoer = new Echoer(cancellationTokenSource);
        _echoer.Start();

        _discoveryServer = new LanDiscoveryServer(_audioServer, _echoer, cancellationTokenSource);
        _discoveryServer.ClientDiscovered += Client_Discovered;
        _discoveryServer.Start();

        UpdateUI();
    }

    private void Client_Discovered(IPEndPoint point, string name)
    {
        _discoveredClients[point.Address.ToString()] = name;
    }

    private void LoadPlaybackDevices()
    {
        _defaultDevice = GetDefaultDevice();
        _availableDevices.Clear();
        panelDevices.Controls.Clear();
        _deviceButtons.Clear();

        foreach (var device in _deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
        {
            if (!_defaultDevice.ID.Equals(device.ID) && !device.FriendlyName.Contains(ShowOnly))
                continue;

            _availableDevices.Add(device);

            var btn = new CustomButton
            {
                Text = device.FriendlyName,
                Tag = device,
                AutoSize = true,
                Margin = new Padding(3)
            };
            btn.Click += DeviceButton_Click;
            _deviceButtons[device] = btn;
        }

        _availableDevices.Sort(SortDevices);

        foreach (var device in _availableDevices)
            panelDevices.Controls.Add(_deviceButtons[device]);
    }

    private int SortDevices(MMDevice a, MMDevice b)
    {
        if (!a.FriendlyName.Contains(ShowOnly))
            return -1;

        var aNum = int.Parse(Regex.Match(a.FriendlyName, @"\d+").Value!);
        var bNum = int.Parse(Regex.Match(b.FriendlyName, @"\d+").Value!);

        return aNum - bNum;
    }

    private void DeviceButton_Click(object sender, EventArgs e)
    {
        var btn = (CustomButton)sender;
        var device = (MMDevice)btn.Tag;

        // Toggle selection
        if (_selectedDevice == device)
        {
            _selectedDevice = null;
            _selectedDeviceButton = null;
            btn.IsSelected = false;
        }
        else
        {
            _selectedDevice = device;
            _selectedDeviceButton?.IsSelected = false;
            btn.IsSelected = true;
            _selectedDeviceButton = btn;
        }

        UpdateUI();
        TryPair();
    }

    private void AudioServer_ClientConnected(TcpClient tcpClient)
    {
        Invoke((Delegate)(() =>
        {
            var endpoint = tcpClient.Client.RemoteEndPoint;
            var pointStr = endpoint!.ToString();
            var address = pointStr.Split(':')[0];
            if (!_discoveredClients.TryGetValue(address, out var clientName))
                clientName = "Unknown";

            var player = new TcpClientAudioPlayer(tcpClient, clientName, cancellationTokenSource.Token);
            _clients.Add(player);

            var btn = new CustomButton
            {
                Text = $"{clientName}\n{endpoint}",
                Tag = player,
                AutoSize = true,
                Margin = new Padding(3)
            };
            btn.Click += ClientButton_Click;
            panelClients.Controls.Add(btn);
            UpdateUI();

            player.ClientDisconnected += () =>
            {
                Invoke((Delegate)(() =>
                {
                    RemoveClient(player);
                    RemovePair(player);
                    _clients.Remove(player);
                    UpdateUI();
                    player.Dispose();
                }));
            };
        }));
    }

    private void AudioServer_ReceivedData(EndPoint endpoint, byte[] data)
    {
        Invoke((Delegate)(() =>
        {
            var pointStr = endpoint.ToString();
            var address = pointStr.Split(':')[0];
            if (!_discoveredClients.TryGetValue(address, out var clientName))
                clientName = "Unknown";

            if (!_players.TryGetValue(address, out var player))
            {
                player = new Player(clientName, address);
                _players[address] = player;
                _clients.Add(player);
                var btn = new CustomButton
                {
                    Text = $"{clientName}\n{endpoint}",
                    Tag = player,
                    AutoSize = true,
                    Margin = new Padding(3)
                };
                btn.Click += ClientButton_Click;
                panelClients.Controls.Add(btn);
                UpdateUI();
            }

            player.play(data);
        }));
    }

    private void RemovePair(Player player)
    {
        foreach (var btn in panelPaired.Controls)
        {
            if (btn is CustomButton button)
            {
                var (client, device) = ((Player, MMDevice))button.Tag;
                if (client == player)
                {
                    _pairedClients.Remove((client, device));
                    return;
                }
            }
        }
    }

    private void RemoveClient(Player player)
    {
        foreach (var btn in panelClients.Controls)
        {
            if (btn is CustomButton button)
            {
                if (button.Tag == player)
                {
                    panelClients.Controls.Remove(button);
                    return;
                }
            }
        }
    }

    private void ClientButton_Click(object sender, EventArgs e)
    {
        var btn = (CustomButton)sender;
        var client = (Player)btn.Tag;

        if (_selectedClient == client)
        {
            _selectedClient = null;
            _selectedClientButton = null;
        }
        else
        {
            _selectedClient = client;
            _selectedClientButton?.IsSelected = false;
            btn.IsSelected = true;
            _selectedClientButton = btn;
        }

        UpdateUI();
        TryPair();
    }

    private void TryPair()
    {
        if (_selectedDevice != null && _selectedClient != null)
        {
            // Pair client with device
            _selectedClient.UpdateOutputDevice(_selectedDevice);

            _pairedClients.Add((_selectedClient, _selectedDevice));
            _clients.Remove(_selectedClient);

            _selectedClientButton?.IsSelected = false;
            _selectedClientButton = null;
            _selectedClient = null;
            _selectedDeviceButton?.IsSelected = false;
            _selectedDeviceButton = null;
            _selectedDevice = null;

            UpdateUI();
        }
    }

    private bool isPaired(MMDevice device)
    {
        foreach (var (client, d) in _pairedClients)
        {
            if (device.Equals(d))
                return true;
        }

        return false;
    }

    private void UpdateUI()
    {
        // Update Devices panel buttons
        panelDevices.Controls.Clear();
        var defaultID = _defaultDevice.ID;

        foreach (var device in _availableDevices)
        {
            if (!_deviceButtons.TryGetValue(device, out var btn))
                continue;
            if (!defaultID.Equals(device.ID) && isPaired(device))
                continue;

            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 2;
            btn.FlatAppearance.BorderColor = Color.Black;
            // btn.BackColor = device == _selectedDevice ? Color.LightBlue : Color.LightGray;
            panelDevices.Controls.Add(btn);
        }

        // Update Clients panel buttons
        foreach (CustomButton btn in panelClients.Controls)
        {
            var client = (Player)btn.Tag;
            btn.BackColor = client == _selectedClient ? Color.LightBlue : Color.LightGray;
            btn.Enabled = _clients.Contains(client);
        }

        // Update Paired panel
        panelPaired.Controls.Clear();
        foreach (var (client, device) in _pairedClients)
        {
            var btn = new CustomButton
            {
                Text = $"{device.FriendlyName}\n{client.ClientName}\n{client.ClientIpAddress}",
                Tag = (client, device),
                AutoSize = true,
                Margin = new Padding(3),
                IsSelected = true,
            };
            btn.Click += PairedButton_Click;
            panelPaired.Controls.Add(btn);
        }
    }

    private void PairedButton_Click(object sender, EventArgs e)
    {
        var btn = (CustomButton)sender;
        var (client, device) = ((Player, MMDevice))btn.Tag;

        // Unpair
        _pairedClients.RemoveAll(p => p.client == client && p.device == device);

        _clients.Add(client);

        // Switch client back to default device
        client.UpdateOutputDevice(null);

        UpdateUI();
    }

    private MMDevice GetDefaultDevice()
    {
        try
        {
            return _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        }
        catch
        {
            return null;
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        cancellationTokenSource.Cancel();
        base.OnFormClosing(e);

        foreach (var client in _clients)
            client.Dispose();

        foreach (var (client, _) in _pairedClients)
            client.Dispose();

        _audioServer.Stop();
    }
}
