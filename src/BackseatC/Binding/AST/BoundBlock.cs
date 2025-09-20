using MrKWatkins.Ast;
using Silverfly.Nodes;

namespace BackseatC.Binding.AST;

public class BoundBlock : BoundStatement
{
    public BoundBlock(Children<AstNode> children)
    {
        foreach (var child in children)
        {
            Children.Add(child);
        }
    }
}