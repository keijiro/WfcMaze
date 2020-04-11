using System.Linq;
using Unity.Mathematics;

namespace Wfc
{
    public struct WaveBuffer
    {
        #region Public members

        public WaveBuffer(int sizeX, int sizeY, int sizeZ, uint seed)
        {
            _dims = math.int3(sizeX, sizeY, sizeZ);
            _waves = new Wave[sizeX * sizeY * sizeZ];
            _random = new Random(seed);

            // Reset all the waves.
            for (var i = 0; i < _waves.Length; i++) _waves[i].Reset();

            // X boundaries
            for (var y = 0; y < sizeY; y++)
                for (var z = 0; z < sizeZ; z++)
                {
                    RefWave(        0, y, z).ForceDirection(Direction.XN, Axis.None);
                    RefWave(sizeX - 1, y, z).ForceDirection(Direction.XP, Axis.None);
                }

            // Y boundaries
            for (var x = 0; x < sizeX; x++)
                for (var z = 0; z < sizeZ; z++)
                {
                    RefWave(x,         0, z).ForceDirection(Direction.YN, Axis.None);
                    RefWave(x, sizeY - 1, z).ForceDirection(Direction.YP, Axis.None);
                }

            // Z boundaries
            for (var x = 0; x < sizeX; x++)
                for (var y = 0; y < sizeY; y++)
                {
                    RefWave(x, y,         0).ForceDirection(Direction.ZN, Axis.None);
                    RefWave(x, y, sizeZ - 1).ForceDirection(Direction.ZP, Axis.None);
                }
        }

        public Wave GetWave(int x, int y, int z)
          => _waves[CoordsToIndex(x, y, z)];

        public void Observe()
        {
            var min_e = 1e+6f;
            var min_i = -1;

            for (var i = 0; i < _waves.Length; i++)
            {
                if (_waves[i].IsObserved) continue;
                var e = _waves[i].Entropy + _random.NextFloat(0.5f);
                if (e < min_e)
                {
                    min_e = e;
                    min_i = i;
                }
            }

            if (min_i < 0) return;

            var coords = IndexToCoords(min_i);
            RefWave(coords.x, coords.y, coords.z).Collapse(_random.NextUInt());
            Propagate(coords.x, coords.y, coords.z);
        }

        #endregion

        #region Private members

        int3 _dims;
        Wave[] _waves;
        Random _random;

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
            var conn = ModuleRegistry.GetConnectivity(state.Index);
            conn = conn.GetRotated(state.Pose);

            // Boundary check
            var coord = math.int3(x, y, z);
            var bl = coord > 0;
            var bh = coord < _dims - 1;

            if (bl.x) RefWave(x - 1, y, z).ForceDirection(Direction.XP, conn[Direction.XN]);
            if (bh.x) RefWave(x + 1, y, z).ForceDirection(Direction.XN, conn[Direction.XP]);
            if (bl.y) RefWave(x, y - 1, z).ForceDirection(Direction.YP, conn[Direction.YN]);
            if (bh.y) RefWave(x, y + 1, z).ForceDirection(Direction.YN, conn[Direction.YP]);
            if (bl.z) RefWave(x, y, z - 1).ForceDirection(Direction.ZP, conn[Direction.ZN]);
            if (bh.z) RefWave(x, y, z + 1).ForceDirection(Direction.ZN, conn[Direction.ZP]);
        }

        #endregion
    }
}
