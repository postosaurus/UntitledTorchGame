using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class KeyInput : MonoBehaviour, IInputController {
    
    public FrameInput Input { get; private set; }
    
    public bool GetJumpKeyDown() {
        return Input.JumpDown;
    }

    public bool GetJumpKeyUp() {
        return Input.JumpUp;
    }


    private IMovementController _moveController;

    private void Awake() => _moveController = GetComponent<IMovementController>();

    private void Update() {
        Input = new FrameInput {
            X = UnityEngine.Input.GetAxisRaw("Horizontal"),
            Y = UnityEngine.Input.GetAxisRaw("Vertical"),
            JumpDown = UnityEngine.Input.GetButtonDown("Jump"),
            JumpUp = UnityEngine.Input.GetButtonUp("Jump"),            
        };

        _moveController.GetInput(Input);
        
    }


}
