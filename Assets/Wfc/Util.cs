using Unity.Mathematics;

namespace Wfc
{
    static class Util
    {
        public static int CountBits(ulong x)
        {
            var count = 0;
            for (var i = 0; i < 64; i++)
                if (((1ul << i) & x) != 0) count++;
            return count;
        }
    }
}
