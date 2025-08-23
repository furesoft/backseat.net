using System.Reflection;
using Backseat.Net_Compiler.Binding;
using Backseat.Net_Compiler.Binding.Listeners.Body;
using Backseat.Net_Compiler.Binding.Scoping;
using Backseat.Net_Compiler.Parsing;
using DistIL.AsmIO;
using DistIL.IR;
using DistIL.IR.Utils;
using LanguageSdk.Templates.Core;
using Silverfly;
using Silverfly.Text;
using Binder = Backseat.Net_Compiler.Binding.Binder;
using MethodBody = DistIL.IR.MethodBody;

namespace Backseat.Net_Compiler;

class Program
{
    static void Main(string[] args)
    {
        var parser = new BackseatParser();
        var ast = parser.Parse(SourceDocument.Create("source.bs"));

        Bind(ast);
    }

    private static void Bind(TranslationUnit tree)
    {
        var bindingSettings = new DriverSettings
        {
            DebugSymbols = false,
            IsDebug = false,
            OptimizeLevel = "O3",
            Sources = ["source.bs"],
            OutputPath = "test.dll",
            Version = "1.0",
            RootNamespace = "Test"
        };
        var bindingContext = BindingContext.Create(bindingSettings);
        bindingContext.Trees = [tree];
        var mainMethod = bindingContext.Compilation.GetAuxType()
            .CreateMethod("Main", new TypeSig(PrimType.Void), [],
                MethodAttributes.Public | MethodAttributes.Static);
        bindingContext.Compilation.Module.EntryPoint = mainMethod;
        mainMethod.Body = new MethodBody(mainMethod);

        var mainBuilder = new IRBuilder(mainMethod.Body.CreateBlock());

        foreach (var tu in bindingContext.Trees)
        {
           // Binder.Listener.Listen(bindingContext, tu.Tree);
           BodyCompilation.Listener.Listen(new(bindingContext, mainMethod, mainBuilder, new Scope(null)), tu.Tree);
        }

        if (mainMethod.Body.EntryBlock.Last is not ReturnInst)
        {
            mainBuilder.CreateReturn();
        }

        mainMethod.ILBody = DistIL.CodeGen.Cil.ILGenerator.GenerateCode(mainMethod.Body);

        bindingContext.Compilation.Module.Save("test.dll", false);

        //bindingContext.Optimizer.Run(Mappings.Functions.Values);
    }
}