﻿using DistIL.AsmIO;
using DistIL.IR;

namespace Backseat.Net_Compiler.Binding.Scoping.Items;

public class VariableScopeItem : ScopeItem
{
    public LocalSlot Slot { get; set; }

    public override TypeDesc Type => Slot.Type;

    public void Deconstruct(out string name, out bool isMutable)
    {
        name = Name;
        isMutable = IsMutable;
    }
}