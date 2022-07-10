using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Posty.Utils;
using System;

public class NPC_Input_Follow : MonoBehaviour, IInputController
{
    private enum State {
        Idle,
        Follow,
    }
    private State state = State.Idle;

    public FrameInput Input { get; private set; }

    [SerializeField] private float _minDistance;
    [SerializeField] private float _maxDistance;    

    private IMovementController _movementController;
    private GameObject _player;

    public event EventHandler<vector3Arg> OnUpdateLastPosition;
    public event EventHandler<vector3Arg> OnNewUpdate;

    private void Awake() {
        _movementController = GetComponent<IMovementController>();
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update() {
        FolloHorizontalAxis();
        // float distance = Vector2.Distance(transform.position, _player.transform.position);

        // switch (state) {
        //     case State.Idle:
        //         if (distance < _maxDistance) {
        //             state = State.Follow;
        //         }
        //         _movementController.GetInput(new FrameInput { X = 0, JumpDown = false });                 
        //         break;
        //     case State.Follow:
        //         if (distance < _minDistance || distance > _maxDistance) {
        //             state = State.Idle;                    
        //         } else {
        //             Vector3 direction = (_player.transform.position - transform.position).normalized;                    
        //             // transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, 10 * Time.deltaTime);

        //             _movementController.GetInput(new FrameInput {
        //                 X = direction.x,
        //                 JumpDown = direction.y > 0
            
        //             });                
        //         }
        //         break;            
        // }      
    }

    private void FolloHorizontalAxis() {
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        Vector3 dir =  Vector3.zero;
        bool jump = false;

        //if the distance if to far or to close skip this
        if (distance > _minDistance && distance < _maxDistance) {
            // if (distance > _maxDistance) return;
            Debug.Log("following");
            
            dir = (_player.transform.position - transform.position).normalized;

            //if player is above entity jump
            if (dir.y > 0) jump = true;            
        } else {
            Debug.Log("Stop following");
            dir = Vector3.zero;
            jump = false;
        }

        _movementController.GetInput(new FrameInput {
            X = dir.x,
            JumpDown = jump
        });
        
        Debug.Log(distance);
    }

    private void FollowverticalAxis() {
        
    }
    
    public bool GetJumpKeyDown() {
        return false;
    }

    public bool GetJumpKeyUp() {
        return false;
    }
}
