using Unity.Burst;
using Unity.Mathematics;

namespace Wfc
{
    [BurstCompile]
    unsafe struct BitField
    {
        #region Public methods

        public bool GetBit(int n)
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

        public int CountBits() => CountBits(this);

        public int FindNthOne(int n) => FindNthOne(this, n);

        #endregion

        #region Internal data

        const int Length = 4;
        fixed ulong _fields[Length];

        #endregion

        #region Burst functions

        [BurstCompile]
        static int CountBits(in BitField bf)
        {
            var count = 0;
            for (var i = 0; i < Length; i++)
                count += math.countbits(bf._fields[i]);
            return count;
        }

        [BurstCompile]
        static int FindNthOne(in BitField bf, int n)
        {
            var count = 0;
            for (var i = 0; i < Length * 64; i++)
                if (bf.GetBit(i) && count++ == n) return i;
            return -1;
        }

        #endregion
    }
}
