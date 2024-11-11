using System.Text.Json;
using FishGoBrrr.Patches;
using GDWeave;

namespace FishGoBrrr;

public class Mod : IMod
{
    public static Config Config;
    public static IModInterface Interface;
    public static ButtClient ButtClient;
    public static Bridge Bridge;

    public static void LogInformation(object msg) => Interface.Logger.Information("FishGoBrrr: " + msg);

    public Mod(IModInterface modInterface)
    {
        Interface = modInterface;
        Config = modInterface.ReadConfig<Config>();

        ButtClient = new ButtClient();
        Bridge = new Bridge();
        
        Interface.RegisterScriptMod(new PlayerPatcher());
        Interface.RegisterScriptMod(new Fishing3Patcher());
        Interface.RegisterScriptMod(new ChalkCanvasPatcher());
        Interface.RegisterScriptMod(new ScratchTicketPatcher());
    }

    public void Dispose() { }
}