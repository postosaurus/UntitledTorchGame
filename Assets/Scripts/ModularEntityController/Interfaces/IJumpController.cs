using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IJumpController {
    
    void GetInput(FrameInput input);

    void GetCurrentSpeed(Vector2 currentSpeed);
    float[] GetApexPointAndFallSpeed();
}
