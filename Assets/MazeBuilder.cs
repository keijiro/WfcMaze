using UnityEngine;
using Unity.Mathematics;
using Wfc;

sealed class MazeBuilder : MonoBehaviour
{
    [SerializeField] Mesh _iMesh = null;
    [SerializeField] Mesh _lMesh = null;
    [SerializeField] Mesh _tMesh = null;
    [SerializeField] Material _material = null;
    [SerializeField] int _size = 10;
    [SerializeField] uint _seed = 1234;

    WaveBuffer _waveBuffer;

    Mesh GetMesh(int index)
    {
        switch (index)
        {
            case 0: return _iMesh;
            case 1: return _lMesh;
            case 2: return _tMesh;
        }
        return null;
    }

    System.Collections.IEnumerator Start()
    {
        // I bar
        ModuleRegistry.AddModule
          (new Connectivity(false, false, false, false, true, true));

        // L bar
        ModuleRegistry.AddModule
          (new Connectivity(false, true, false, false, true, false));

        // T bar
        ModuleRegistry.AddModule
          (new Connectivity(true, true, false, false, true, false));

        _waveBuffer = new WaveBuffer(_size, _size, _size, _seed);

        while (true)
        {
            _waveBuffer.Observe();
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
        var pos = math.float3(ix, iy, iz) - 0.5f * _size + 0.5f;
        var rot = state.Pose.ToRotation();

        Graphics.DrawMesh(
          GetMesh(state.Index),
          math.mul(transform.localToWorldMatrix, math.float4x4(rot, pos)),
          _material, gameObject.layer
        );
    }
}
