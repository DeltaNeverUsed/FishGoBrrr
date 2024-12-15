using GDWeave.Godot;
using GDWeave.Godot.Variants;
using GDWeave.Modding;

namespace FishGoBrrr.Patches;

public class ChalkCanvasPatcher : IScriptMod
{
    public bool ShouldRun(string path) => path == "res://Scenes/Entities/ChalkCanvas/chalk_canvas.gdc";

    public IEnumerable<Token> Modify(string path, IEnumerable<Token> tokens)
    {
        var addBrushWaiter = new MultiTokenWaiter([
            t => t.Type == TokenType.PrFunction,
            t => t is IdentifierToken { Name: "_add_brush" },
            t => t.Type == TokenType.ParenthesisOpen,
            t => t is IdentifierToken { Name: "grid_pos" },
            t => t.Type == TokenType.Comma,
            t => t is IdentifierToken { Name: "type" },
            t => t.Type == TokenType.Comma,
            t => t is IdentifierToken { Name: "from" },
            t => t.Type == TokenType.ParenthesisClose,
            t => t.Type == TokenType.Colon,
            t => t.Type == TokenType.Newline
        ]);

        foreach (var token in tokens)
        {
            if (addBrushWaiter.Check(token))
            {
                yield return token;

                foreach (var t in Helpers.GetMain())
                    yield return t;
                yield return new Token(TokenType.Period);
                yield return new IdentifierToken("continues_vibrate");
                yield return new Token(TokenType.ParenthesisOpen);

                yield return new IdentifierToken("from");
                yield return new Token(TokenType.Period);
                yield return new IdentifierToken("distance_to");
                yield return new Token(TokenType.ParenthesisOpen);
                yield return new IdentifierToken("grid_pos");
                yield return new Token(TokenType.ParenthesisClose);

                yield return new Token(TokenType.OpMul);

                yield return new ConstantToken(new RealVariant(Mod.Config.ChalkVibrate));

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