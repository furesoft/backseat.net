using MrKWatkins.Ast.Listening;
using Silverfly.Nodes;

namespace Backseat.Net_Compiler.Binding.Listeners;

public class BlockListener : Listener<BindingContext, AstNode, BlockNode>
{
    protected override void ListenToNode(BindingContext context, BlockNode node)
    {
        base.ListenToNode(context, node);
    }
}