using Silverfly.Helpers;
using Silverfly.Nodes;

namespace BackseatC.Parsing.AST;

public class ParameterDeclaration : Declaration
{
    public ParameterDeclaration(AstNode type, string name, AstNode? defaultValue)
    {
        Properties.Set(nameof(Name), name);
        Properties.Set(nameof(DefaultValue), defaultValue);

        Children.Add(type);
    }

    public string Name => Properties.GetOrThrow<string>(nameof(Name));
    public AstNode? DefaultValue => Properties.GetOrDefault<AstNode?>(nameof(DefaultValue));

    public TypeName Type => (TypeName)Children[0];
}