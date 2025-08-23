using Backseat.Net_Compiler.Parsing.AST;
using DistIL.AsmIO;

namespace Backseat.Net_Compiler;

public static class Mappings
{
    public static readonly Dictionary<FunctionDefinition, MethodDef> Functions = [];
   // public static readonly Dictionary<ClassDeclaration, TypeDef> Types = [];
}