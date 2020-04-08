namespace WfcMaze
{
    unsafe struct BitField
    {
        const int Length = 16;
        fixed ulong _fields[Length];

        public bool CheckBit(int n)
          => (_fields[n / 64] & (1ul << (n % 64))) != 0ul;

        public void SetBit(int n)
          => _fields[n / 64] |= 1ul << (n % 64);

        public void ClearBit(int n)
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

        public int CountBits()
        {
            var count = 0;
            for (var i = 0; i < Length; i++)
                count += Util.CountBits(_fields[i]);
            return count;
        }

        public int FindNthOne(int n)
        {
            var count = 0;
            for (var i = 0; i < Length * 64; i++)
                if (CheckBit(i) && count++ == n) return i;
            return -1;
        }
    }
}
