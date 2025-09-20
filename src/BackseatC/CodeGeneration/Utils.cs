using DistIL.AsmIO;
using DistIL.IR;
using Silverfly.Backend;
using Silverfly.Nodes;
using Silverfly.Nodes.Operators;

namespace BackseatC.CodeGeneration;

public static class Utils
{
    public static Value CreateValue(AstNode astNode, BodyCompilation context)
    {
        if (astNode is LiteralNode literal)
        {
            if (literal.Value is ulong uv) //Todo: remove when upgradet to newsest silvlerfly
            {
                return ConstInt.CreateL((long)uv);
            }

            return literal.ToConstant()!;
        }

        if (astNode is BinaryOperatorNode bin)
        {
            return CreateBinary(context, bin);
        }

        if (astNode is CallNode call)
        {
            if (CreateCall(context, call, out var value)) return value!;
        }

        return new Undef(PrimType.Void);
    }

    private static bool CreateCall(BodyCompilation context, CallNode call, out Value? value)
    {
        var args = call.Arguments.Select(arg => CreateValue(arg, context)).ToArray();

        if (CreateStaticContainingTypeCalls(context, call, args, out var staticCall))
        {
            context.Builder.Emit(staticCall!);
            value = staticCall!;
            return true;
        }

        if (CreatePrintCalls(context, call, args, out var printCall))
        {
            context.Builder.Emit(printCall!);
            value = printCall!;
            return true;
        }

        call.AddError($"Function {call.FunctionExpr} not found");
        value = null;
        return false;
    }

    private static bool CreateStaticContainingTypeCalls(BodyCompilation context, CallNode node, IEnumerable<Value> args, out Instruction? CallInstruction)
    {
        var candidates = GetStaticMethodCandidates(node, args, context.Builder.Method.Definition.DeclaringType);
        if (candidates.Length == 0)
        {
            CallInstruction = null;
            return false;
        }

        var method = candidates[0];
        CallInstruction = new CallInst(method, [.. args]);
        return true;
    }

    private static bool CreatePrintCalls(BodyCompilation context, CallNode node, IEnumerable<Value> args, out Instruction? CallInstruction)
    {
        if (node.FunctionExpr is not NameNode identifier)
        {
            CallInstruction = null;
            return false;
        }

        var callee = identifier.Token.Text.ToString();
        if (callee is "print")
        {
            var method = context.Context.Compilation.Module.Resolver.FindMethod("System.Console::Write", [.. args]);
            CallInstruction = new CallInst(method, [.. args]);
            return true;
        }

        if (callee is "println")
        {
            var method = context.Context.Compilation.Module.Resolver.FindMethod("System.Console::WriteLine", [.. args]);
            CallInstruction = new CallInst(method, [.. args]);
            return true;
        }

        CallInstruction = null;
        return false;
    }


    private static MethodDesc[] GetStaticMethodCandidates(CallNode node, IEnumerable<Value> args, TypeDesc type)
    {
        var callee = node.FunctionExpr as NameNode;
        return type.Methods
            .Where(m => m.Name == callee.Token.Text.ToString() && m.IsStatic)
            .Where(m => m.ParamSig.Count == node.Arguments.Count())
            .Where(m => m.ParamSig.Zip(
                args,
                (p, a) => p.Type.IsAssignableTo(a.ResultType)).All(x => x)).ToArray();
    }

    private static Value CreateBinary(BodyCompilation context, BinaryOperatorNode bin)
    {
        var left = CreateValue(bin.Left, context);
        var right = CreateValue(bin.Right, context);

        var op = bin.Operator.Text.ToString() switch
        {
            "+" => BinaryOp.Add,
            "-" => BinaryOp.Sub,
            "*" => BinaryOp.Mul,
            "/" => BinaryOp.FDiv,
            _ => throw new NotSupportedException($"Unsupported binary operator '{bin.Operator.Text}'")
        };

        return context.Builder.CreateBin(op, left, right);
    }
}