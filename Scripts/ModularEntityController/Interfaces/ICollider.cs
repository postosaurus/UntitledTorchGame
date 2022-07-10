using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollider {
    bool IsColDown();
    bool IsColUp();
    bool IsColRight();
    bool IsColLeft();
    bool LandingThisFrame();
    float GetTimeLeftGrounded();
    void RunCollisionChecks();
    Bounds GetPlayerBounds();
    LayerMask GetGroundLayerMask();
}
