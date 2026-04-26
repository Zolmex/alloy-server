#region

using Common.Database;
using System.Linq;

#endregion

namespace GameServer.Game.Entities.Types;

public class VaultChest : Container
{
    public VaultChest(ushort objType, int ownerId)
        : base(objType, 8, ownerId, -1)
    { }

    // public void LoadVaultChest(DbVault dbVaults, int vaultId) // TODO: fix
    // {
    //     var dbChest = dbVaults.VaultChests[vaultId];
    //     if (dbChest.ItemTypes == null || dbChest.ItemDatas == null)
    //     {
    //         dbChest.ItemTypes = Enumerable.Repeat(-1, 8).ToArray();
    //         dbChest.ItemDatas = Enumerable.Repeat<byte>(0, 8).ToArray();
    //     }
    //
    //     Inventory.SetItems(dbChest.ItemTypes, dbChest.ItemDatas);
    // }
}