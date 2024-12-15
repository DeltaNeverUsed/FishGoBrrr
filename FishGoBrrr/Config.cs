using Buttplug;
using System.Text.Json.Serialization;

namespace FishGoBrrr;

public class Config
{
    [JsonInclude] public string URL = "ws://127.0.0.1:12345";

    [JsonInclude] public double UiHoverVibrate = 0.075d;
    [JsonInclude] public double UiHoverDuration = 0.075d;

    [JsonInclude] public double UiClickVibrate = 0.1d;
    [JsonInclude] public double UiClickDuration = 0.1d;

    [JsonInclude] public double JumpVibrate = 0.1d;
    [JsonInclude] public double JumpDuration = 0.1d;

    [JsonInclude] public double MushroomVibrate = 0.5d;
    [JsonInclude] public double MushroomDuration = 0.2d;

    [JsonInclude] public double DieVibrate = 0.5d;
    [JsonInclude] public double DieDuration = 0.2d;

    [JsonInclude] public double ChalkVibrate = 0.01d;

    [JsonInclude] public double PunchedVibrate = 0.5d;
    [JsonInclude] public double PunchedDuration = 0.2d;

    [JsonInclude] public double ScratchGamblingVibrate = 0.002d;

    [JsonInclude] public double WinGamblingVibrate = 0.5d;
    [JsonInclude] public double WinGamblingDuration = 0.2d;

    [JsonInclude] public double LoseGamblingVibrate = 0.1d;
    [JsonInclude] public double LoseGamblingDuration = 0.3d;

    [JsonInclude] public double BiteVibrate = 0.5d;
    [JsonInclude] public double BiteDuration = 0.3d;

    [JsonInclude] public double MashVibrate = 0.1d;

    [JsonInclude] public double BadProgressVibrate = 0.005d;

    [JsonInclude] public string[] TriggerActuatorTypes = Enum.GetNames(typeof(ActuatorType));
}