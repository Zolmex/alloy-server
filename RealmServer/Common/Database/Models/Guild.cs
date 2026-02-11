using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class Guild : DbModel, IDbQueryable
{
    public const string KEY_BASE = "guild";
    
    public override string Key => KEY_BASE + $".{Id}";
    
    public int Id { get; set; }

    public string? Name { get; set; }

    public short Level { get; set; }

    public uint CurrentFame { get; set; }

    public uint TotalFame { get; set; }

    public string? GuildBoard { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<GuildMember> GuildMembers { get; set; } = new List<GuildMember>();

    protected override void Prepare()
    {
        RegisterProperty("Id",
            wtr => wtr.Write(Id),
            rdr => Id = rdr.ReadInt32()
        );
        RegisterProperty("Name",
            wtr => wtr.Write(Name ?? ""),
            rdr => Name = rdr.ReadUTF()
        );
        RegisterProperty("Level",
            wtr => wtr.Write(Level),
            rdr => Level = rdr.ReadInt16()
        );
        RegisterProperty("CurrentFame",
            wtr => wtr.Write(CurrentFame),
            rdr => CurrentFame = rdr.ReadUInt32()
        );
        RegisterProperty("TotalFame",
            wtr => wtr.Write(TotalFame),
            rdr => TotalFame = rdr.ReadUInt32()
        );
        RegisterProperty("GuildBoard",
            wtr => wtr.Write(GuildBoard ?? ""),
            rdr => GuildBoard = rdr.ReadUTF()
        );
        RegisterProperty("CreatedAt",
            wtr => wtr.Write((CreatedAt ?? DateTime.MinValue).ToUnixTimestamp()),
            rdr => CreatedAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32())
        );
        RegisterProperty("GuildMembers",
            wtr =>
            {
                wtr.Write((short)GuildMembers.Count);
                foreach (var guildMember in GuildMembers)
                {
                    var hasValue = guildMember != null;
                    wtr.Write(hasValue);
                    if (hasValue)
                        guildMember.WriteProperties(wtr);
                }
            },
            rdr =>
            {
                GuildMembers.Clear();
                var count = rdr.ReadInt16();
                for (var i = 0; i < count; i++)
                {
                    var member = DbModel.Read<GuildMember>(rdr);
                    if (member != null)
                        GuildMembers.Add(member);
                }
            }
        );
    }

    public static Guild Read(string key)
    {
        var ret = new Guild();
        var split = key.Split('.');
        ret.Id = int.Parse(split[1]);
        return ret;
    }

    public static IEnumerable<string> GetIncludes()
    {
        yield return "GuildMembers";
    }
    
    public static string BuildKey(int id)
    {
        return KEY_BASE + $".{id}";
    }
}
