using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTypeHolder: MonoBehaviour {
    [SerializeField] public Target type;
    // public Transform transform;

    private void Awake() {
        // transform = gameObject.transform;
    }
}
