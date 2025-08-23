using DistIL.AsmIO;

namespace Backseat.Net_Compiler.Binding;

public class KnownTypes(ModuleResolver resolver)
{
    public TypeDefOrSpec? ConsoleType { get; set; } = resolver.Import(typeof(Console));
    public TypeDefOrSpec? ExceptionType { get; set; } = resolver.Import(typeof(Exception));
    public TypeDefOrSpec? ArgumentNullExceptionType { get; set; } = resolver.Import(typeof(ArgumentNullException));
}