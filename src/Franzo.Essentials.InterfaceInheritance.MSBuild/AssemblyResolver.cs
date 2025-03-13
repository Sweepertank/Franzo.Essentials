using Mono.Cecil;

namespace Franzo.Essentials.InterfaceInheritance.MSBuild;

internal class AssemblyResolver : DefaultAssemblyResolver
{
    private readonly Dictionary<string, string> _assemblyPathsBySimpleAssemblyName =
        new(StringComparer.InvariantCultureIgnoreCase);
    private readonly Dictionary<string, AssemblyDefinition> _loadedAssembliesByPath =
        new(StringComparer.InvariantCultureIgnoreCase);

    public DevirtualizationTask Task { get; }

    public AssemblyResolver(DevirtualizationTask task, string[] referencePaths)
    {
        Task = task;

        foreach (var referencePath in referencePaths)
        {
            _assemblyPathsBySimpleAssemblyName[Path.GetFileNameWithoutExtension(referencePath)] =
                referencePath;
        }
    }

    public override AssemblyDefinition Resolve(
        AssemblyNameReference name,
        ReaderParameters parameters)
    {
        if (!_assemblyPathsBySimpleAssemblyName.TryGetValue(name.Name, out var assemblyPath))
        {
            return base.Resolve(name, parameters);
        }
        if (!_loadedAssembliesByPath.TryGetValue(assemblyPath, out var assembly))
        {
            parameters.AssemblyResolver ??= this;
            assembly = AssemblyDefinition.ReadAssembly(assemblyPath, parameters);
            _loadedAssembliesByPath[assemblyPath] = assembly;
        }

        return assembly;
    }

    protected override void Dispose(bool disposing)
    {
        foreach (var assembly in _loadedAssembliesByPath.Values)
        {
            assembly.Dispose();
        }

        base.Dispose(disposing);
    }
}
