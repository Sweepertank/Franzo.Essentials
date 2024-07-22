using Microsoft.Build.Framework;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Franzo.Essentials.InterfaceInheritance.MSBuild;

public class DevirtualizationTask : Microsoft.Build.Utilities.Task
{
    [Required]
    public string AssemblyPath { get; set; } = "";

    public override bool Execute()
    {
        var module = ModuleDefinition.ReadModule(AssemblyPath, new ReaderParameters()
        {
            InMemory = true,
            ReadSymbols = true
        });

        foreach (var type in module.GetTypes())
        {
            foreach (var property in type.Properties)
            {
                if (property.HasInterfaceInheritanceDevirtualizeAttribute())
                {
                    DevirtualizeMethod(property.GetMethod);
                    if (property.SetMethod is not null)
                    {
                        DevirtualizeMethod(property.SetMethod);
                    }
                }
            }

            foreach (var @event in type.Events)
            {
                if (@event.HasInterfaceInheritanceDevirtualizeAttribute())
                {
                    DevirtualizeMethod(@event.AddMethod);
                    DevirtualizeMethod(@event.RemoveMethod);
                }
            }

            foreach (var method in type.Methods)
            {
                if (method.HasInterfaceInheritanceDevirtualizeAttribute())
                {
                    DevirtualizeMethod(method);
                }
            }
        }

        module.Write(AssemblyPath, new WriterParameters()
        {
            WriteSymbols = true
        });

        module.Dispose();

        return true;
    }

    private void DevirtualizeMethod(MethodDefinition method)
    {
        if (method.Body is null)
        {
            return;
        }

        foreach (var instruction in method.Body.Instructions)
        {
            if (instruction.OpCode == OpCodes.Callvirt)
            {
                instruction.OpCode = OpCodes.Call;
            }
        }
    }
}
