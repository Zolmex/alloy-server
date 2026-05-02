namespace Common.Utilities;

public unsafe struct BitMask256 {
    private fixed uint _bits[8];

    public void Set(int index) {
        if (index < 0 || index > 255)
            return;
        _bits[index >> 5] |= 1u << (index & 31);
    }

    public bool IsSet(int index) {
        if (index < 0 || index > 255)
            return false;
        return (_bits[index >> 5] & (1u << (index & 31))) != 0;
    }

    public void Clear() {
        for (var i = 0; i < 8; i++)
            _bits[i] = 0;
    }
}