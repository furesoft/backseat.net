using Backseat.Net_Compiler.Binding.Listeners;
using MrKWatkins.Ast.Listening;
using Silverfly.Nodes;

namespace Backseat.Net_Compiler.Binding;

public class Binder
{
    public static CompositeListener<BindingContext, AstNode> Listener = CompositeListener<BindingContext, AstNode>
        .Build()
        .With(new BlockListener())

        .ToListener();

}