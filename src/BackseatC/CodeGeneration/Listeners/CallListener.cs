using BackseatC.CodeGeneration.Listeners.Body;
using DistIL.AsmIO;
using DistIL.IR;
using MrKWatkins.Ast.Listening;
using Silverfly.Nodes;

namespace BackseatC.CodeGeneration.Listeners;

public class CallExpressionListener(bool shouldEmit) : Listener<BodyCompilation, AstNode, CallNode>
{
    public Instruction CallInstruction;

    protected override void ListenToNode(BodyCompilation context, CallNode node)
    {
        var args = node.Arguments.Select(arg => Utils.CreateValue(arg, context)).ToArray();

        if (CreateStaticContainingTypeCalls(context, node, args))
        {
            return;
        }

        if (CreatePrintCalls(context, node, args))
        {
            return;
        }

        node.AddError($"Function {node.FunctionExpr} not found");
    }

    protected override void AfterListenToNode(BodyCompilation context, CallNode node)
    {
        if (shouldEmit && CallInstruction.Block == null)
        {
            context.Builder.Emit(CallInstruction);
        }
    }

    private bool CreateStaticContainingTypeCalls(BodyCompilation context, CallNode node, IEnumerable<Value> args)
    {
        var candidates = GetStaticMethodCandidates(node, args, context.Builder.Method.Definition.DeclaringType);
        if (candidates.Length == 0)
        {
            return false;
        }

        var method = candidates[0];
        CallInstruction = new CallInst(method, [.. args]);
        return true;
    }

    private bool CreatePrintCalls(BodyCompilation context, CallNode node, IEnumerable<Value> args)
    {
        if (node.FunctionExpr is not NameNode identifier)
        {
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

    protected override bool ShouldListenToChildren(BodyCompilation context, CallNode node)
    {
        return false;
    }
}