using GDWeave.Godot;
using GDWeave.Godot.Variants;
using GDWeave.Modding;

namespace FishGoBrrr.Patches;

public class PlayerPatcher : IScriptMod
{
    public bool ShouldRun(string path) => path == "res://Scenes/Entities/Player/player.gdc";

    public IEnumerable<Token> Modify(string path, IEnumerable<Token> tokens)
    {
        var fishingStartWaiter = new MultiTokenWaiter([
            t => t.Type == TokenType.Newline,
            t => t is IdentifierToken { Name: "failed_casts" },
            t => t.Type == TokenType.OpAssign,
            t => t is ConstantToken { Value: RealVariant { Value: 0d } },
            t => t.Type == TokenType.Newline
        ]);

        var jumpWaiter = new MultiTokenWaiter([
            t => t is IdentifierToken { Name: "request_jump" },
            t => t.Type == TokenType.OpAssign,
            t => t is ConstantToken { Value: BoolVariant { Value: false } },
            t => t.Type == TokenType.Newline,
            t => t is IdentifierToken { Name: "snap_vec" },
            t => t.Type == TokenType.OpAssign,
            t => t.Type == TokenType.BuiltInType,
            t => t.Type == TokenType.Period,
            t => t is IdentifierToken { Name: "ZERO" },
            t => t.Type == TokenType.Newline,
            t => t is Token { Type: TokenType.CfIf },
            t => t is IdentifierToken { Name: "is_on_floor" },
            t => t.Type == TokenType.ParenthesisOpen,
            t => t.Type == TokenType.ParenthesisClose,
            t => t.Type == TokenType.Colon,
            t => t.Type == TokenType.Newline
        ]);

        var mushroomWaiter = new MultiTokenWaiter([
            t => t.Type == TokenType.PrFunction,
            t => t is IdentifierToken { Name: "_mushroom_bounce" },
            t => t.Type == TokenType.ParenthesisOpen,
            t => t.Type == TokenType.ParenthesisClose,
            t => t.Type == TokenType.Colon,
            t => t.Type == TokenType.Newline,
            t => t.Type == TokenType.CfIf,
            t => t.Type == TokenType.OpNot,
            t => t is IdentifierToken { Name: "controlled" },
            t => t.Type == TokenType.Colon,
            t => t.Type == TokenType.CfReturn,
            t => t.Type == TokenType.Newline
        ], false, true);

        var punchedWaiter = new MultiTokenWaiter([
            t => t is IdentifierToken { Name: "bounce_vert" },
            t => t.Type == TokenType.OpAssign,
            t => t is ConstantToken,
            t => t.Type == TokenType.Newline,
            t => t.Type == TokenType.Newline,
            t => t is IdentifierToken { Name: "gravity_vec" },
            t => t.Type == TokenType.OpAssign,
            t => t.Type == TokenType.BuiltInType,
            t => t.Type == TokenType.Period,
            t => t is IdentifierToken { Name: "ZERO" },
            t => t.Type == TokenType.Newline
        ], false, true);
        
        var killWaiter = new MultiTokenWaiter([
            t => t.Type == TokenType.PrFunction,
            t => t is IdentifierToken { Name: "_kill" },
            t => t.Type == TokenType.ParenthesisOpen,
            t => t is IdentifierToken { Name: "skip_anim" },
            t => t.Type == TokenType.OpAssign,
            t => t.Type == TokenType.Constant,
            t => t.Type == TokenType.ParenthesisClose,
            t => t.Type == TokenType.Colon,
            t => t.Type == TokenType.Newline
        ]);
        
        foreach (var token in tokens)
        {
            if (fishingStartWaiter.Check(token))
            {
                yield return token;

                foreach (var t in Helpers.VibrateConstants(Mod.Config.BiteVibrate, Mod.Config.BiteDuration))
                    yield return t;
                yield return new Token(TokenType.Newline, 1);
            }
            else if (jumpWaiter.Check(token))
            {
                yield return token;

                foreach (var t in Helpers.VibrateConstants(Mod.Config.JumpVibrate, Mod.Config.JumpDuration))
                    yield return t;
                yield return new Token(TokenType.Newline, 3);
            }
            else if (mushroomWaiter.Check(token))
            {
                yield return token;

                foreach (var t in Helpers.VibrateConstants(Mod.Config.MushroomVibrate, Mod.Config.MushroomDuration))
                    yield return t;
                yield return new Token(TokenType.Newline, 1);
            }
            else if (punchedWaiter.Check(token))
            {
                yield return token;

                foreach (var t in Helpers.VibrateConstants(Mod.Config.PunchedVibrate, Mod.Config.PunchedDuration))
                    yield return t;
                yield return new Token(TokenType.Newline, 1);
            }
            else if (killWaiter.Check(token))
            {
                yield return token;

                foreach (var t in Helpers.VibrateConstants(Mod.Config.DieVibrate, Mod.Config.DieVibrate))
                    yield return t;
                yield return new Token(TokenType.Newline, 1);
            }
            else
            {
                yield return token;
            }
        }
    }

    public static void TestMethod()
    {
        Mod.LogInformation("Method got called!");
    }
}