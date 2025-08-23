﻿using DistIL.AsmIO;

namespace Backseat.Net_Compiler.Binding;

public class KnownAttributes(ModuleResolver resolver)
{
    public CustomAttrib GetAttribute<T>(params object[] args)
        where T : Attribute
    {
        var type = typeof(T);
        var importedType = resolver.Import(type);
        var ctor = importedType.FindMethod(".ctor");
        var customAttrib = new CustomAttrib(ctor, [.. args]);

        return customAttrib;
    }
}