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
            foreach (var field in type.Fields)
            {
                if (field.IsPossiblyJrifConstructTyped())
                {
                    MakeDebuggerUnbrowsable(
                        field,
                        debuggerBrowsableAttributeConstructor,
                        debuggerBrowsableStateType);
                }
            }

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

                if ((property.GetMethod.IsExplicitInterfaceMethodImplementation()
                     && !property.GetMethod.HasDebuggerBrowsableAttribute())
                    || property.IsPossiblyJrifConstructTyped())
                {
                    MakeDebuggerUnbrowsable(
                        property,
                        debuggerBrowsableAttributeConstructor,
                        debuggerBrowsableStateType);
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

    private static void MakeDebuggerUnbrowsable(
        IMemberDefinition member,
        MethodReference attributeConstructor,
        TypeReference stateType)
    {
        var argument = new CustomAttributeArgument(stateType, 0);
        var attribute = new CustomAttribute(attributeConstructor);
        attribute.ConstructorArguments.Add(argument);
        member.CustomAttributes.Add(attribute);
    }

    private static void DevirtualizeMethod(MethodDefinition method)
    {
        method.Attributes &= ~(MethodAttributes.Final | MethodAttributes.NewSlot | MethodAttributes.Virtual);
    }
}
