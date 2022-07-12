using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Posty.Utils;

public class Bug_Movement : MonoBehaviour {

    private enum State {
        Idle,
        Follow
    }
    private State state = State.Idle;

    [Header("Move")]
    [SerializeField] private float _frequency = 2;
    [SerializeField] private float _amplitude = 10;
    [SerializeField] private float _moveSpeed = 3;

    [Header("Follow")]
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private float _detectionRange = 15f;
    [SerializeField] private float _keepDistance = 3f;

    private float _ran;

    private void Start() {
        BehaviourRandomizer();
    }

    private void Update() {
        float distance = Vector2.Distance(transform.position, _targetTransform.position);
        switch (state) {
            case State.Idle:
                if (distance < _detectionRange && distance > _keepDistance) state = State.Follow;
                
                    FlyAroundMovement();
            break;
            
            case State.Follow:
                if (distance > _detectionRange || distance <= _keepDistance) state = State.Idle;
                    Chase();
             
            break;
        }
        
    }

    private Vector3 GetFlyAroundMovmentDirection() {
        float x = Mathf.Sin( (Time.time + _ran) * _frequency) * _amplitude;
        float y = Mathf.Cos(Time.time + (_frequency / _ran)) * (_amplitude / 2);
        return new Vector3(x, y).normalized;
    }

    private void FlyAroundMovement() {
        Vector3 dir = GetFlyAroundMovmentDirection();
        transform.position += dir * _moveSpeed * Time.deltaTime;
    }

    private void BehaviourRandomizer() {
        _ran = Random.Range(-3f, 3f);
    }

    #region follow target
        private void StickTo() {
            Vector3 dir = GetFlyAroundMovmentDirection();
            Vector3 pull = _targetTransform.position - transform.position;
            
            transform.position += (dir + pull) * _moveSpeed * Time.deltaTime;
        }

        private void Gather() {        
            Vector3 pull = _targetTransform.position - transform.position;
            
            transform.position += pull * _moveSpeed * Time.deltaTime;
        }

        private void Tempt() {
            Vector3 dir = GetFlyAroundMovmentDirection();
            Vector3 pull = (_targetTransform.position - transform.position).normalized;
                    
            transform.position += (dir + pull) * _moveSpeed * Time.deltaTime;
        }

        private void Chase() {        
            Vector3 pull = (_targetTransform.position - transform.position).normalized;
            transform.position += (pull) * _moveSpeed*2 * Time.deltaTime;
        }
    #endregion

    #region avoid target
        private void Flee() {
            Vector3 pull = Vector3.MoveTowards(transform.position, _targetTransform.position, _moveSpeed * Time.deltaTime).normalized;
            Vector3 dir = GetFlyAroundMovmentDirection();

            // They flee from target to the opposite side
            transform.position += dir - pull * _moveSpeed * Time.deltaTime;
        }

        private void Avoid() {
            Vector3 pull = Vector3.MoveTowards(transform.position, _targetTransform.position, _moveSpeed * Time.deltaTime).normalized;
            Vector3 dir = GetFlyAroundMovmentDirection();

            // They flee from target to the opposite side
            transform.position += (dir + pull) * _moveSpeed * Time.deltaTime;
        }

        private void MoveOutOfRange() {
            Vector3 pull = Vector3.MoveTowards(transform.position, _targetTransform.position, _moveSpeed * Time.deltaTime).normalized;
            Vector3 dir = GetFlyAroundMovmentDirection();
            transform.position += (pull) * _moveSpeed * Time.deltaTime;    
        }
    #endregion
}
