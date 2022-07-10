using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TightCollsion : MonoBehaviour , ICollider{

    //events
    // public static event EventHandler OnCoyoteIsUseable; 

    [Header("COLLSION")]
    [SerializeField] private Bounds _playerBounds;
    [SerializeField] private LayerMask _groundLayer;        
    [SerializeField] private int _detectorCount = 3;
    [SerializeField] private float _detectionRayLength = 0.1f;
    [SerializeField] [Range(0.1f, 0.3f)] private float _rayBuffer = 0.1f;

    private RayRange _raysUp, _raysDown, _raysLeft, _raysRight;
    private bool _colUp, _colDown, _colLeft, _colRight;        

    private float _timeLeftGrounded;
    private bool _landingThisFrame;

    private IMovementController _movementController;

    private void Awake() {
        _movementController = GetComponent<IMovementController>();
    }

    public void RunCollisionChecks() {
        CalculateRayRanged();

        
        _landingThisFrame = false;
        var groundedCheck = RunDetection(_raysDown);
        if (_colDown && !groundedCheck) _timeLeftGrounded = Time.time;
        else if (!_colDown && groundedCheck) {

            //event
            // OnCoyoteIsUseable?.Invoke(this, EventArgs.Empty);
            _movementController.TriggerCoyote();


            _landingThisFrame = true;
        }

        _colDown = groundedCheck;
        _colLeft = RunDetection(_raysLeft);
        _colRight = RunDetection(_raysRight);
        _colUp = RunDetection(_raysUp);        

        bool RunDetection(RayRange range) {
            return EvaluateRayPositions(range).Any(point => Physics2D.Raycast(point, range.Dir, _detectionRayLength, _groundLayer));
        }   
    }

    public void CalculateRayRanged() {
        Bounds b = new Bounds(transform.position + _playerBounds.center, _playerBounds.size);

        _raysDown = new RayRange(b.min.x + _rayBuffer, b.min.y, b.max.x - _rayBuffer, b.min.y, Vector2.down);
        _raysUp = new RayRange(b.min.x + _rayBuffer, b.max.y, b.max.x - _rayBuffer, b.max.y, Vector2.up);
        _raysLeft = new RayRange(b.min.x, b.min.y + _rayBuffer, b.min.x, b.max.y - _rayBuffer, Vector2.left);
        _raysRight = new RayRange(b.max.x, b.min.y + _rayBuffer, b.max.x, b.max.y - _rayBuffer, Vector2.right);
    }

    private IEnumerable<Vector2> EvaluateRayPositions(RayRange range) {
        for (var i = 0; i < _detectorCount; i++) {
            var t = (float)i / (_detectorCount - 1);
            yield return Vector2.Lerp(range.Start, range.End, t);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + _playerBounds.center, _playerBounds.size);

        if (!Application.isPlaying) {
            CalculateRayRanged();
            Gizmos.color = Color.blue;
            foreach (RayRange range in new List<RayRange> {_raysDown, _raysLeft, _raysRight, _raysUp}) {
                foreach(var point in EvaluateRayPositions(range)) {
                    Gizmos.DrawRay(point, range.Dir * _detectionRayLength);
                }
            }                    
        }

        // if(!Application.isPlaying) return;

        // Gizmos.color = Color.red;
        // var move = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed) * Time.deltaTime;
        // Gizmos.DrawWireCube(transform.position + _playerBounds.center + move, _playerBounds.size);
    }

    public bool IsColDown() {
        return _colDown;
    }

    public bool IsColUp() {
        return _colUp;
    }

    public bool IsColRight() {
        return _colRight;
    }

    public bool IsColLeft() {
        return _colLeft;
    }

    public float GetTimeLeftGrounded() {
        return float.MinValue;
    }

    public Bounds GetPlayerBounds() {
        return _playerBounds;
    }

    public LayerMask GetGroundLayerMask() {
        return _groundLayer;
    }

    public bool LandingThisFrame() {
        return _landingThisFrame;
    }
}
