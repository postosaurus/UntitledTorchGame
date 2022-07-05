using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TightMovement : MonoBehaviour, IMovementController {
    
    //interface properties
    public Vector3 Velocity { get; private set; }
    public bool Grounded => _colliderController.IsColDown();
    public bool JumpingThisFrame { get; private set; }
    public bool LandingThisFrame { get; private set; }

    public void GetInput(FrameInput input) {
        _frameInput = input;
        if (input.JumpDown) {
            _lastJumpPressed = Time.time;
        }        
    }

    //eventlistener
    private void SUB_OnUpdateLastPosition(object sender, TightKeyInput.vector3Arg e) {
        this.Velocity = e.Velocity;
    }

    private void SUB_OnCoyoteIsUsable(object sender, EventArgs e) {
        _coyoteUsuable = true;           
    }

    private FrameInput _frameInput;
    private ICollider _colliderController;

    [Header("WALKING")]
    [SerializeField] private float _acceleration = 90f;
    [SerializeField] private float _deAcceleration = 60f;
    [SerializeField] private float _moveClamp = 13;
    [SerializeField] private float _apexBonus = 2f;

    [Header("MOVE")]
    [SerializeField] private int _freeColliderIterations = 10;

    [Header("GRAVITY")]
    [SerializeField] private float _fallClamp = -40;
    [SerializeField] private float _minFallSpeed = 80;
    [SerializeField] private float _maxFallSpeed = 160;
    private float _fallSpeed;

    [Header("JUMPING")]
    [SerializeField] private float _jumpHeight = 40f;
    [SerializeField] private float _jumpApexThreshold = 0.1f;
    [SerializeField] private float _coyoteTreshold = 0.1f;
    [SerializeField] private float _jumpEarlyGravityModifier = 3f; 
    [SerializeField] private float _jumpBuffer = 0.1f;

    private bool _coyoteUsuable;
    private bool _endedJumpEarly = true;
    private float _apexPoint;
    private float _lastJumpPressed;
    private bool CanUseCoyote => _coyoteUsuable && !_colliderController.IsColDown() && _timeLeftGrounded + _coyoteTreshold > Time.time;
    private bool HasBufferedJump => _colliderController.IsColDown() && _lastJumpPressed + _jumpBuffer > Time.time;

    private float _currentHorizontalSpeed, _currentVerticalSpeed;    
    private float _timeLeftGrounded;

    private bool _active;
    private void Awake() {
        Invoke(nameof(Activate), 0.5f);     
        _colliderController = GetComponent<ICollider>();
    }
    void Activate() => _active = true;

    private void Start() {
        TightKeyInput.OnUpdateLastPosition += SUB_OnUpdateLastPosition;
        TightCollsion.OnCoyoteIsUseable += SUB_OnCoyoteIsUsable;
    }

    private void Update() {
        if (!_active) return;
        _colliderController.RunCollisionChecks();
        LandingThisFrame = _colliderController.LandingThisFrame();
        CalculateWalk();

        CaclulateApexJump();
        CalculateGravity();
        CalculateJump();

        MovePlayer();
    }

    public void CaclulateApexJump() {
        //  Depenencies: Collider 
        if (!_colliderController.IsColDown()) {
            _apexPoint = UnityEngine.Mathf.InverseLerp(_jumpApexThreshold, 0, Mathf.Abs(Velocity.y));
            _fallSpeed = Mathf.Lerp(_minFallSpeed, _maxFallSpeed, _apexPoint);
        } else {
            _apexPoint = 0;            
        }
    }

    private void CalculateGravity() {
        if (_colliderController.IsColDown()) {
            if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
        } else {
            var fallSpeed = _endedJumpEarly && _currentVerticalSpeed > 0 ? _fallSpeed * _jumpEarlyGravityModifier : _fallSpeed;

            _currentVerticalSpeed -= fallSpeed * Time.deltaTime;

            if (_currentVerticalSpeed < _fallClamp) _currentVerticalSpeed = _fallClamp;
        }
    }

    private void CalculateJump() {
        if (_frameInput.JumpDown) {        
            _currentVerticalSpeed = _jumpHeight;
            _endedJumpEarly = false;
            _coyoteUsuable = false;        
            _timeLeftGrounded =_colliderController.GetTimeLeftGrounded();            

            JumpingThisFrame = true;
        } else {
            JumpingThisFrame = false;
            _endedJumpEarly = true;
        }

        if (!_colliderController.IsColDown() && _frameInput.JumpUp && !_endedJumpEarly && Velocity.y > 0) {        
            _endedJumpEarly = true;
        }
        
        if (_colliderController.IsColUp()) {
            if (_currentVerticalSpeed > 0) _currentVerticalSpeed = 0;
        }
    }

    private void CalculateWalk() {
        if (_frameInput.X != 0) {
            _currentHorizontalSpeed += _frameInput.X * _acceleration * Time.deltaTime;

            _currentHorizontalSpeed = UnityEngine.Mathf.Clamp(_currentHorizontalSpeed, -_moveClamp, + _moveClamp);

            var apexBonus = UnityEngine.Mathf.Sign(_frameInput.X) * _apexBonus * _apexPoint;
            _currentHorizontalSpeed += apexBonus * Time.deltaTime;
        } else {
            _currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _deAcceleration * Time.deltaTime);
        }

        if (_currentHorizontalSpeed > 0 && _colliderController.IsColRight() ||
             _currentHorizontalSpeed < 0 && _colliderController.IsColLeft()) {
            _currentHorizontalSpeed = 0;
        }
    }

    private void MovePlayer() {
        // var move = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed) * Time.deltaTime;
        // transform.position += move;

        var pos = transform.position + _colliderController.GetPlayerBounds().center;
        // RawMovement = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed); // Used externally
        // RawMovement = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed); // Used externally
        // var move = RawMovement * Time.deltaTime;
        var move = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed) * Time.deltaTime;
        var furthestPoint = pos + move;

        // check furthest movement. If nothing hit, move and don't do extra checks
        var hit = Physics2D.OverlapBox(furthestPoint, _colliderController.GetPlayerBounds().size, 0, _colliderController.GetGroundLayerMask());
        if (!hit) {
            transform.position += move;
            return;
        }

        // otherwise increment away from current pos; see what closest position we can move to
        var positionToMoveTo = transform.position;
        for (int i = 1; i < _freeColliderIterations; i++) {
            // increment to check all but furthestPoint - we did that already
            var t = (float)i / _freeColliderIterations;
            var posToTry = Vector2.Lerp(pos, furthestPoint, t);

            if (Physics2D.OverlapBox(posToTry, _colliderController.GetPlayerBounds().size, 0, _colliderController.GetGroundLayerMask())) {
                transform.position = positionToMoveTo;

                // We've landed on a corner or hit our head on a ledge. Nudge the player gently
                if (i == 1) {
                    if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
                    var dir = transform.position - hit.transform.position;
                    transform.position += dir.normalized * move.magnitude;
                }

                return;
            }

            positionToMoveTo = posToTry;
        }
    }


}
