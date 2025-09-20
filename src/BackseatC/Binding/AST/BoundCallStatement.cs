using DistIL.AsmIO;
using Silverfly.Nodes;

namespace BackseatC.Binding.AST;

public class BoundCallStatement : BoundStatement
{
    public required MethodDesc Method { get; set; }
}