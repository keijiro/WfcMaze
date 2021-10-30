using System.Linq;
using UnityEngine;
using Unity.Mathematics;
using Wfc;

using Stopwatch = System.Diagnostics.Stopwatch;

sealed class MazeBuilder : MonoBehaviour
{
    [SerializeField] ModuleSet _moduleSet = null;
    [SerializeField] Material _material = null;
    [SerializeField] int3 _size = math.int3(10, 10, 10);
    [SerializeField] uint _seed = 1234;

    Wave[] _waves;
    WaveBuffer Buffer => new WaveBuffer(_waves, _size.x, _size.y, _size.z);

    void Start()
    {
        ModuleRegistry.Reset
          (_moduleSet.modules.Select(m => (Connectivity)m.Connectivity));

        _waves = new Wave[_size.x * _size.y * _size.z];
        Buffer.Reset();

        var sw = new Stopwatch();
        sw.Start();

        var observer = new Observer(_seed);
        while (!observer.Observe(Buffer)) {}

        sw.Stop();
        Debug.Log(sw.Elapsed.TotalMilliseconds);
    }

    void Update()
    {
        for (var iz = 0; iz < _size.z; iz++)
            for (var iy = 0; iy < _size.y; iy++)
                for (var ix = 0; ix < _size.x; ix++)
                    DrawWave(ix, iy, iz);
    }

    void DrawWave(int ix, int iy, int iz)
    {
        var wave = Buffer[ix, iy, iz];
        if (!wave.IsObserved) return;

        var state = wave.ObservedState;
        var mesh = _moduleSet.modules[state.Index].Mesh;
        if (mesh == null) return;

        var pos = math.float3(ix, iy, iz) - _size / 2 + 0.5f;
        var rot = state.Pose.ToRotation();

        var m1 = (float4x4)transform.localToWorldMatrix;
        var m2 = math.float4x4(rot, pos);

        Graphics.DrawMesh(mesh, math.mul(m1, m2), _material, gameObject.layer);
    }
}
