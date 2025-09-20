using DistIL.AsmIO;
using DistIL.IR.Utils;
using Silverfly.Backend.Scoping;

namespace BackseatC.CodeGeneration;

public record BodyCompilation(Driver Context, MethodDef Method, IRBuilder Builder, Scope Scope)
{
    public object? Tag { get; set; }
}