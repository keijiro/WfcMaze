using Unity.Mathematics;
using System;

namespace Wfc {

public ref struct WaveBuffer
{
    #region Public members

    public readonly int3 Dimensions => _dims;
    public readonly int Length => _storage.Length;

    public readonly int3 IndexToCoords(int i)
      => math.int3(i % _dims.x,
                   (i / _dims.x) % _dims.y,
                   i / (_dims.x * _dims.y));

    public readonly int CoordsToIndex(int x, int y, int z)
      => x + _dims.x * (y + _dims.y * z);

    public ref Wave this[int i]
      => ref _storage[i];

    public ref Wave this[int x, int y, int z]
      => ref _storage[CoordsToIndex(x, y, z)];

    public WaveBuffer(Span<Wave> storage, int sizeX, int sizeY, int sizeZ)
    {
        _storage = storage;
        _dims = math.int3(sizeX, sizeY, sizeZ);
    }

    public void Reset()
    {
        for (var i = 0; i < Length; i++) this[i].Reset();

        // X boundaries
        for (var y = 0; y < _dims.y; y++)
            for (var z = 0; z < _dims.z; z++)
            {
                this[          0, y, z].ForceDirection(Direction.XN, Axis.None);
                this[_dims.x - 1, y, z].ForceDirection(Direction.XP, Axis.None);
            }

        // Y boundaries
        for (var x = 0; x < _dims.x; x++)
            for (var z = 0; z < _dims.z; z++)
            {
                this[x,           0, z].ForceDirection(Direction.YN, Axis.None);
                this[x, _dims.y - 1, z].ForceDirection(Direction.YP, Axis.None);
            }

        // Z boundaries
        for (var x = 0; x < _dims.x; x++)
            for (var y = 0; y < _dims.y; y++)
            {
                this[x, y,           0].ForceDirection(Direction.ZN, Axis.None);
                this[x, y, _dims.z - 1].ForceDirection(Direction.ZP, Axis.None);
            }
    }

    #endregion

    #region Private members

    Span<Wave> _storage;
    int3 _dims;

    #endregion
}

} //namespace Wfc
