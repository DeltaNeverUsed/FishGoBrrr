using GDWeave.Godot;
using GDWeave.Godot.Variants;
using GDWeave.Modding;

namespace FishGoBrrr.Patches;

public class Fishing3Patcher : IScriptMod
{
    public bool ShouldRun(string path) => path == "res://Scenes/Minigames/Fishing3/fishing3.gdc";

    public IEnumerable<Token> Modify(string path, IEnumerable<Token> tokens)
    {
        var ysHealthWaiter = new MultiTokenWaiter([
            t => t.Type == TokenType.OpAssignSub,
            t => t is IdentifierToken { Name: "params" },
            t => t.Type == TokenType.BracketOpen,
            t => t is ConstantToken { Value: StringVariant { Value: "damage" } },
            t => t.Type == TokenType.BracketClose,
            t => t.Type == TokenType.Newline,
        ]);

        var badProgressWaiter = new MultiTokenWaiter([
            t => t is IdentifierToken { Name: "value" },
            t => t.Type == TokenType.OpAssign,
            t => t is IdentifierToken { Name: "bad_progress" },
            t => t.Type == TokenType.Newline,
        ]);

        var reachedEndWaiter = new MultiTokenWaiter([
            t => t.Type == TokenType.PrFunction,
            t => t is IdentifierToken { Name: "_reached_end" },
            t => t.Type == TokenType.ParenthesisOpen,
            t => t is IdentifierToken { Name: "win" },
            t => t.Type == TokenType.ParenthesisClose,
            t => t.Type == TokenType.Colon,
            t => t.Type == TokenType.Newline,
        ]);

        foreach (var token in tokens)
        {
            if (ysHealthWaiter.Check(token))
            {
                yield return token;

                foreach (var t in Helpers.GetMain())
                    yield return t;
                yield return new Token(TokenType.Period);
                yield return new IdentifierToken("continues_vibrate");
                yield return new Token(TokenType.ParenthesisOpen);
                yield return new ConstantToken(new RealVariant(Mod.Config.MashVibrate));
                yield return new Token(TokenType.ParenthesisClose);
                yield return new Token(TokenType.Newline, 1);
            }
            else if (badProgressWaiter.Check(token))
            {
                yield return token;

                yield return new Token(TokenType.CfIf);
                yield return new Token(TokenType.OpNot);
                yield return new IdentifierToken("over");
                yield return new Token(TokenType.Colon);

                foreach (var t in Helpers.GetMain())
                    yield return t;
                yield return new Token(TokenType.Period);
                yield return new IdentifierToken("vibrate");
                yield return new Token(TokenType.ParenthesisOpen);
                yield return new IdentifierToken("bad_progress");
                yield return new Token(TokenType.OpMul);
                yield return new ConstantToken(new RealVariant(Mod.Config.BadProgressVibrate));
                yield return new Token(TokenType.Comma);
                yield return new ConstantToken(new RealVariant(-1));
                yield return new Token(TokenType.ParenthesisClose);
                yield return new Token(TokenType.Newline, 1);
            }
            else if (reachedEndWaiter.Check(token))
            {
                yield return token;

                foreach (var t in Helpers.GetMain())
                    yield return t;
                yield return new Token(TokenType.Period);
                yield return new IdentifierToken("stop");
                yield return new Token(TokenType.ParenthesisOpen);
                yield return new Token(TokenType.ParenthesisClose);
                yield return new Token(TokenType.Newline, 1);
            }
            else
            {
                yield return token;
            }
        }
    }
}