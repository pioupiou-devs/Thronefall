using System;
using System.Collections.Generic;
using System.Reflection;

namespace CityBuilder.EventBus
{
    /// <summary>
    /// Scans predefined Unity assemblies (Assembly-CSharp, etc.) for types implementing
    /// a given interface.  Used by <see cref="EventBusUtil"/> to discover all <see cref="IEvent"/>
    /// types at startup.
    /// </summary>
    public static class PredefinedAssemblyUtil
    {

        private const string PROJECT_NAME = "ThroneFall";

    enum AssemblyType
    {
        AssemblyCSharp,
        AssemblyCSharpEditor,
        AssemblyCSharpEditorFirstPass,
        AssemblyCSharpFirstPass
    }

    static AssemblyType? GetAssemblyType(string assemblyName)
    {
        return assemblyName switch
        {
            "Assembly-CSharp"                 => AssemblyType.AssemblyCSharp,
            "Assembly-CSharp-Editor"          => AssemblyType.AssemblyCSharpEditor,
            "Assembly-CSharp-Editor-firstpass" => AssemblyType.AssemblyCSharpEditorFirstPass,
            "Assembly-CSharp-firstpass"        => AssemblyType.AssemblyCSharpFirstPass,
            _ => null
        };
    }

    static void AddTypesFromAssembly(Type[] assemblyTypes, Type interfaceType, ICollection<Type> results)
    {
        if (assemblyTypes == null) return;
        foreach (var type in assemblyTypes)
        {
            if (type != interfaceType && interfaceType.IsAssignableFrom(type))
                results.Add(type);
        }
    }

    /// <summary>
    /// Returns all types in the current AppDomain's predefined assemblies that implement
    /// <paramref name="interfaceType"/>.  Also scans assemblies whose name starts with
    /// "<ProjectName>.*" so our asmdef-based assemblies are covered.
    /// </summary>
    public static List<Type> GetTypes(Type interfaceType)
    {
        var assemblies     = AppDomain.CurrentDomain.GetAssemblies();
        var assemblyTypes  = new Dictionary<AssemblyType, Type[]>();
        var results        = new List<Type>();

        foreach (var asm in assemblies)
        {
            string name = asm.GetName().Name;

            // Predefined Unity assemblies
            var at = GetAssemblyType(name);
            if (at != null)
            {
                assemblyTypes[(AssemblyType)at] = asm.GetTypes();
                continue;
            }

            // Our own asmdef-based assemblies (<ProjectName>.Core, <ProjectName>.NPC, …)
            if (name.StartsWith(PROJECT_NAME+".", StringComparison.Ordinal))
            {
                AddTypesFromAssembly(asm.GetTypes(), interfaceType, results);
            }
        }

        if (assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharp, out var cs))
            AddTypesFromAssembly(cs, interfaceType, results);
        if (assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharpFirstPass, out var fp))
            AddTypesFromAssembly(fp, interfaceType, results);

        return results;
    }
}
}
