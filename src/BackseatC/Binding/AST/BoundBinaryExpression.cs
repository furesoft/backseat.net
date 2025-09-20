using Silverfly;

namespace BackseatC.Binding.AST;

public class BoundBinaryExpression : BoundExpression
{
    public BoundBinaryExpression(BoundExpression leftExpr, Token @operator, BoundExpression rightExpr)
    {
        Properties.Set(nameof(Operator), @operator);
        Children.Add(leftExpr);
        Children.Add(rightExpr);
    }

    public Token Operator => Properties.GetOrThrow<Token>(nameof(Operator));

    public BoundExpression Left => (BoundExpression)Children.First;

    public BoundExpression Right => (BoundExpression)Children.Last;
}