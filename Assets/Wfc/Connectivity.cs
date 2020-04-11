using Unity.Mathematics;

namespace Wfc
{
    // Connectivity information
    public struct Connectivity
    {
        #region Public methods

        public Connectivity
          (bool xn, bool xp, bool yn, bool yp, bool zn, bool zp)
          => _encoded = (xn ? 0x01u : 0u) | (xp ? 0x02u : 0u) |
                        (yn ? 0x04u : 0u) | (yp ? 0x08u : 0u) |
                        (zn ? 0x10u : 0u) | (zp ? 0x20u : 0u);

        public bool this[Direction dir]
        {
            get => (_encoded & (1u << (int)dir)) != 0u;
            set => SetDirection(dir, value);
        }

        public Connectivity GetRotated(Pose pose)
          => Rotate(this, pose);

        #endregion

        #region Private members

        uint _encoded;

        void SetDirection(Direction dir, bool flag)
        {
            if (flag)
                _encoded |= (1u << (int)dir);
            else
                _encoded &= ~(1u << (int)dir);
        }

        static Connectivity Rotate(Connectivity con, Pose pose)
        {
            var res = new Connectivity();
            if (con[Direction.XN]) res[Direction.XN.GetRotated(pose)] = true;
            if (con[Direction.XP]) res[Direction.XP.GetRotated(pose)] = true;
            if (con[Direction.YN]) res[Direction.YN.GetRotated(pose)] = true;
            if (con[Direction.YP]) res[Direction.YP.GetRotated(pose)] = true;
            if (con[Direction.ZN]) res[Direction.ZN.GetRotated(pose)] = true;
            if (con[Direction.ZP]) res[Direction.ZP.GetRotated(pose)] = true;
            return res;
        }

        #endregion
    }
}
