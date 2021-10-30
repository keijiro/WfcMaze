namespace Wfc {

// Connectivity information
public readonly struct Connectivity
{
    #region Public methods

    public Connectivity(Axis xn, Axis xp, Axis yn, Axis yp, Axis zn, Axis zp)
      => _encoded = (uint)xn      | (uint)xp <<  2 |
                    (uint)yn << 4 | (uint)yp <<  6 |
                    (uint)zn << 8 | (uint)zp << 10;

    public readonly Axis this[Direction dir]
      => (Axis)((_encoded >> (int)dir * 2) & 3u);

    public readonly Connectivity GetRotated(Pose pose)
      => Rotate(this, pose);

    #endregion

    #region Private members

    readonly uint _encoded;

    Connectivity(uint encoded) => _encoded = encoded;

    static uint Append(uint encoded, Direction dir, Axis axis)
    {
        var shift = (int)dir * 2;
        encoded &= ~(3u << shift);
        encoded |= (uint)axis << shift;
        return encoded;
    }

    static Connectivity Rotate(Connectivity con, Pose pose)
    {
        var d = 0u;
        d = Append(d, Direction.XN.GetRotated(pose), con[Direction.XN].GetRotated(pose));
        d = Append(d, Direction.XP.GetRotated(pose), con[Direction.XP].GetRotated(pose));
        d = Append(d, Direction.YN.GetRotated(pose), con[Direction.YN].GetRotated(pose));
        d = Append(d, Direction.YP.GetRotated(pose), con[Direction.YP].GetRotated(pose));
        d = Append(d, Direction.ZN.GetRotated(pose), con[Direction.ZN].GetRotated(pose));
        d = Append(d, Direction.ZP.GetRotated(pose), con[Direction.ZP].GetRotated(pose));
        return new Connectivity(d);
    }

    #endregion
}

} //namespace Wfc
