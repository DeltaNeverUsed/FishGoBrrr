namespace FishGoBrrr;

public enum ButtAction
{
    Vibrate,
    StopAll,
    Hover,
    Click,
}

[Serializable]
public class BridgeMessage
{
    public ButtAction Action { get; set; }
    public double Value { get; set; }
    public double Duration { get; set; }
}