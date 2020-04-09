using System.Linq;
using Unity.Mathematics;

namespace Wfc
{
    public struct Wave
    {
        #region Public members

        public bool IsObserved => _observed.IsDetermined;
        public State ObservedState => _observed;
        public float Entropy => _bitField.CountBits();

        public void Reset()
        {
            _bitField.Fill(State.Count);
            _observed = State.Undetermined;
        }

        public void Collapse()
        {
            var count = _bitField.CountBits();
            var state = _bitField.FindNthOne(_random.NextInt(count));
            _observed = State.NewEncoded(state);
        }

        public void ForceDirection(Direction dir, bool flag)
        {
            for (var i = 0; i < State.Count; i++)
            {
                if (!_bitField.GetBit(i)) continue;

                var state = State.NewEncoded(i);
                var connectivity = ModuleRegistry
                    .GetConnectivity(state.Index).GetRotated(state.Pose);

                if (connectivity.Check(dir) != flag) _bitField.UnsetBit(i);
            }
        }

        #endregion

        #region Private members

        static Random _random = new Random(0xdeadbeef);

        BitField _bitField;
        State _observed;

        #endregion
    }
}
