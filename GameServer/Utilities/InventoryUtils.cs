using Common;

namespace GameServer.Utilities;

public static class InventoryUtils {
    public static ushort GetBagIdFromType(BagType bagType) {
        switch (bagType) {
            case BagType.Common:
                return 1280;
            case BagType.Pink:
                return 1286;
            case BagType.Cyan:
                return 1288;
            case BagType.Blue:
                return 1289;
            case BagType.White:
                return 1296;
            case BagType.Purple:
                return 1287;
        }
        return 1280;
    }
}