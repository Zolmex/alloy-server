namespace Common.Database.Models;

public class VaultChest {
    public int ChestId { get; set; }
    public int[] ItemTypes { get; set; }
    public byte[] ItemDatas { get; set; }
}