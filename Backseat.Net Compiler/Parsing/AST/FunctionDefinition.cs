using Silverfly.Nodes;

namespace Backseat.Net_Compiler.Parsing.AST;

public class FunctionDefinition : Declaration
{
    public FunctionDefinition(Signature signature, BlockNode? body)
    {
        Children.Add(signature);
        Children.Add(body);
    }

    public Signature Signature => (Signature)Children[0];
    public BlockNode? Body => (BlockNode?)Children[1];
}