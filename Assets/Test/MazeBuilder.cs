using System.Linq;
using UnityEngine;
using Unity.Mathematics;
using Wfc;

sealed class MazeBuilder : MonoBehaviour
{
    [SerializeField] ModuleSet _moduleSet = null;
    [SerializeField] Material _material = null;
    [SerializeField] int3 _size = math.int3(10, 10, 10);
    [SerializeField] uint _seed = 1234;

    WaveBuffer _waveBuffer;

    System.Collections.IEnumerator Start()
    {
        ModuleRegistry.Clear();

        foreach (var c in _moduleSet.modules.Select(m => m.Connectivity))
            ModuleRegistry.AddModule(new Connectivity
              (c.Left, c.Right, c.Bottom, c.Top, c.Back, c.Front));

        _waveBuffer = new WaveBuffer(_size.x, _size.y, _size.z, _seed);

        while (true)
        {
            _waveBuffer.Observe();
            yield return null;
        }
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
        var wave = _waveBuffer.GetWave(ix, iy, iz);
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
