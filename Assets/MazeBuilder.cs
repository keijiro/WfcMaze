using UnityEngine;
using Unity.Mathematics;

sealed class MazeBuilder : MonoBehaviour
{
    [SerializeField] Mesh _iMesh = null;
    [SerializeField] Mesh _lMesh = null;
    [SerializeField] Material _material = null;
    [SerializeField] int _size = 10;

    Wfc.WaveBuffer _waveBuffer;

    System.Collections.IEnumerator Start()
    {
        // Empty
        Wfc.ModuleRegistry.AddModule
          (new Wfc.Connectivity(false, false, false, false, false, false));

        // I bar
        Wfc.ModuleRegistry.AddModule
          (new Wfc.Connectivity(false, false, false, false, true, true));

        // L bar
        Wfc.ModuleRegistry.AddModule
          (new Wfc.Connectivity(false, true, false, false, true, false));

        _waveBuffer = new Wfc.WaveBuffer(_size, _size, _size);

        _waveBuffer.Collapse(_size / 2, _size / 2, _size / 2);

        while (true)
        {
            _waveBuffer.CollapseMinimumEntropyWave();
            yield return null;
        }
    }

    void Update()
    {
        for (var iz = 0; iz < _size; iz++)
            for (var iy = 0; iy < _size; iy++)
                for (var ix = 0; ix < _size; ix++)
                    DrawWave(ix, iy, iz);
    }

    void DrawWave(int ix, int iy, int iz)
    {
        var wave = _waveBuffer.GetWave(ix, iy, iz);
        if (!wave.IsObserved) return;

        var state = wave.ObservedState;
        if (state.Index == 0) return; // Empty

        var mesh = state.Index == 1 ? _iMesh : _lMesh;
        var pos = math.float3(ix, iy, iz);
        pos -= math.float3(0.5f, 0.5f, 0.5f) * _size;
        var rot = Wfc.Geometry.ToRotation(state.Pose);

        Graphics.DrawMesh(mesh, pos, rot, _material, gameObject.layer);
    }
}
