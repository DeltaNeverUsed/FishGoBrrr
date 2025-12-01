using System.Text;
using Buttplug.Client;
using Buttplug.Core.Messages;

namespace FishGoBrrr;

public class ButtClient {
    private static readonly CancellationToken CancellationToken = CancellationToken.None;

    private readonly string[] ActuatorTypes = Enum.GetNames(typeof(ActuatorType));
    private readonly ButtplugClient _buttplugClient;

    public bool IsConnected => _buttplugClient.Connected;

    public ButtClient() 
    {
        _buttplugClient = new ButtplugClient("WebFishing");
        Setup();
    }

    private async Task Setup() 
    {
        _buttplugClient.DeviceAdded += HandleDeviceAdded;
        _buttplugClient.DeviceRemoved += HandleDeviceRemoved;

        Mod.LogInformation($"Connecting to Intiface at {Mod.Config.URL}");
        var connector = new ButtplugWebsocketConnector(new Uri(Mod.Config.URL));
        await _buttplugClient.ConnectAsync(connector, CancellationToken);
        Mod.LogInformation("Connected to Intiface!");

        await _buttplugClient.StartScanningAsync(CancellationToken);
    }

    private void HandleDeviceAdded(object? sender, DeviceAddedEventArgs deviceAddedEventArgs) 
    {
        var buttplugDevice = deviceAddedEventArgs.Device;
        Mod.LogInformation($"Device Added: {buttplugDevice}");
        var sb = new StringBuilder();
        var l = buttplugDevice.MessageAttributes.ScalarCmd;
        foreach (var t in l) {
            sb.Append(t.ActuatorType);
            sb.Append(", ");
        }

        Mod.LogInformation($"Device has Actuators: {sb}");
    }

    private void HandleDeviceRemoved(object? sender, DeviceRemovedEventArgs deviceRemovedEventArgs) 
    {
        var buttplugDevice = deviceRemovedEventArgs.Device;
        Mod.LogInformation($"Device Removed: {buttplugDevice}");
    }

    public async Task StopAllIn(double seconds) 
    {
        if (!_buttplugClient.Connected)
            return;
        if (seconds <= 0)
            return;
        await Task.Delay((int)(seconds * 1000));
        await StopAll();
    }

    public async Task Vibrate(double scalar, double time) 
    {
        if (!_buttplugClient.Connected)
            return;
        if (scalar < 0.01)
            return;
        //Mod.LogInformation($"Vibrating at {scalar} time = {time}");
        foreach (var device in _buttplugClient.Devices) 
        {
            foreach (var actuatorType in device.MessageAttributes.ScalarCmd.Select(a => a.ActuatorType)) 
            {
                string actuatorName = ActuatorTypes[(int)actuatorType];
                try 
                {
                    if (Mod.Config.TriggerActuatorTypes.Contains(actuatorName)) {
                        switch (actuatorType) {
                            case ActuatorType.Vibrate:
                                await device.VibrateAsync(scalar);
                                break;
                            case ActuatorType.Rotate:
                                await device.RotateAsync(scalar, true);
                                break;
                            case ActuatorType.Oscillate:
                                await device.OscillateAsync(scalar);
                                break;
                            case ActuatorType.Constrict:
                            case ActuatorType.Inflate:
                            case ActuatorType.Position:
                            case ActuatorType.Unknown:
                            default:
                                Mod.LogInformation($"Got invalid actuator type of {actuatorType}");
                                break;
                        }
                    }
                }
                catch (Exception ex) 
                {
                    Mod.LogInformation(
                        $"Exception Raised Vibrate ScalarAsync: {actuatorName} | Exception: {ex.Message}");
                }
            }
        }

        _ = StopAllIn(time);
    }

    public async Task StopAll() 
    {
        if (!_buttplugClient.Connected)
            return;
        try 
        {
            await _buttplugClient.StopAllDevicesAsync(CancellationToken);
        }
        catch (Exception ex) 
        {
            Mod.LogInformation($"Expection Raised StopAll StopAllDevicesAsync | Exception: {ex.Message}");
        }
    }
}