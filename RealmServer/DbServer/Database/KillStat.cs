using System;
using System.Collections.Generic;

namespace DbServer.Database;

public partial class KillStat
{
    public int Id { get; set; }

    public uint? MonsterKills { get; set; }

    public uint? MonsterAssists { get; set; }

    public uint? GodKills { get; set; }

    public uint? GodAssists { get; set; }

    public ushort? OryxKills { get; set; }

    public ushort? OryxAssists { get; set; }

    public ushort? CubeKills { get; set; }

    public ushort? CubeAssists { get; set; }

    public ushort? BlueBags { get; set; }

    public ushort? CyanBags { get; set; }

    public ushort? WhiteBags { get; set; }

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();
}
