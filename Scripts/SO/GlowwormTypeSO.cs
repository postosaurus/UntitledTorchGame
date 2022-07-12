using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Target {
    None,
    Player,
    Enemy,
    Friend,
    Safe
    
}

[CreateAssetMenu(menuName = "ScriptableObjects/GlowwormTypeSO")]
public class GlowwormTypeSO : ScriptableObject {

    [Header("Appearence")]
    public string nameString;
    public Color color;
    public Target type;

    [Header("Move")]
    public float frequency = 2;
    public float amplitude = 10;
    public float moveSpeed = 3;

    [Header("Social Life")]
    public Target gather;
    public Target follow;
    public Target tempt;
    public Target flee;
    public Target avoid;
    public Target outOfRange;
    public Target chase;

    [Header("Detection")]
    public float detectionRange = 15f;
    public float keepDistance = 3f;
}
