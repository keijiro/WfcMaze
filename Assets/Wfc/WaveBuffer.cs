using Unity.Mathematics;

namespace Wfc {

public struct WaveBuffer
{
    #region Public members

    public readonly int3 Dimensions => _dims;
    public readonly int Length => _waves.Length;

    public readonly int3 IndexToCoords(int i)
      => math.int3(i % _dims.x,
                   (i / _dims.x) % _dims.y,
                   i / (_dims.x * _dims.y));

    public readonly int CoordsToIndex(int x, int y, int z)
      => x + _dims.x * (y + _dims.y * z);

    public ref Wave this[int i]
      => ref _waves[i];

    public ref Wave this[int x, int y, int z]
      => ref _waves[CoordsToIndex(x, y, z)];

    public WaveBuffer(int sizeX, int sizeY, int sizeZ)
    {
        _waves = new Wave[sizeX * sizeY * sizeZ];
        _dims = math.int3(sizeX, sizeY, sizeZ);
        Reset();
    }

    #endregion

    #region Private members

    int3 _dims;
    Wave[] _waves;

    void Reset()
    {
        for (var (i, z) = (0, 0); z < _dims.z; z++)
        {
            var (z0, z1) = (z == 0, z == _dims.z - 1);

            for (var y = 0; y < _dims.y; y++)
            {
                var (y0, y1) = (y == 0, y == _dims.y - 1);

                for (var x = 0; x < _dims.x; x++, i++)
                {
                    var (x0, x1) = (x == 0, x == _dims.x - 1);

                    ref var w = ref _waves[i];
                    w.Reset();
                    if (x0) w.ForceDirection(Direction.XN, Axis.None);
                    if (y0) w.ForceDirection(Direction.YN, Axis.None);
                    if (z0) w.ForceDirection(Direction.ZN, Axis.None);
                    if (x1) w.ForceDirection(Direction.XP, Axis.None);
                    if (y1) w.ForceDirection(Direction.YP, Axis.None);
                    if (z1) w.ForceDirection(Direction.ZP, Axis.None);
                }
            }
        }
    }

    #endregion
}

} //namespace Wfc
