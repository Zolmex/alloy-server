namespace Common.Utilities;

public static class ItemUtils {
    
    public static ushort GetBagIdFromType(BagType bagType) {
        switch (bagType) {
            case BagType.Common:
                return 0x0500;
            case BagType.Pink:
                return 0x0506;
            case BagType.Cyan:
                return 0x0507;
            case BagType.Blue:
                return 0x0508;
            case BagType.White:
                return 0x0509;
            case BagType.Purple:
                return 0x050B;
        }

        return 0x0500;
    }
}