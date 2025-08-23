using Backseat.Net_Compiler.Binding.Scoping;
using DistIL.AsmIO;
using DistIL.IR.Utils;
using MrKWatkins.Ast.Listening;
using Silverfly.Nodes;

namespace Backseat.Net_Compiler.Binding.Listeners.Body;

public record BodyCompilation(BindingContext Context, MethodDef Method, IRBuilder Builder, Scope Scope)
{
    public static readonly CompositeListener<BodyCompilation, AstNode> Listener =
        CompositeListener<BodyCompilation, AstNode>.Build()
            .With(new CallExpressionListener(true))
            .ToListener();

    public object? Tag { get; set; }
}