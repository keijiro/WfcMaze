using System.Linq;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace WfcMaze
{
    static class ModuleRegistry
    {
        public static int Count => _masks.Length;

        public static DirectionMask GetMask(int i) => _masks[i];

        public static void Load(ModuleSet moduleSet)
          => _masks = moduleSet.modules
                      .Select(m => Encode(m.WallFlags)).ToArray();

        static DirectionMask[] _masks;

        static DirectionMask Encode(in ModuleSet.WallFlags fl)
          => new DirectionMask
               (fl.Left, fl.Right, fl.Down, fl.Up, fl.Back, fl.Front);
    }

    struct State
    {
        //             -1 : Invalid
        //              0 : Empty
        //              1 : Block
        //      2 - (n+2) : Module /w Pose 0
        // (n+2) - (2n+2) : Module /w Pose 1
        // ...

        public static int Count = Geometry.PoseCount * ModuleRegistry.Count + 2;

        public int Index;

        public bool IsValid => Index >= 0;
        public bool IsEmpty => Index == 0;
        public bool IsBlock => Index == 1;
        public bool IsModule => Index > 1;
        public int ModuleIndex => (Index - 2) % ModuleRegistry.Count;
        public Pose Pose => (Pose)((Index - 2) / ModuleRegistry.Count);

        public static State Invalid => new State { Index = -1 };
        public static State Empty => new State { Index = 0 };
        public static State Block => new State { Index = 1 };
    }

    struct Wave
    {
        public bool IsObserved => _observed.IsValid;
        public State ObservedState => _observed;

        static Random _random = new Random(0xff332);

        BitField _bitField;
        State _observed;

        public float Entropy => _bitField.CountBits();

        public void Reset()
        {
            _bitField.Fill(State.Count);
            _observed = State.Invalid;
        }

        public void Collapse()
        {
            Debug.Log(IsObserved);
            var count = _bitField.CountBits();
            var select = _bitField.FindNthOne(_random.NextInt(count));
            _observed = new State { Index = select };
        }

        public void ShouldAllowEmptyOrMustBeBlock(Direction dir, bool block)
        {
            if (block)
            {
                _bitField.Clear();
                _bitField.SetBit(State.Block.Index);
            }
            else
            {
                ShouldAllowEmptyOrBlock(dir, false);
            }
        }

        public void ShouldAllowEmptyOrBlock(Direction dir, bool flip)
        {
            _bitField.ClearBit(flip ? 0 : 1);

            for (var i = 0; i < State.Count; i++)
            {
                if (!_bitField.CheckBit(i)) continue;

                var state = new State { Index = i };

                if (state.IsEmpty)
                {
                    if (flip) _bitField.ClearBit(i);
                }
                else if (state.IsBlock)
                {
                    if (!flip) _bitField.ClearBit(i);
                }
                else
                {
                    var mask = ModuleRegistry
                      .GetMask(state.ModuleIndex).GetRotated(state.Pose);

                    if (mask.CheckMasked(dir) ^ flip) _bitField.ClearBit(i);
                }
            }
        }
    }

    struct WaveBuffer
    {
        Wave[] _waves;
        int _size;

        ref Wave RefWave(int x, int y, int z)
          => ref _waves[x + _size * (y + _size * z)];

        public WaveBuffer(int size, int moduleCount)
        {
            _waves = new Wave[size * size * size];
            _size = size;

            for (var i = 0; i < _waves.Length; i++) _waves[i].Reset();
        }

        public Wave RetrieveWave(int x, int y, int z)
          => _waves[x + _size * (y + _size * z)];

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

            Debug.Log(min_e);

            Collapse(min_i % _size, (min_i / _size) % _size, min_i / _size / _size);
        }

        public void Collapse(int x, int y, int z)
        {
            RefWave(x, y, z).Collapse();
            Propagate(x, y, z);
        }

        void Propagate(int x, int y, int z)
        {
            var state = RefWave(x, y, z).ObservedState;
            if (state.IsEmpty)
                PropagateEmptyOrBlock(x, y, z, false);
            else if (state.IsBlock)
                PropagateEmptyOrBlock(x, y, z, true);
            else
                PropagateModule(x, y, z, state);
        }

        void PropagateEmptyOrBlock(int x, int y, int z, bool block)
        {
            if (x > 0        ) RefWave(x - 1, y, z).ShouldAllowEmptyOrBlock(Direction.XP, block);
            if (x < _size - 1) RefWave(x + 1, y, z).ShouldAllowEmptyOrBlock(Direction.XN, block);
            if (y > 0        ) RefWave(x, y - 1, z).ShouldAllowEmptyOrBlock(Direction.YP, block);
            if (y < _size - 1) RefWave(x, y + 1, z).ShouldAllowEmptyOrBlock(Direction.YN, block);
            if (z > 0        ) RefWave(x, y, z - 1).ShouldAllowEmptyOrBlock(Direction.ZP, block);
            if (z < _size - 1) RefWave(x, y, z + 1).ShouldAllowEmptyOrBlock(Direction.ZN, block);
        }

        void PropagateModule(int x, int y, int z, State state)
        {
            var mask = ModuleRegistry.GetMask(state.ModuleIndex);
            if (x > 0        ) RefWave(x - 1, y, z).ShouldAllowEmptyOrMustBeBlock(Direction.XP, mask.CheckMasked(Direction.XN));
            if (x < _size - 1) RefWave(x + 1, y, z).ShouldAllowEmptyOrMustBeBlock(Direction.XN, mask.CheckMasked(Direction.XP));
            if (y > 0        ) RefWave(x, y - 1, z).ShouldAllowEmptyOrMustBeBlock(Direction.YP, mask.CheckMasked(Direction.YN));
            if (y < _size - 1) RefWave(x, y + 1, z).ShouldAllowEmptyOrMustBeBlock(Direction.YN, mask.CheckMasked(Direction.YP));
            if (z > 0        ) RefWave(x, y, z - 1).ShouldAllowEmptyOrMustBeBlock(Direction.ZP, mask.CheckMasked(Direction.ZN));
            if (z < _size - 1) RefWave(x, y, z + 1).ShouldAllowEmptyOrMustBeBlock(Direction.ZN, mask.CheckMasked(Direction.ZP));
        }
    }

    public class Constructor : MonoBehaviour
    {
        [SerializeField] ModuleSet _moduleSet = null;
        [SerializeField] Material _material = null;
        [SerializeField] int _size = 10;

        WaveBuffer _waveBuffer;

        System.Collections.IEnumerator Start()
        {
            ModuleRegistry.Load(_moduleSet);

            _waveBuffer = new WaveBuffer(_size, _moduleSet.modules.Length);
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
            var wave = _waveBuffer.RetrieveWave(ix, iy, iz);
            if (!wave.IsObserved) return;

            var state = wave.ObservedState;
            if (!state.IsModule) return;

            var mesh = _moduleSet.modules[state.ModuleIndex].Mesh;
            var pos = math.float3(ix, iy, iz) - math.float3(0.5f, 0.5f, 0.5f) * _size;
            var rot = Geometry.ToRotation(state.Pose);

            Graphics.DrawMesh(mesh, pos, rot, _material, gameObject.layer);
        }
    }
}
