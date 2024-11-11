using GDWeave.Godot;
using GDWeave.Godot.Variants;

namespace FishGoBrrr;

public static class Helpers
{
    public static IEnumerable<Token> VibrateConstants(double scaler, double duration)
    {
        foreach (var token in GetMain()) yield return token;
        yield return new Token(TokenType.Period);
        yield return new IdentifierToken("vibrate");
        yield return new Token(TokenType.ParenthesisOpen);
        yield return new ConstantToken(new RealVariant(scaler));
        yield return new Token(TokenType.Comma);
        yield return new ConstantToken(new RealVariant(duration));
        yield return new Token(TokenType.ParenthesisClose);
    }
    
    public static IEnumerable<Token> GetMain()
    {
        yield return new IdentifierToken("get_node");
        yield return new Token(TokenType.ParenthesisOpen);
        yield return new ConstantToken(new StringVariant("/root/deltaneverusedfishgobrrr"));
        yield return new Token(TokenType.ParenthesisClose);
    }
}