using System.Runtime.Versioning;
using DistIL;
using DistIL.AsmIO;
using LanguageSdk.Templates.Core;
using Silverfly;
using Silverfly.Text;

namespace Backseat.Net_Compiler.Binding;

public sealed class BindingContext
{
    public required Compilation Compilation { get; set; }

    public required KnownAttributes KnownAttributes { get; set; }
    public required KnownTypes KnownTypes { get; set; }

    public Optimizer Optimizer { get; set; }
    public List<Message> Messages { get; set; } = [];
    public List<TranslationUnit> Trees { get; set; } = [];
    public DriverSettings Settings { get; set; }

    public static BindingContext Create(DriverSettings settings)
    {
        var moduleResolver = new ModuleResolver();
        moduleResolver.AddTrustedSearchPaths();

        var module = moduleResolver.Create(settings.RootNamespace, Version.Parse("1.0"));
        SetAttributes(module, moduleResolver);

        var compilation = new Compilation(module, new ConsoleLogger(), new CompilationSettings());
        var optimizer = new Optimizer();
        optimizer.DefineDefaults();
        optimizer.CreatePassManager(compilation, settings);

        return new BindingContext()
        {
            Compilation = compilation,
            Settings = settings,
            Optimizer = optimizer,
            KnownAttributes = new KnownAttributes(moduleResolver),
            KnownTypes = new KnownTypes(moduleResolver)
        };
    }

    private static void SetAttributes(ModuleDef module, ModuleResolver moduleResolver)
    {
        var targetFramework = new CustomAttrib(moduleResolver.Import(typeof(TargetFrameworkAttribute))
            .FindMethod(".ctor", new MethodSig(default, [PrimType.String])), [".NETCoreApp,Version=v9.0"]);

        module.GetCustomAttribs(true).Add(targetFramework);
    }
}