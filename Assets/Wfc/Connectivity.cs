using Unity.Mathematics;

namespace Wfc
{
    // Connectivity information
    public struct Connectivity
    {
        #region Public methods

        public Connectivity
          (Axis? xn, Axis? xp, Axis? yn, Axis? yp, Axis? zn, Axis? zp)
          => _encoded = Convert(xn)      | Convert(xp) <<  2 |
                        Convert(yn) << 4 | Convert(yp) <<  6 |
                        Convert(zn) << 8 | Convert(zp) << 10;

        public Axis? this[Direction dir]
        {
            get => GetDirection(dir);
            set => SetDirection(dir, value);
        }

        public Connectivity GetRotated(Pose pose)
          => Rotate(this, pose);

        #endregion

        #region Private members

        uint _encoded;

        static uint Convert(Axis? axis)
          => axis == null ? 0u : (uint)axis + 1u;

        Axis? GetDirection(Direction dir)
        {
            var bits = (_encoded >> (int)dir * 2) & 3u;
            return bits == 0u ? null : (Axis?)(bits - 1u);
        }

        void SetDirection(Direction dir, Axis? axis)
        {
            var shift = (int)dir * 2;
            _encoded &= ~(3u << shift);
            _encoded |= Convert(axis) << shift;
        }

        static Connectivity Rotate(Connectivity con, Pose pose)
        {
            var res = new Connectivity();
            res[Direction.XN.GetRotated(pose)] = con[Direction.XN]?.GetRotated(pose);
            res[Direction.XP.GetRotated(pose)] = con[Direction.XP]?.GetRotated(pose);
            res[Direction.YN.GetRotated(pose)] = con[Direction.YN]?.GetRotated(pose);
            res[Direction.YP.GetRotated(pose)] = con[Direction.YP]?.GetRotated(pose);
            res[Direction.ZN.GetRotated(pose)] = con[Direction.ZN]?.GetRotated(pose);
            res[Direction.ZP.GetRotated(pose)] = con[Direction.ZP]?.GetRotated(pose);
            return res;
        }

        #endregion
    }
}
