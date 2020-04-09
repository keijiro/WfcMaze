using System.Linq;
using Unity.Mathematics;

namespace WfcMaze
{
    struct Wave
    {
        public bool IsObserved => _observed.IsDetermined;
        public State ObservedState => _observed;

        static Random _random = new Random(0xff332);

        BitField _bitField;
        State _observed;

        public float Entropy => _bitField.CountBits();

        public void Reset()
        {
            _bitField.Fill(State.Count);
            _observed = State.Undetermined;
        }

        public void Collapse()
        {
            var count = _bitField.CountBits();
            var select = _bitField.FindNthOne(_random.NextInt(count));
            _observed = State.Encoded(select);
        }

        public void ForceDirection(Direction dir, bool flag)
        {
            for (var i = 0; i < State.Count; i++)
            {
                if (!_bitField.CheckBit(i)) continue;

                var state = State.Encoded(i);

                var mask = ModuleRegistry
                    .GetMask(state.Index).GetRotated(state.Pose);

                var masked = mask.CheckMasked(dir);

                if (masked != flag) _bitField.ClearBit(i);
            }
        }
    }
}
