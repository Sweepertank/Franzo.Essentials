using System.Diagnostics;
using Microsoft.Build.Framework;
using Mono.Cecil;

namespace Franzo.Essentials.MSBuild;

public class WeaveTask : Microsoft.Build.Utilities.Task
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
        var debuggerBrowsableAttributeConstructor = module.ImportReference(
            typeof(DebuggerBrowsableAttribute).GetConstructors().First());
        var debuggerBrowsableStateType = module.ImportReference(typeof(DebuggerBrowsableState));

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

                if (property.GetMethod.IsExplicitInterfaceMethodImplementation()
                    && !property.GetMethod.HasDebuggerBrowsableAttribute())
                {
                    var argument = new CustomAttributeArgument(debuggerBrowsableStateType, 0);
                    var attribute = new CustomAttribute(debuggerBrowsableAttributeConstructor);
                    attribute.ConstructorArguments.Add(argument);
                    property.CustomAttributes.Add(attribute);
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
