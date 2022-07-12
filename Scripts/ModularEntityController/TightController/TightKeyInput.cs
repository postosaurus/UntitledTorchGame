using System;
using UnityEngine;

//eventArgs
public class vector3Arg : EventArgs {
    public Vector3 Velocity;
}

public class TightKeyInput : MonoBehaviour, IInputController {

    //interface properties
    public FrameInput Input { get; private set; }

    //events
    public event EventHandler<vector3Arg> OnUpdateLastPosition;

    

    //class
    private IMovementController _moveController;

    public Vector3 Velocity;
    public Vector3 _lastPosition;


    private void Awake() {
        _moveController = GetComponent<IMovementController>();
    }

    private void Update() {

        Velocity = (transform.position - _lastPosition) / Time.deltaTime;
        _moveController.GetVelocity(Velocity);
        _lastPosition = transform.position;        
        

        Input = new FrameInput {
            X = UnityEngine.Input.GetAxisRaw("Horizontal"),
            Y = UnityEngine.Input.GetAxisRaw("Vertical"),
            JumpDown = UnityEngine.Input.GetButtonDown("Jump"),
            JumpUp = UnityEngine.Input.GetButtonUp("Jump"),            
        };

        _moveController.GetInput(Input);        
    }

    //interface methodes
    public bool GetJumpKeyDown() {
        return Input.JumpDown;
    }

    public bool GetJumpKeyUp() {
        return Input.JumpUp;
    }

    public Vector3 GetVelocity() {
        return Velocity;
    }

    public Vector3 GetLastPosition() {
        return _lastPosition;
    }
}
