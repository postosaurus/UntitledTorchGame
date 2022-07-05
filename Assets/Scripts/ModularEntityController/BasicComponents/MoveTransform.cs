using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTransform : MonoBehaviour, IMovementController {

    [SerializeField] private float moveSpeed = 10f;
    public Vector3 Velocity { get; private set; }

    public bool Grounded => throw new System.NotImplementedException();
    public bool JumpingThisFrame => throw new System.NotImplementedException();
    public bool LandingThisFrame => throw new System.NotImplementedException();

    private bool _jumpKeyDown;
    private bool _jumpKeyUp;


    private void Update() {    
        transform.position += Velocity * moveSpeed * Time.deltaTime;

        if (_jumpKeyDown) Debug.Log("JumpKey pressed");
        else if (_jumpKeyUp) Debug.Log("JumpKey released");
    }

    public void GetInput(FrameInput input) {        
        SetVelocity(new Vector3(input.X, input.Y));
        _jumpKeyDown = input.JumpDown;
        _jumpKeyUp = input.JumpUp;
    }

    private void SetVelocity(Vector3 vel) {
        Velocity = vel; 
    }

    public Vector3 GetVelocity() {
        return Velocity;
    }
}
