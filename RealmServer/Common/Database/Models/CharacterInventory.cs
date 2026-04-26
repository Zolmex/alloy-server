using System.Collections.Generic;

namespace Common.Database.Models;

public class CharacterInventory : DbModel, IDbQueryable {
    public const string KEY_BASE = "characterInventory";

    public CharacterInventory() {
        RegisterProperty("CharacterId",
            (ref wtr) => wtr.Write(CharacterId),
            (ref rdr) => CharacterId = rdr.ReadInt32()
        );
        RegisterProperty("SlotId",
            (ref wtr) => wtr.Write(SlotId),
            (ref rdr) => SlotId = rdr.ReadInt32()
        );
        RegisterProperty("ItemType",
            (ref wtr) => wtr.Write(ItemType),
            (ref rdr) => ItemType = rdr.ReadUInt16()
        );
        RegisterProperty("ItemData",
            (ref wtr) => wtr.Write(ItemData ?? []),
            (ref rdr) => ItemData = rdr.ReadBytes(rdr.ReadUInt16()).ToArray()
        );
    }

    public override string Key => KEY_BASE + $".{CharacterId}.{SlotId}";

    public int CharacterId { get; set; }

    public int SlotId { get; set; }

    public ushort ItemType { get; set; }

    public byte[]? ItemData { get; set; }

    public virtual Character Character { get; set; } = null!;

    public static IEnumerable<string> GetIncludes() {
        yield break;
    }

    public static CharacterInventory Read(string key) {
        var ret = new CharacterInventory();
        var split = key.Split('.');
        ret.CharacterId = int.Parse(split[1]);
        ret.SlotId = int.Parse(split[2]);
        return ret;
    }

    public static string BuildKey(int charId, int slotId) {
        return KEY_BASE + $".{charId}.{slotId}";
    }
}