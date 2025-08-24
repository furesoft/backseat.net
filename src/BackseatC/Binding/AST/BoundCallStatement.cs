using DistIL.AsmIO;
using Silverfly.Nodes;

namespace BackseatC.Binding.AST;

public class BoundCallStatement : AstNode
{
    public required MethodDesc Method { get; set; }
}