using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateObject : MonoBehaviour {

    private IMovementController _movementController;
    private IInputController _inputController;

    private void Awake() {
        _movementController = GetComponentInParent<IMovementController>();
        _inputController = GetComponentInParent<IInputController>();

    } 

    private void Update() {
        Vector3 _moveVelocity = _movementController.Velocity;

        if (_moveVelocity != Vector3.zero) {
            GetComponent<SpriteRenderer>().color = Color.cyan;
        } else {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        Debug.Log(_moveVelocity);

        if( _inputController.GetJumpKeyDown() ) GetComponent<SpriteRenderer>().color = Color.red;
        else if ( _inputController.GetJumpKeyUp() ) GetComponent<SpriteRenderer>().color = Color.white;
        
    }
}
