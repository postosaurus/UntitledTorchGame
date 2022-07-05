using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFov {
    
    public IInputController InputController { get; }
    [SerializeField] public float moveDelta { get; }
    [SerializeField] public float moveAmount  { get; }
    public Vector3 _lastDirection { get; } 
    public float angleX { get; }
    public float angleY { get; }

    public float fov { get; }

    float CalculateFOV();
    Vector3 CalculateOrigin();
    float CalcutlareViewDistance();
    float CalculateAngle();

}
