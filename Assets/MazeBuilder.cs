using UnityEngine;
using Unity.Mathematics;
using Wfc;

sealed class MazeBuilder : MonoBehaviour
{
    [SerializeField] Mesh _iMesh = null;
    [SerializeField] Mesh _lMesh = null;
    [SerializeField] Mesh _tMesh = null;
    [SerializeField] Mesh _sMesh = null;
    [SerializeField] Material _material = null;
    [SerializeField] int _size = 10;
    [SerializeField] uint _seed = 1234;

    WaveBuffer _waveBuffer;

    Mesh GetMesh(int index)
    {
        switch (index)
        {
            case 0: return null;
            case 1: return _iMesh;
            case 2: return _lMesh;
            case 3: return _tMesh;
            case 4: return _sMesh;
        }
        return null;
    }

    System.Collections.IEnumerator Start()
    {
        ModuleRegistry.AddModule
          (new Connectivity(null, null, null, null, null, null));

        // I bar
        ModuleRegistry.AddModule
          (new Connectivity(null, null, null, null, Axis.X, Axis.X));

        // L bar
        ModuleRegistry.AddModule
          (new Connectivity(null, Axis.Z, null, null, Axis.X, null));

        // T bar
        ModuleRegistry.AddModule
          (new Connectivity(Axis.Z, Axis.Z, null, null, Axis.X, null));

        // Stairs
        ModuleRegistry.AddModule
          (new Connectivity(null, null, null, Axis.X, Axis.X, null));

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
        var mesh = GetMesh(state.Index);
        if (mesh == null) return;

        var pos = math.float3(ix, iy, iz) - 0.5f * _size + 0.5f;
        var rot = state.Pose.ToRotation();

        var m1 = (float4x4)transform.localToWorldMatrix;
        var m2 = math.float4x4(rot, pos);

        Graphics.DrawMesh(mesh, math.mul(m1, m2), _material, gameObject.layer);
    }
}
