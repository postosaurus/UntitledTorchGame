using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RayRange {
    public RayRange(float x1, float x2, float y1, float y2, Vector2 dir) {
        Start = new Vector2(x1, x2);
        End = new Vector2(y1, y2);
        Dir = dir;
    }

    public readonly Vector2 Start, End, Dir;
}

public enum AnimationKey {
    IDLE,
    WALK,
    RUN,
    JUMP,
    LAND,
    FALL,
}  

public struct FrameInput {
    public float X;
    public float Y;
    public bool JumpDown;
    public bool JumpUp;
}

