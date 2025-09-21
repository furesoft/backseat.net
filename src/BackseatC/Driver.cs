using System.Reflection;
using System.Runtime.Versioning;
using BackseatC.CodeGeneration;
using BackseatC.Parsing;
using DistIL;
using DistIL.AsmIO;
using DistIL.IR;
using DistIL.IR.Utils;
using Silverfly.Text;
using LanguageSdk.Templates.Core;
using Silverfly;
using Silverfly.Backend.Scoping;
using Silverfly.Nodes;
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
        if(string.IsNullOrEmpty(settings.RootNamespace))
        {
            settings.RootNamespace = "Test";
        }

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
        var parsers = new List<BackseatParser>();
        foreach (var source in Settings.Sources)
        {
            var document = SourceDocument.Create(source);
            var parser = new BackseatParser();
            parsers.Add(parser);

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
            var context = new BodyCompilation(this, mainMethod, mainBuilder, new Scope(null));
            foreach (var node in ((BlockNode)tu.Tree).Children)
            {
                Utils.CreateValue(node, context);
            }
        }

        if (mainMethod.Body.EntryBlock.Last is not ReturnInst)
        {
            mainBuilder.CreateReturn();
        }

        mainMethod.ILBody = DistIL.CodeGen.Cil.ILGenerator.GenerateCode(mainMethod.Body);

        Optimizer.Run(Mappings.Functions.Values);

        foreach (var parser in parsers)
        {
            parser.PrintMessages();
        }

        if (!parsers.Any(_ => _.Document.HasErrors))
        {
            Compilation.Module.Save("test.dll", false);
            Environment.Exit(0);
        }

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