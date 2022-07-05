using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Posty.Utils;

public class StaticFOV : MonoBehaviour {
   
    [Header("Settings")]
    [SerializeField] protected LayerMask _blockLightMask;
    [SerializeField] protected int _rayCount = 50;
    [SerializeField] [Range(0, 360)] protected float _fov = 180f;  
    [SerializeField] protected float _viewDistance = 15f;  

    [Header("Debug")]
    [SerializeField] protected float _startingAngle;
    [SerializeField] protected Vector3 _origin;

    protected Mesh _mesh;

    protected IMovementController _owner;
    // protected Transform _ownerTransform;

    protected virtual void Awake() { 
        _owner = GetComponentInParent<IMovementController>();               
        // _ownerTransform = GetComponentInParent<Transform>();

        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;        
    }

    protected virtual void LateUpdate() {  
        SetFOV(_fov);
        SetViewDistance(_viewDistance);
        SetOrigin();
        CreateMesh();        
    }

    protected virtual void SetFOV(float fov) {
        this._fov = fov;
    }

    protected virtual void SetViewDistance(float viewDistance) {
        this._viewDistance = viewDistance;
    }

    protected virtual void SetOrigin() {                
        _origin = transform.parent.position; //_ownerTransform.position;        
    }

    protected virtual void SetOrigin(Vector3 origin) {
        _origin = origin;
    }

    protected virtual void SetStartingAngle(float angle) {
        _startingAngle = angle;
    }

    protected virtual void CreateMesh() {
        // if (!Application.isPlaying)  _mesh = new Mesh();

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


}
