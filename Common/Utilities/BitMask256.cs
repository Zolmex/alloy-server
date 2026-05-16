namespace Common.Utilities;

public unsafe struct BitMask256 {
    public bool IsEmpty => !_notEmpty;
    
    private fixed uint _bits[8];
    private bool _notEmpty;

    public void Set(int index) {
        if (index < 0 || index > 255)
            return;
        _bits[index >> 5] |= 1u << (index & 31);
        _notEmpty = true;
    }

    public bool IsSet(int index) {
        if (index < 0 || index > 255)
            return false;
        return (_bits[index >> 5] & (1u << (index & 31))) != 0;
    }

    public void Clear() {
        for (var i = 0; i < 8; i++)
            _bits[i] = 0;
        _notEmpty = false;
    }
}