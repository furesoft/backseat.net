using DistIL.AsmIO;

namespace BackseatC.CodeGeneration.Scoping;

public abstract class ScopeItem
{
    public required string Name { get; init; }

    public bool IsMutable { get; init; }
    public abstract TypeDesc Type { get; }
}