using BackseatC.Binding.AST;
using BackseatC.CodeGeneration;
using DistIL.IR;
using Silverfly.Backend;
using Silverfly.Nodes;
using Silverfly.Nodes.Operators;

namespace BackseatC.Binding;

public partial class Binder
{
    private static BoundExpression? BindExpression(BodyCompilation context, AstNode node)
    {
        if (node is BinaryOperatorNode binary)
        {
            return BindBinaryExpression(context, binary);
        }

        if (node is CallNode call)
        {
            return BindCall(context, call);
        }

        if (node is LiteralNode literal)
        {
            return BindLiteral(context, literal);
        }

        return null;
    }

    private static BoundExpression? BindLiteral(BodyCompilation context, LiteralNode literal)
    {
        if (literal.Value is ulong uv) //Todo: remove when upgradet to newsest silvlerfly
        {
            return new BoundLiteral(ConstInt.CreateL((long)uv));
        }

        return new BoundLiteral(literal.ToConstant()!);
    }

    private static BoundExpression? BindBinaryExpression(BodyCompilation context, BinaryOperatorNode binary)
    {
        var left = BindExpression(context, binary.Left);
        var right = BindExpression(context, binary.Right);

        if (left == null || right == null)
        {
            binary.AddError("Failed to bind binary expression operands");
            return null;
        }

        return new BoundBinaryExpression(left, binary.Operator, right);
    }

    public static BoundStatement? Bind(BodyCompilation context, AstNode node)
    {
        if (node is BlockNode blk)
        {
            for (int i = 0; i < blk.Children.Count; i++)
            {
                blk.Children[i] = Bind(context, blk.Children[i])!;
            }

            return new BoundBlock(blk.Children);
        }

        return new BoundExpressionStatement(BindExpression(context, node));
    }
}