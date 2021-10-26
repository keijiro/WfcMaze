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

        public void Collapse(uint random)
        {
            var count = _bitField.CountBits();

            if (count == 0)
            {
                UnityEngine.Debug.Log("Contradiction found. Skipping.");
                _observed = State.NewEncoded(0);
                return;
            }

            var state = _bitField.FindNthOne((int)(random % count));
            _observed = State.NewEncoded(state);
        }

        public void ForceDirection(Direction dir, Axis axis)
        {
            for (var i = 0; i < State.Count; i++)
            {
                if (!_bitField.GetBit(i)) continue;

                var state = State.NewEncoded(i);
                var connectivity = ModuleRegistry
                    .GetConnectivity(state.Index).GetRotated(state.Pose);

                if (connectivity[dir] != axis) _bitField.UnsetBit(i);
            }
        }

        #endregion

        #region Private members

        BitField _bitField;
        State _observed;

        #endregion
    }
}
