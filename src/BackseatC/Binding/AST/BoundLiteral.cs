using DistIL.IR;

namespace BackseatC.Binding.AST;

public class BoundLiteral : BoundExpression
{
    public BoundLiteral(Value value)
    {
        Properties.GetOrAdd(nameof(Value), s => value);
    }

    public Value Value => Properties.GetOrThrow<Value>(nameof(Value));
}