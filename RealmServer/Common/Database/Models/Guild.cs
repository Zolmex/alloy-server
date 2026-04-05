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

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<GuildMember> GuildMembers { get; set; } = new List<GuildMember>();

    public Guild()
    {
        RegisterProperty("Id",
           (ref wtr) => wtr.Write(Id),
            (ref rdr) => Id = rdr.ReadInt32()
        );
        RegisterProperty("Name",
           (ref wtr) => wtr.WriteUTF(Name ?? ""),
            (ref rdr) => Name = rdr.ReadUTF()
        );
        RegisterProperty("Level",
           (ref wtr) => wtr.Write(Level),
            (ref rdr) => Level = rdr.ReadInt16()
        );
        RegisterProperty("CurrentFame",
           (ref wtr) => wtr.Write(CurrentFame),
            (ref rdr) => CurrentFame = rdr.ReadUInt32()
        );
        RegisterProperty("TotalFame",
           (ref wtr) => wtr.Write(TotalFame),
            (ref rdr) => TotalFame = rdr.ReadUInt32()
        );
        RegisterProperty("GuildBoard",
           (ref wtr) => wtr.WriteUTF(GuildBoard ?? ""),
            (ref rdr) => GuildBoard = rdr.ReadUTF()
        );
        RegisterProperty("CreatedAt",
           (ref wtr) => wtr.Write(CreatedAt.ToUnixTimestamp()),
            (ref rdr) => CreatedAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32())
        );
        RegisterProperty("GuildMembers",
            (ref wtr) =>
            {
                wtr.Write((short)GuildMembers.Count);
                foreach (var guildMember in GuildMembers)
                {
                    var hasValue = guildMember != null;
                    wtr.Write(hasValue);
                    if (hasValue)
                        guildMember.WriteProperties(ref wtr);
                }
            },
            (ref rdr) =>
            {
                GuildMembers.Clear();
                var count = rdr.ReadInt16();
                for (var i = 0; i < count; i++)
                {
                    var member = DbModel.Read<GuildMember>(ref rdr);
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
