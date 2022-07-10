using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Posty;

[CreateAssetMenu(menuName = "ScriptableObjects/AnimationSO")]
public class AnimationSO : ScriptableObject {

    public AnimationKey key;
    public Sprite[] frames;
    public bool isLooping = true;
    public float interval = .12f;
}
