using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugMovementScriptable : MonoBehaviour {
    
    [SerializeField] private GlowwormTypeSO _glowwormTypeSO;    
    private Vector3 _spawnPoint;
    private SpriteRenderer _spriteRenderer;
    private Transform _safeSpace;

    private float _ran;

    public bool _isSafe = false;
    public bool _isPanic = false;

    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start() {        
        _spriteRenderer.color = _glowwormTypeSO.color;      
        _spawnPoint = new Vector3(transform.position.x, transform.position.y);  

        BehaviourRandomizer();
    }

    private void Update() {   
        // if the bug is safe, gather and/or fly around?   
        if (_isSafe) {
            FlyAroundMovement();
            Gather(_safeSpace); 
            return;  
        }

        // keep the badbugs in their range, otherwise they scatter all around the area
        if (_glowwormTypeSO.type == Target.Enemy) {
            var distance = Vector3.Distance(transform.position, _spawnPoint);            
            if (distance > _glowwormTypeSO.detectionRange) _isPanic = true;
            if (distance < _glowwormTypeSO.keepDistance) { 
                _isPanic = false;                
            }
        }
        //when they have panic badbugs go back to their spawnpoint
        if (_isPanic) { 
            GoTo(_spawnPoint);
            return;                   
        }

        // BehaviourRandomizer();

        //bug is not safe or an enemy
        Collider2D[] hitCollider2DArray = Physics2D.OverlapCircleAll(transform.position, _glowwormTypeSO.detectionRange);
        foreach(Collider2D hitCollider in hitCollider2DArray) {

            if (hitCollider != null) {

                // // First we check if there is a safespace around
                if (hitCollider.GetComponent<ColliderTypeHolder>() != null &&
                    hitCollider.GetComponent<ColliderTypeHolder>().type == Target.Safe) {

                        ColliderTypeHolder target = hitCollider.GetComponent<ColliderTypeHolder>();                        
                        DoBehavior(target.type, hitCollider.gameObject.transform);
                        // return;
                
                //then we check if there is any other bug to flee from or be attracted to
                } else if (hitCollider.GetComponent<BugMovementScriptable>() != null &&
                        hitCollider.GetComponent<BugMovementScriptable>().gameObject != gameObject) {     
                                                    
                            BugMovementScriptable targetBug = hitCollider.GetComponent<BugMovementScriptable>();

                            if (_glowwormTypeSO.type == Target.Enemy && targetBug._isSafe) { 
                                GoTo(_spawnPoint); 
                                continue;
                            }

                            DoBehavior(targetBug._glowwormTypeSO.type , targetBug.transform);
                            // return;

                // then check for the player, do the same as for the safespace
                } else if (hitCollider.GetComponent<ColliderTypeHolder>() != null &&
                    hitCollider.GetComponent<ColliderTypeHolder>().type == Target.Player) {            
                        
                        ColliderTypeHolder target = hitCollider.GetComponent<ColliderTypeHolder>();                
                        DoBehavior(target.type, hitCollider.gameObject.transform);       
                        // return;
                        
                // fly aroud as default
                } else FlyAroundMovement();                    

            // not collision, fly normal    
            } else FlyAroundMovement();
        }
    }

    private void DoBehavior(Target targetType, Transform targetTransform) {
        if (_glowwormTypeSO.flee == targetType) {
            Flee(targetTransform);  
            return;
        } else if (_glowwormTypeSO.chase == targetType) {
            Chase(targetTransform);    
            return;        
        } else if (_glowwormTypeSO.avoid == targetType) {
            Avoid(targetTransform);                   
            return;
        } else if (_glowwormTypeSO.outOfRange == targetType) {
            MoveOutOfRange(targetTransform); 
            return;
        } else if (_glowwormTypeSO.gather == targetType) {
            Gather(targetTransform);                                        
            return;
        } else if (_glowwormTypeSO.tempt == targetType) {
            Tempt(targetTransform);                       
            return;
        } else if (_glowwormTypeSO.follow == targetType) {
            StickTo(targetTransform);
            return;
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        
        // // when a friendly bug is in the box skip the collision check
        if (_isSafe) return;

        //Check which type the object has
        if  (collider.GetComponent<ColliderTypeHolder>() != null) {
            Target colliderType = collider.GetComponent<ColliderTypeHolder>().type;            

            //collided with safe space
            if (colliderType == Target.Safe) {


                if (_glowwormTypeSO.type == Target.Friend) {
                    this._isSafe = true;
                    _safeSpace = collider.transform;

                // bad bug inside the safe space
                } else if (_glowwormTypeSO.type == Target.Enemy) {
                    Debug.Log("Bad Bug inside the safespace!");                                  
                    _isPanic = true;
                }
            }
        }
        return;
    }

    #region movement
        private Vector3 GetFlyAroundMovmentDirection() {
            float x = Mathf.Sin( (Time.time + _ran) * _glowwormTypeSO.frequency) * _glowwormTypeSO.amplitude;
            float y = Mathf.Cos(Time.time + (_glowwormTypeSO.frequency / _ran)) * (_glowwormTypeSO.amplitude / 2);
            return new Vector3(x, y).normalized;
        }

        private void FlyAroundMovement() {
            Vector3 dir = GetFlyAroundMovmentDirection();
            transform.position += dir * _glowwormTypeSO.moveSpeed * Time.deltaTime;
        }

        private void BehaviourRandomizer() {
            //hardcoded values!!!
            _ran = Random.Range(-3f, 3f);
        }

        private void GoTo(Vector3 targetPosition) {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, (_glowwormTypeSO.moveSpeed * 2) * Time.deltaTime);
        }

    #endregion

    #region follow
        private void StickTo(Transform target) {
            Vector3 dir = GetFlyAroundMovmentDirection();
            Vector3 pull = target.position - transform.position;
            
            transform.position += (dir + pull) * _glowwormTypeSO.moveSpeed * Time.deltaTime;            
        }

        private void Gather(Transform target) {        
            Vector3 pull = target.position - transform.position;
            
            transform.position += pull * _glowwormTypeSO.moveSpeed * Time.deltaTime;
        }

        private void Tempt(Transform target) {
            Vector3 dir = GetFlyAroundMovmentDirection();
            Vector3 pull = (target.position - transform.position).normalized;
                    
            transform.position += (dir + pull) * _glowwormTypeSO.moveSpeed * Time.deltaTime;
        }

        private void Chase(Transform target) {       
            Vector3 pull = (target.position - transform.position).normalized;
            transform.position += (pull) * _glowwormTypeSO.moveSpeed*2 * Time.deltaTime;
        }
    #endregion

    #region avoid
        private void Flee(Transform target) {
            Vector3 pull = Vector3.MoveTowards(transform.position, target.position, _glowwormTypeSO.moveSpeed * Time.deltaTime).normalized;
            Vector3 dir = GetFlyAroundMovmentDirection();

            // targetTransform to the opposite side
            transform.position += dir - pull * _glowwormTypeSO.moveSpeed * Time.deltaTime;
        }

        private void Avoid(Transform target) {
            Vector3 pull = Vector3.MoveTowards(transform.position, target.transform.position, _glowwormTypeSO.moveSpeed * Time.deltaTime).normalized;
            Vector3 dir = GetFlyAroundMovmentDirection();

            // targetTransform to the opposite side
            transform.position += (dir + pull) * _glowwormTypeSO.moveSpeed * Time.deltaTime;
        }

        private void MoveOutOfRange(Transform target) {
            Vector3 pull = Vector3.MoveTowards(transform.position, target.transform.position, _glowwormTypeSO.moveSpeed * Time.deltaTime).normalized;
            Vector3 dir = GetFlyAroundMovmentDirection();
            transform.position += (pull) * _glowwormTypeSO.moveSpeed * Time.deltaTime;    
        }

    #endregion
}
