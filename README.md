# LanMicServer

LanMicServer is a Windows desktop application built with .NET and WinForms that acts as an audio server over LAN. It receives audio streams from connected clients via TCP sockets and plays them back on selected Windows playback devices.

---

## Features

- **Discovery Protocol:**  
  Listens for UDP discovery messages from clients and responds with its audio TCP port.

- **Multiple Socket Clients:**  
  Accepts simultaneous TCP connections from multiple clients sending audio data.

- **Device Management UI:**  
  Shows three sections in the UI:  
  1. Available unpaired playback devices  
  2. Connected unpaired clients  
  3. Paired client-device connections

- **Pairing Interface:**  
  Select one playback device and one client, then press "Pair" to bind them. Paired devices play audio from the corresponding client.

- **Dynamic Audio Routing:**  
  Audio playback devices can be switched dynamically by pairing/unpairing.

- **Unpairing:**  
  Easily unpair devices and clients from the paired section.

---

## Requirements

- Windows OS  
- .NET 10 SDK (or later)  
- [NAudio](https://github.com/naudio/NAudio) library for audio playback

---

## Setup

1. Clone this repository:

   ```bash
   git clone https://github.com/yourusername/LanMicServer.git
   cd LanMicServer
