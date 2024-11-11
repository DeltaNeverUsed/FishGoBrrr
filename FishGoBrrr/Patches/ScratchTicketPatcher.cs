using GDWeave.Godot;
using GDWeave.Godot.Variants;
using GDWeave.Modding;

namespace FishGoBrrr.Patches;

public class ScratchTicketPatcher : IScriptMod
{
    public bool ShouldRun(string path) => path == "res://Scenes/Minigames/ScratchTicket/scratch_ticket.gdc";

    public IEnumerable<Token> Modify(string path, IEnumerable<Token> tokens)
    {
        var physicsProcessWaiter = new MultiTokenWaiter([
            t => t.Type == TokenType.PrFunction,
            t => t is IdentifierToken { Name: "_physics_process" },
            t => t.Type == TokenType.ParenthesisOpen,
            t => t is IdentifierToken { Name: "delta" },
            t => t.Type == TokenType.ParenthesisClose,
            t => t.Type == TokenType.Colon,
            t => t.Type == TokenType.Newline
        ]);

        var winWaiter = new MultiTokenWaiter([
            t => t.Type == TokenType.ParenthesisOpen,
            t => t is ConstantToken { Value: StringVariant { Value: "jingle_win" } },
            t => t.Type == TokenType.ParenthesisClose,
        ]);
        var loseWaiter = new MultiTokenWaiter([
            t => t.Type == TokenType.ParenthesisOpen,
            t => t is ConstantToken { Value: StringVariant { Value: "jingle_lose" } },
            t => t.Type == TokenType.ParenthesisClose,
        ]);

        foreach (var token in tokens)
        {
            if (physicsProcessWaiter.Check(token))
            {
                yield return token;

                foreach (var t in Helpers.GetMain())
                    yield return t;
                yield return new Token(TokenType.Period);
                yield return new IdentifierToken("continues_vibrate");
                yield return new Token(TokenType.ParenthesisOpen);

                yield return new IdentifierToken("scratching");
                yield return new Token(TokenType.OpMul);
                yield return new ConstantToken(new RealVariant(Mod.Config.ScratchGamblingVibrate));

                yield return new Token(TokenType.ParenthesisClose);

                yield return new Token(TokenType.Newline, 1);
            }
            else if (winWaiter.Check(token))
            {
                yield return token;

                yield return new Token(TokenType.Newline, 3);
                foreach (var t in Helpers.VibrateConstants(Mod.Config.WinGamblingVibrate,
                             Mod.Config.WinGamblingDuration))
                    yield return t;
            }
            else if (loseWaiter.Check(token))
            {
                yield return token;

                yield return new Token(TokenType.Newline, 3);
                foreach (var t in Helpers.VibrateConstants(Mod.Config.LoseGamblingVibrate,
                             Mod.Config.LoseGamblingDuration))
                    yield return t;
            }
            else
            {
                yield return token;
            }
        }
    }
}