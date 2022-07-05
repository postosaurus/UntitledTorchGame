using System;
using UnityEngine;

public class TightKeyInput : MonoBehaviour, IInputController {

    //interface properties
    public FrameInput Input { get; private set; }

    //events
    public static event EventHandler<vector3Arg> OnUpdateLastPosition;

    //eventArgs
    public class vector3Arg : EventArgs {
        public Vector3 Velocity;
    }

    

    //class
    private IMovementController _moveController;
    private IJumpController _jumpController;

    public Vector3 Velocity;
    public Vector3 _lastPosition;

    private void Awake() {
        _moveController = GetComponent<IMovementController>();
        _jumpController = GetComponent<IJumpController>();
    }

    private void Update() {

        Velocity = (transform.position - _lastPosition) / Time.deltaTime;
        _lastPosition = transform.position;
        OnUpdateLastPosition?.Invoke(this, new vector3Arg { Velocity = this.Velocity } );

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
}
