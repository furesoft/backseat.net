using Backseat.Net_Compiler.Binding.Listeners.Body;
using DistIL.AsmIO;
using DistIL.IR;
using Silverfly.Backend;
using Silverfly.Nodes;

namespace Backseat.Net_Compiler.Binding;

public static class Utils
{
    public static Value CreateValue(AstNode astNode, BodyCompilation context)
    {
        if (astNode is LiteralNode literal)
        {
            return literal.ToConstant();
        }

        return new Undef(PrimType.Void);
    }
}