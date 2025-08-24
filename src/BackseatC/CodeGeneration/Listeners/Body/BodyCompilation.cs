using DistIL.AsmIO;
using DistIL.IR.Utils;
using MrKWatkins.Ast.Listening;
using Silverfly.Backend.Scoping;
using Silverfly.Nodes;

namespace BackseatC.CodeGeneration.Listeners.Body;

public record BodyCompilation(Driver Context, MethodDef Method, IRBuilder Builder, Scope Scope)
{
    public static readonly CompositeListener<BodyCompilation, AstNode> Listener =
        CompositeListener<BodyCompilation, AstNode>.Build()
            .With(new CallExpressionListener(true))
            .ToListener();

    public object? Tag { get; set; }
}