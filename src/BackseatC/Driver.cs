using System.Reflection;
using System.Runtime.Versioning;
using BackseatC.CodeGeneration;
using BackseatC.CodeGeneration.Listeners.Body;
using BackseatC.CodeGeneration.Scoping;
using BackseatC.Parsing;
using DistIL;
using DistIL.AsmIO;
using DistIL.IR;
using DistIL.IR.Utils;
using Silverfly.Text;
using LanguageSdk.Templates.Core;
using Silverfly;
using MethodBody = DistIL.IR.MethodBody;

namespace BackseatC;

public class Driver
{
    public DriverSettings Settings { get; set; } = new();
    public required Compilation Compilation { get; set; }

    public required Optimizer Optimizer { get; set; }

    public required KnownAttributes KnownAttributes { get; set; }
    public required KnownTypes KnownTypes { get; set; }

    public List<Message> Messages { get; set; } = [];
    public List<TranslationUnit> Trees { get; set; } = [];

    public static Driver Create(DriverSettings settings)
    {
        var moduleResolver = new ModuleResolver();
        moduleResolver.AddTrustedSearchPaths();

        var module = moduleResolver.Create(settings.RootNamespace, Version.Parse(settings.Version));
        SetAttributes(module, moduleResolver);

        var compilation = new Compilation(module, new ConsoleLogger(), new CompilationSettings());
        var optimizer = new Optimizer();
        optimizer.DefineDefaults();
        optimizer.CreatePassManager(compilation, settings);

        return new Driver
        {
            Compilation = compilation,
            Settings = settings,
            Optimizer = optimizer,
            KnownAttributes = new KnownAttributes(moduleResolver),
            KnownTypes = new KnownTypes(moduleResolver)
        };
    }

    public SourceDocument[] Compile()
    {
        foreach (var source in Settings.Sources)
        {
            var document = SourceDocument.Create(source);
            var parser = new BackseatParser();

            Trees.Add(parser.Parse(document));
        }

        var mainMethod = Compilation.GetAuxType()
            .CreateMethod("Main", new TypeSig(PrimType.Void), [],
                MethodAttributes.Public | MethodAttributes.Static);
        Compilation.Module.EntryPoint = mainMethod;
        mainMethod.Body = new MethodBody(mainMethod);

        var mainBuilder = new IRBuilder(mainMethod.Body.CreateBlock());

        foreach (var tu in Trees)
        {
            // Binder.Listener.Listen(bindingContext, tu.Tree);
            BodyCompilation.Listener.Listen(new(this, mainMethod, mainBuilder, new Scope(null)), tu.Tree);
        }

        if (mainMethod.Body.EntryBlock.Last is not ReturnInst)
        {
            mainBuilder.CreateReturn();
        }

        mainMethod.ILBody = DistIL.CodeGen.Cil.ILGenerator.GenerateCode(mainMethod.Body);

        Compilation.Module.Save("test.dll", false);

        //bindingContext.Optimizer.Run(Mappings.Functions.Values);
        return Trees
            .Select(_ => _.Document)
            .ToArray();
    }

    private static void SetAttributes(ModuleDef module, ModuleResolver moduleResolver)
    {
        var targetFramework = new CustomAttrib(moduleResolver.Import(typeof(TargetFrameworkAttribute))
            .FindMethod(".ctor", new MethodSig(default, [PrimType.String])), [".NETCoreApp,Version=v9.0"]);

        module.GetCustomAttribs(true).Add(targetFramework);
    }
}