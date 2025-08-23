using DistIL.AsmIO;

namespace Backseat.Net_Compiler.Binding.Scoping;

public abstract class ScopeItem
{
    public required string Name { get; init; }

    public bool IsMutable { get; init; }
    public abstract TypeDesc Type { get; }
}