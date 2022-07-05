using UnityEngine;
using Posty.Utils;

public class FieldOfVision : MonoBehaviour {
    
    private IMovementController _owner;
    private Transform _ownerTransform;

    [Header("References")]    
    [SerializeField] private LayerMask _blockLightMask;

    [Header("Settings")]
    [SerializeField] private int _rayCount = 50;
    [SerializeField] private float _fov = 90f;  
    [SerializeField] private float _viewDistance = 15f;    
    [SerializeField] private bool _getAngleFromMosueInput = false;
    [SerializeField] private bool _lookAtPlayer = false;
    [SerializeField] private bool _getAngleByVelocity = false;

    [Header("Debug")]
    [SerializeField] private float angleX;
    [SerializeField] private float angleY;
    [SerializeField] private float _startingAngle;
    [SerializeField] private Vector3 _origin;

    [SerializeField] [Range(0.1f, 0.9f)] private float moveDelta = 0.1f;
    [SerializeField] private float moveAmount = -100f;
    private Vector3 _lastDirection;    

    private Mesh _mesh;     
    private bool _switchTorch;
    private bool _switchTorchOn;
    private GameObject _player;
    
    private void Awake() { 
        _owner = GetComponentInParent<IMovementController>();               
        _ownerTransform = GetComponentInParent<Transform>();

        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
        if (gameObject.tag != "Player") _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start() {
    }

    private void Update() {
        SetOrigin();
        if (_getAngleFromMosueInput) SetAimingDirectionByMouse();
        else if (_lookAtPlayer) SetAimingDirectionByPlayer();
        else if (_getAngleByVelocity) SetAimingDirectionByVelocity();

        // if (_owner.Input.MouseDownRight) {
        //     _switchTorch = !_switchTorch;
        //     if (_switchTorch) {
        //         SetFOV(50);
        //         SetViewDistance(50);
        //     } else {
        //         SetFOV(90);
        //         SetViewDistance(25);
        //     }
        // } 

        // if (_owner.Input.MouseDownLeft) {
        //     _switchTorchOn = !_switchTorchOn;
        //     if (_switchTorchOn) {
        //         SetFOV(60);
        //         SetViewDistance(10);
        //     } else {
        //         SetFOV(0);
        //         SetViewDistance(0);
        //     }
        // }
    }

    private void LateUpdate() {        

        //offset is used to make this a child of some other gameobject
        Vector3 offsetPosition = transform.parent.position;

        float _angle = _startingAngle;
        float _angleIncrease = _fov / _rayCount;
        
        Vector3[] _verticies    = new Vector3[_rayCount + 1 + 1];
        Vector2[] _uv           = new Vector2[_verticies.Length];
        int[] _triangles        = new int[_rayCount * 3];

        _verticies[0] = _origin - offsetPosition; 

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= _rayCount; i++) {
            Vector3 vertex;
            RaycastHit2D hit = Physics2D.Raycast(_origin, UtilsClass.GetVectorFromAngle(_angle), _viewDistance, _blockLightMask);            

            if (hit.collider == null) {
                vertex = (_origin - offsetPosition) + UtilsClass.GetVectorFromAngle(_angle) * _viewDistance;
            } else {
                vertex = hit.point - (Vector2)offsetPosition;
            }

            _verticies[vertexIndex] = vertex;

            if (i > 0) {
                _triangles[triangleIndex + 0] = 0;
                _triangles[triangleIndex + 1] = vertexIndex - 1;
                _triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }
            vertexIndex++;

            _angle -= _angleIncrease;
        }

        _mesh.vertices = _verticies;
        _mesh.uv = _uv;
        _mesh.triangles = _triangles;
    }

        private void SetFOV(float fov) {
        this._fov = fov;
    }

    private void SetViewDistance(float viewDistance) {
        this._viewDistance = viewDistance;
    }

    private void SetOrigin() {                
        _origin = _ownerTransform.position;
        
    }

    private void SetAimingDirectionByPlayer() {        
        Vector3 dist = new Vector3(_player.transform.position.x - transform.position.x, _player.transform.position.y - transform.position.y);
        Vector3 dir = new Vector3 (dist.x, dist.y).normalized;                
        _startingAngle = UtilsClass.GetAngleFromVectorFloat( dir ) + _fov / 2f;        
    }

    private void SetAimingDirectionByMouse() { 
        Vector3 dir = UtilsClass.GetMouseWorldPosition() - _ownerTransform.position;
        _startingAngle = UtilsClass.GetAngleFromVectorFloat( dir ) + _fov / 2f;        
    }

    private void SetAimingDirectionByVelocity() {
        
        // float angleX;
        //Velocity needs maybe to be normalized
        Debug.Log(_owner.Velocity);
        if (_owner.Velocity.x != 0) angleY = _owner.Velocity.x;
        else angleX = _lastDirection.x;

        float angleXMove = 0;
        if (_owner.Velocity.y != 0) {
            angleXMove = _owner.Velocity.y * moveAmount;            
        }
        // else {
        //     angleXMove = -moveAmount;
        // }
        
        angleX =  Mathf.MoveTowards(0, angleXMove, moveDelta);
        angleX = Mathf.Clamp(angleX, -moveAmount, moveAmount);
        Vector3 dir = new Vector3(angleY, angleX);
        _startingAngle = UtilsClass.GetAngleFromVectorFloat( dir ) + (_fov / 2);

        _lastDirection = new Vector3(angleX, angleY);
    }
}
