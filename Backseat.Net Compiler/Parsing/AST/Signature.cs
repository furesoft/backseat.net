using Silverfly.Helpers;
using Silverfly.Nodes;

namespace Backseat.Net_Compiler.Parsing.AST;

public class Signature : AstNode
{
    public Signature(AstNode name, AstNode? returnType, List<ParameterDeclaration> parameters, List<AstNode> generics)
    {
        Children.Add(name);
        Children.Add(returnType);
        Children.Add(parameters);
    }

    public NameNode Name => (NameNode)Children[0];
    public TypeName? ReturnType => (TypeName)Children[1];
    public IEnumerable<ParameterDeclaration> Parameters => Children.OfType<ParameterDeclaration>();
}