using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TightAnimationController : MonoBehaviour {

    [SerializeField] private string _pathToResources;

    private IMovementController _movementController;
    private SpriteRenderer _spriteRenderer;

    private void Awake() {            
        _movementController = GetComponent<IMovementController>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        LoadAnimations();
    }

    private void Start() {
        PlayAnimation(AnimationKey.IDLE);            
        _movementController = GetComponentInParent<IMovementController>();
    }

    private void Update() {
    if (_movementController == null) return;

        if (_movementController.Velocity.x != 0) transform.localScale = new Vector3(_movementController.Velocity.x < 0 ? -1 : 1, 1, 1);

        if (_movementController.Grounded) {
            var vel = Mathf.Abs(_movementController.Velocity.x);
            if (vel == 0) PlayAnimation(AnimationKey.IDLE);
            else PlayAnimation(AnimationKey.RUN);
        
        } else {

            if (_movementController.JumpingThisFrame) return;
            if (_movementController.LandingThisFrame) PlayAnimation(AnimationKey.LAND);

            if (_movementController.Velocity.y > 0) {
                PlayAnimation(AnimationKey.JUMP);
            } else if (_movementController.Velocity.y < 0) {
                PlayAnimation(AnimationKey.FALL);
            }
        }

        AnimationUpdate();
        _spriteRenderer.sprite = _currentAnimation.frames[_index];
    }    


    #region animations    

    private Dictionary<AnimationKey, AnimationSO> _stateAnimationDictionary;        
    private AnimationSO _currentAnimation;
    private float _timer;
    private int _index;

    private void LoadAnimations() {
        _stateAnimationDictionary = new Dictionary<AnimationKey, AnimationSO>();

        Object[] obj = Resources.LoadAll(_pathToResources, typeof(AnimationSO));            
        foreach(AnimationSO animationSO in obj) {                
            _stateAnimationDictionary.Add(animationSO.key, animationSO);               
        }
    }

    private void AnimationUpdate() {
        if (_currentAnimation == null) return;
                    
        if (!_currentAnimation.isLooping && _index == _currentAnimation.frames.Length - 1) return;
        
        _timer -= Time.deltaTime;
        if (_timer <= 0) {
            _timer += _currentAnimation.interval;                
            _index++;                

            if (_index >= _currentAnimation.frames.Length - 1) {
                _index = 0;
            }
        }
    }

    private void PlayAnimation(AnimationKey animationKey) {
        _currentAnimation =_stateAnimationDictionary[animationKey];    
        if (_index > _currentAnimation.frames.Length - 1)_index = 0;
        if(_timer <= 0) _timer = _currentAnimation.interval;
        
    }
    #endregion

}
    

