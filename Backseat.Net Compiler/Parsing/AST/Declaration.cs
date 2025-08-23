using Silverfly.Nodes;

namespace Backseat.Net_Compiler.Parsing.AST;

public class Declaration : AstNode
{
    public List<Modifier> Modifiers {
        get => Properties.GetOrThrow<List<Modifier>>(nameof(Modifiers));
        set => Properties.Set(nameof(Modifiers), value);
    }
}