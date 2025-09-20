namespace BackseatC.Binding.AST;

public class BoundExpressionStatement : BoundStatement
{
    public BoundExpression? Expression => (BoundExpression)Children.First;

    public BoundExpressionStatement(BoundExpression? expression)
    {
        Children.Add(expression!);
    }
}