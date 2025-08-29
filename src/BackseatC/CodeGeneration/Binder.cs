using BackseatC.CodeGeneration.Listeners;
using MrKWatkins.Ast.Listening;
using Silverfly.Nodes;

namespace BackseatC.CodeGeneration;

public class Binder
{
    public static CompositeListener<Driver, AstNode> Listener = CompositeListener<Driver, AstNode>
        .Build()
        .With(new BlockListener())

        .ToListener();
}