using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderManager : MonoBehaviour {

    [SerializeField] private Transform _targetTransform;
    [SerializeField] private Material _material;

    private void Update() {
        _material.SetVector("_targetPosition", _targetTransform.position);
    }

}
