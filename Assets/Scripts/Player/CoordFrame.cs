using System;
using UnityEngine;

internal class CoordFrame
{
    internal Vector3 position = Vector3.zero;
    internal Vector3 vel = Vector3.zero;
    internal Vector3 mousePosOnMap = Vector3.zero;
    internal int frameID = 0;

    internal void reset()
    {
        frameID = 0;
    }
}