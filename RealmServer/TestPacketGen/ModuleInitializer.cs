using System.Runtime.CompilerServices;

namespace TestPacketGen;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Initialize();
    }
}
