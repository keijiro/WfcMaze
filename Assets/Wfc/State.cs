namespace Wfc
{
    public struct State
    {
        //       -1 : Undetermined
        // 0 -  n-1 : Module with Pose 0
        // n - 2n-2 : Module with Pose 1
        // ...

        #region Public properties

        public static int Count = Geometry.PoseCount * ModuleRegistry.Count;

        public bool IsDetermined => _value >= 0;
        public int Index => _value % ModuleRegistry.Count;
        public Pose Pose => (Pose)(_value / ModuleRegistry.Count);

        #endregion

        #region Constructor methods

        public static State Undetermined
          => new State { _value = -1 };

        public static State NewEncoded(int value)
          => new State { _value = value };

        public static State NewIndexPose(int index, Pose pose)
          => new State { _value = (int)pose * ModuleRegistry.Count + index };

        #endregion

        #region Private member

        int _value;

        #endregion
    }
}
