using System.Runtime.CompilerServices;

namespace Common.Structs;

[InlineArray(StatData.STAT_COUNT)]
public struct StatValueBuffer { private StatValue _; }

[InlineArray(StatData.STAT_COUNT)]
public struct StatDataBuffer { private StatData _; }