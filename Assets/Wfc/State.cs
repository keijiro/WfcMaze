namespace Wfc {

public readonly struct State
{
    //       -1 : Undetermined
    // 0 -  n-1 : Module with Pose 0
    // n - 2n-2 : Module with Pose 1
    // ...

    #region Public properties

    public static int Count = Geometry.PoseCount * ModuleRegistry.Count;

    public readonly bool IsDetermined => _value >= 0;
    public readonly int Index => _value % ModuleRegistry.Count;
    public readonly Pose Pose => (Pose)(_value / ModuleRegistry.Count);

    #endregion

    #region Constructor methods

    public static State Undetermined
      => new State(-1);

    public static State NewEncoded(int value)
      => new State(value);

    public static State NewIndexPose(int index, Pose pose)
      => new State((int)pose * ModuleRegistry.Count + index);

    #endregion

    #region Private member

    readonly int _value;

    State(int value) => _value = value;

    #endregion
}

} //namespace Wfc
