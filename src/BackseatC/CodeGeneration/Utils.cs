using BackseatC.CodeGeneration.Listeners.Body;
using DistIL.AsmIO;
using DistIL.IR;
using Silverfly.Backend;
using Silverfly.Nodes;

namespace BackseatC.CodeGeneration;

public static class Utils
{
    public static Value CreateValue(AstNode astNode, BodyCompilation context)
    {
        if (astNode is LiteralNode literal)
        {
            return literal.ToConstant()!;
        }

        return new Undef(PrimType.Void);
    }
}