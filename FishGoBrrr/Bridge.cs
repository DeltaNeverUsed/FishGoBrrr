using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace FishGoBrrr;

public class Bridge
{
    private readonly TcpListener _tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7345);
    private ButtClient Client => Mod.ButtClient;
    
    public Bridge()
    {
        Run();
    }

    private async void Run()
    {
        Mod.LogInformation("Starting bridge");
        _tcpListener.Start();
        Mod.LogInformation("Bridge started");
        TcpClient client = await _tcpListener.AcceptTcpClientAsync();
        Mod.LogInformation("Client connected");

        HandleClient(client);
    }

    private async void HandleClient(TcpClient client)
    {
        Mod.LogInformation("Client connected to bridge!");

        using (client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            while (client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer);
                string clientMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                
                try
                {
                    BridgeMessage? message = JsonSerializer.Deserialize<BridgeMessage>(clientMessage);
                    if (message == null)
                    {
                        Mod.LogInformation("Received null message");
                        continue;
                    }

                    HandleMessage(message);
                }
                catch (Exception e)
                {
                    Mod.LogInformation("Received message error: " + clientMessage);
                }
                
            }
        }
    }

    private async void HandleMessage(BridgeMessage message)
    {
        if (!Client.IsConnected)
            return;
        
        switch (message.Action)
        {
            case ButtAction.Vibrate:
                await Client.Vibrate(message.Value, message.Duration);
                break;
            case ButtAction.StopAll:
                await Client.StopAll();
                break;
            case ButtAction.Hover:
                await Client.Vibrate(Mod.Config.UiHoverVibrate, Mod.Config.UiHoverDuration);
                break;
            case ButtAction.Click:
                await Client.Vibrate(Mod.Config.UiClickVibrate, Mod.Config.UiClickDuration);
                break;
            default:
                Mod.LogInformation("Received invalid message");
                break;
        }
    }
}