using System.Linq;
using Unity.Mathematics;

namespace WfcMaze
{
    struct WaveBuffer
    {
        #region Public members

        public WaveBuffer(int sizeX, int sizeY, int sizeZ)
        {
            _dims = math.int3(sizeX, sizeY, sizeZ);
            _waves = new Wave[sizeX * sizeY * sizeZ];
            for (var i = 0; i < _waves.Length; i++) _waves[i].Reset();
        }

        public Wave GetWave(int x, int y, int z)
          => _waves[CoordsToIndex(x, y, z)];

        public void CollapseMinimumEntropyWave()
        {
            var min_e = 1e+6f;
            var min_i = -1;

            for (var i = 0; i < _waves.Length; i++)
            {
                if (_waves[i].IsObserved) continue;
                var e = _waves[i].Entropy;
                if (e < min_e)
                {
                    min_e = e;
                    min_i = i;
                }
            }

            if (min_i < 0) return;

            var coords = IndexToCoords(min_i);
            Collapse(coords.x, coords.y, coords.z);
        }

        public void Collapse(int x, int y, int z)
        {
            RefWave(x, y, z).Collapse();
            Propagate(x, y, z);
        }

        #endregion

        #region Private members

        int3 _dims;
        Wave[] _waves;

        int3 IndexToCoords(int i) => math.int3
          (i % _dims.x, (i / _dims.x) % _dims.y, i / (_dims.x * _dims.y));

        int CoordsToIndex(int x, int y, int z)
          => x + _dims.x * (y + _dims.y * z);

        ref Wave RefWave(int x, int y, int z)
          => ref _waves[CoordsToIndex(x, y, z)];

        void Propagate(int x, int y, int z)
        {
            // Retrieve the wave state.
            var state = RefWave(x, y, z).ObservedState;
            var mask = ModuleRegistry.GetMask(state.Index);
            mask = mask.GetRotated(state.Pose);

            // Boundary check
            var coord = math.int3(x, y, z);
            var bl = coord > 0;
            var bh = coord < _dims - 1;

            if (bl.x) RefWave(x - 1, y, z).ForceDirection(Direction.XP, mask.CheckMasked(Direction.XN));
            if (bh.x) RefWave(x + 1, y, z).ForceDirection(Direction.XN, mask.CheckMasked(Direction.XP));
            if (bl.y) RefWave(x, y - 1, z).ForceDirection(Direction.YP, mask.CheckMasked(Direction.YN));
            if (bh.y) RefWave(x, y + 1, z).ForceDirection(Direction.YN, mask.CheckMasked(Direction.YP));
            if (bl.z) RefWave(x, y, z - 1).ForceDirection(Direction.ZP, mask.CheckMasked(Direction.ZN));
            if (bh.z) RefWave(x, y, z + 1).ForceDirection(Direction.ZN, mask.CheckMasked(Direction.ZP));
        }

        #endregion
    }
}
