#region

using Common;
using Common.ProjectilePaths;
using Common.Resources.World;
using Common.Utilities;
using GameServer.Game.DamageSources.Projectiles;
using GameServer.Game.Entities.Behaviors.Actions;
using GameServer.Game.Entities.Behaviors.Classic;
using GameServer.Game.Entities.Behaviors.Library;
using GameServer.Game.Entities.Behaviors.Transitions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using System.Xml.Linq;

#endregion

namespace GameServer.Game.Entities.Behaviors;

public static class BehaviorLibrary
{
    private static readonly Logger _log = new(typeof(BehaviorLibrary));

    private static readonly string _assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);

    private static readonly IEnumerable<MetadataReference> _defaultReferences =
    [
        MetadataReference.CreateFromFile(Path.Combine(_assemblyPath, "mscorlib.dll")),
        MetadataReference.CreateFromFile(Path.Combine(_assemblyPath, "System.dll")),
        MetadataReference.CreateFromFile(Path.Combine(_assemblyPath, "System.Core.dll")),
        MetadataReference.CreateFromFile(Path.Combine(_assemblyPath, "System.Runtime.dll")),
        MetadataReference.CreateFromFile(Path.Combine(_assemblyPath, "System.Linq.dll")),
        MetadataReference.CreateFromFile(Path.Combine(_assemblyPath, "System.Collections.dll")),
        MetadataReference.CreateFromFile(Path.Combine(_assemblyPath, "System.Xml.XDocument.dll")),
        MetadataReference.CreateFromFile(Path.Combine(_assemblyPath, "System.Numerics.Vectors.dll")),
        MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
        MetadataReference.CreateFromFile(typeof(BehaviorLibrary).GetTypeInfo().Assembly.Location),
        MetadataReference.CreateFromFile(typeof(TimedTransition).GetTypeInfo().Assembly.Location),
        MetadataReference.CreateFromFile(typeof(State).GetTypeInfo().Assembly.Location),
        MetadataReference.CreateFromFile(typeof(AOE).GetTypeInfo().Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Projectile).GetTypeInfo().Assembly.Location),
        MetadataReference.CreateFromFile(typeof(ProjectilePathSegment).GetTypeInfo().Assembly.Location),
        MetadataReference.CreateFromFile(typeof(ConditionEffectIndex).GetTypeInfo().Assembly.Location),
        MetadataReference.CreateFromFile(typeof(TileRegion).GetTypeInfo().Assembly.Location),
        MetadataReference.CreateFromFile(typeof(WorldPosData).GetTypeInfo().Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location),
        MetadataReference.CreateFromFile(typeof(XElement).GetTypeInfo().Assembly.Location)
    ];

    private static readonly ConcurrentDictionary<string, string> _behaviorFileCache = new();
    private static Assembly _lastAssembly;

    public static readonly ConcurrentDictionary<string, State> ClassicBehaviors = new();
    public static readonly ConcurrentDictionary<string, Type> EntityBehaviors = new();

    public static void Load(Assembly asm = null)
    {
        asm ??= Assembly.GetExecutingAssembly();

        _log.Info("Loading behavior library...");

        // Load custom C# behaviors
        foreach (var behaviorType in asm.GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(EntityBehavior))))
        {
            EntityBehaviors[behaviorType.Name] = behaviorType;
            _log.Debug($"Loading behavior '{behaviorType.Name}'");
        }

        // Load classic behaviors (root states) with the CharacterBehavior attribute
        var classicLibType = asm.GetType(typeof(BehaviorLib).FullName);
        if (classicLibType == null) // No classic lib changes
            return;

        var lib = Activator.CreateInstance(classicLibType); // Lib instance to get the property values
        var libType = classicLibType;
        foreach (var prop in libType.GetProperties())
        {
            var attribute = prop.GetCustomAttribute(typeof(CharacterBehaviorAttribute)) as CharacterBehaviorAttribute;
            if (attribute == null)
            {
                continue;
            }

            ClassicBehaviors[attribute.ObjectId] = (State)prop.GetValue(lib);
            _log.Debug($"Loading classic behavior '{attribute.ObjectId}'");
        }

        _log.Info("Finished loading behavior library.");
    }

    public static bool Reload(string behaviorsPath)
    {
        Assembly asm;
        using (new EasyTimer(LogLevel.Info, "Compiling behavior assembly...", "Compiled behavior assembly in [TIME]"))
        {
            asm = CompileBehaviorAssembly(behaviorsPath);
            _lastAssembly = asm;

            if (asm == null)
            {
                _log.Error("Failed to compile behavior assembly.");
                return false;
            }
        }

        Load(asm);
        return true;
    }

    private static Assembly CompileBehaviorAssembly(string behaviorsPath)
    {
        var defaultCompilationOptions =
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithOverflowChecks(true)
                .WithOptimizationLevel(OptimizationLevel.Release);

        var syntaxTree = ParseSyntaxTrees(behaviorsPath);
        if (syntaxTree.Length == 0)
            return _lastAssembly;

        var compilation = CSharpCompilation.Create(
            "BehaviorLibASM",
            syntaxTree,
            _defaultReferences,
            defaultCompilationOptions);

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);
        if (result.Success)
        {
            ms.Seek(0, SeekOrigin.Begin);
            var context = new AssemblyLoadContext("BehaviorLib", true);
            var asm = context.LoadFromStream(ms);
            context.Unload();
            return asm;
        }

        List<string> sb = new();
        var failures = result.Diagnostics.Where(diagnostic =>
            diagnostic.IsWarningAsError ||
            diagnostic.Severity == DiagnosticSeverity.Error);
        foreach (var diagnostic in failures)
            _log.Error(diagnostic.GetMessage());
        return null;
    }

    private static SyntaxTree[] ParseSyntaxTrees(string behaviorsPath)
    {
        var files = Directory.GetFiles(behaviorsPath, "*", SearchOption.AllDirectories);
        var ret = new List<SyntaxTree>(files.Length);
        Parallel.For(0, files.Length, i =>
        {
            var fileText = File.ReadAllText(files[i]);
            if (_behaviorFileCache.TryGetValue(files[i], out var cachedFileText))
            {
                if (fileText == cachedFileText) // File hasn't changed
                {
                    return;
                }
            }

            _behaviorFileCache[files[i]] = fileText;
            var tree = CSharpSyntaxTree.ParseText(fileText);
            using (TimedLock.Lock(ret))
                ret.Add(tree);
        });
        return ret.ToArray();
    }
}