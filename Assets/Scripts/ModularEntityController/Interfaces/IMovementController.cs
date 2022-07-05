using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementController {

    public bool Grounded { get; }
    public bool JumpingThisFrame { get; }
    public bool LandingThisFrame { get; }

    public Vector3 Velocity { get; }
    void GetInput(FrameInput input);
}
