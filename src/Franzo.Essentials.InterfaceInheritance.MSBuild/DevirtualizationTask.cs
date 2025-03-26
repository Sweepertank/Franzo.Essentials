using Microsoft.Build.Framework;
using Mono.Cecil;

namespace Franzo.Essentials.InterfaceInheritance.MSBuild;

public class DevirtualizationTask : Microsoft.Build.Utilities.Task
{
    [Required]
    public string AssemblyPath { get; set; } = "";

    [Required]
    public string ReferencePath { get; set; } = "";

    public override bool Execute()
    {
        var referencePaths = ReferencePath.Split([';'], StringSplitOptions.RemoveEmptyEntries);
        using var assemblyResolver = new AssemblyResolver(this, referencePaths);
        using var module = ModuleDefinition.ReadModule(AssemblyPath, new ReaderParameters()
        {
            InMemory = true,
            ReadSymbols = true,
            AssemblyResolver = assemblyResolver
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

        return true;
    }

    private void DevirtualizeMethod(MethodDefinition method)
    {
        method.Attributes &= ~(MethodAttributes.Final | MethodAttributes.NewSlot | MethodAttributes.Virtual);
    }
}
