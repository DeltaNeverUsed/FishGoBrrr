using Buttplug;
using Buttplug.SystemTextJson;

namespace FishGoBrrr;

public class ButtClient
{
    private static readonly CancellationToken CancellationToken = CancellationToken.None;

    private readonly ButtplugSystemTextJsonConverter _converter = new();
    private readonly ButtplugClient _buttplugClient;

    public bool IsConnected => _buttplugClient.IsConnected;
    
    public ButtClient()
    {
        _buttplugClient = new ButtplugClient("WebFishing", _converter);
        Setup();
    }

    private async Task Setup()
    {
        _buttplugClient.DeviceAdded += HandleDeviceAdded;
        _buttplugClient.DeviceRemoved += HandleDeviceRemoved;
        
        Mod.LogInformation($"Connecting to Intiface at {Mod.Config.URL}");
        await _buttplugClient.ConnectAsync(new Uri(Mod.Config.URL), CancellationToken);
        Mod.LogInformation("Connected to Intiface!");

        await _buttplugClient.StartScanningAsync(CancellationToken);
    }

    private void HandleDeviceAdded(object? sender, ButtplugDevice buttplugDevice)
    {
        Mod.Interface.Logger.Information($"Device Added: {buttplugDevice}");
    }
    
    private void HandleDeviceRemoved(object? sender, ButtplugDevice buttplugDevice)
    {
        Mod.Interface.Logger.Information($"Device Removed: {buttplugDevice}");
    }

    public async Task StopAllIn(double seconds)
    {
        if (!_buttplugClient.IsConnected)
            return;
        if (seconds <= 0)
            return;
        await Task.Delay((int)(seconds * 1000), CancellationToken);
        await StopAll();
    }
    
    public async Task Vibrate(double scalar, double time)
    {
        if (!_buttplugClient.IsConnected)
            return;
        if (scalar < 0.01)
            return;
        //Mod.LogInformation($"Vibrating at {scalar} time = {time}");
        foreach (var device in _buttplugClient.Devices)
            await device.ScalarAsync(scalar, ActuatorType.Vibrate, CancellationToken);
        _ = StopAllIn(time);
    }

    public async Task StopAll()
    {
        if (!_buttplugClient.IsConnected)
            return;
        await _buttplugClient.StopAllDevicesAsync(CancellationToken);
    }
}