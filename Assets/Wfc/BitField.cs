using Unity.Mathematics;

namespace Wfc
{
    unsafe struct BitField
    {
        #region Public methods

        public readonly bool GetBit(int n)
          => (_fields[n / 64] & (1ul << (n % 64))) != 0ul;

        public void SetBit(int n)
          => _fields[n / 64] |= 1ul << (n % 64);

        public void UnsetBit(int n)
          => _fields[n / 64] &= ~(1ul << (n % 64));

        public void Clear()
        {
            for (var i = 0; i < Length; i++) _fields[i] = 0;
        }

        public void Fill(int bits)
        {
            for (var i = 0; i < bits / 64; i++) _fields[i] = ~0ul;
            _fields[bits / 64] = (1ul << (bits % 64)) - 1ul;
        }

        public readonly int CountBits()
        {
            var count = 0;
            for (var i = 0; i < Length; i++)
                count += math.countbits(_fields[i]);
            return count;
        }

        public readonly int FindNthOne(int n)
        {
            var count = 0;
            for (var i = 0; i < Length * 64; i++)
                if (GetBit(i) && count++ == n) return i;
            return -1;
        }

        #endregion

        #region Internal data

        const int Length = 4;
        fixed ulong _fields[Length];

        #endregion
    }
}
