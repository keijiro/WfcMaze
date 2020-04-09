using Unity.Mathematics;

namespace Wfc
{
    // Connectivity information
    struct Connectivity
    {
        public Connectivity
          (bool xn, bool xp, bool yn, bool yp, bool zn, bool zp)
          => _encoded = (xn ? 0x01u : 0u) | (xp ? 0x02u : 0u) |
                        (yn ? 0x04u : 0u) | (yp ? 0x08u : 0u) |
                        (zn ? 0x10u : 0u) | (zp ? 0x20u : 0u);

        public Connectivity(uint encoded)
          => _encoded = encoded;

        public bool Check(Direction dir)
          => (_encoded & (1u << (int)dir)) != 0u;

        public Connectivity GetRotated(Pose pose)
          => Rotate(this, pose);

        public static Connectivity Rotate(Connectivity source, Pose pose)
        {
            var encoded = 0u;
            for (var i = 0; i < Geometry.DirectionCount; i++)
            {
                if ((source._encoded & (1u << i)) == 0u) continue;
                encoded |= 1u << (int)Geometry.Rotate((Direction)i, pose);
            }
            return new Connectivity(encoded);
        }

        uint _encoded;
    }
}
