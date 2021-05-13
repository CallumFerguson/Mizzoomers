using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour
{
    [SyncVar] public NetworkIdentity owner;
    
    public static PlayerController localPlayer;
    private Rigidbody _body;
    public Transform cam;

    private const float TargetSpeed = 5f;
    private const float MaxVelocityChange = 1.25f;
    private const float MaxFallSpeed = 25f;

    private float _lastJumpTime = -JumpCooldown;
    private const float JumpCooldown = 1f;
    private const float JumpVelocity = 5f;

    private int _playerMask;
    private int _allButPlayerMask;

    private Animator _playerAnimator;
    private int _isRunningHash;
    private int _inAirHash;
    private int _isJumpingHash;
    private float _inAirTimer;

    public Transform follow;
    public Transform lookAt;

    /*    private int _isStunnedHash;*/

    /*    private float timeSinceStunned;
        private float stunTime = 0.5f;
        private bool hit = false;*/

    void Start()
    {
        _body = GetComponent<Rigidbody>();
        _playerMask = LayerMask.GetMask("Player");
        _allButPlayerMask = ~_playerMask;
        _playerAnimator = GetComponentInChildren<Animator>();
        _isRunningHash = Animator.StringToHash("isRunning");
        _inAirHash = Animator.StringToHash("inAir");
        _isJumpingHash = Animator.StringToHash("isJumping");
       
    }
    
    private IEnumerator LookForStartPosition()
    {
        Transform startPosition;
        do
        {
            startPosition = NetworkManagerGame.singleton.GetStartPosition();
            yield return null;
        } while (startPosition == null);
        transform.position = startPosition.position;
        transform.rotation = startPosition.rotation;
    }

    public override void OnStartClient()
    {
        if (!owner.isLocalPlayer)
        {
            return;
        }
        
        cam = GameObject.Find("ObstacleCamera").transform;
        StartCoroutine(LookForStartPosition());

        var look = GameObject.Find("TPCamera").GetComponent<CinemachineFreeLook>();
        look.Follow = follow;
        look.LookAt = lookAt;
    }

    /*    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!_body)
            _body = GetComponent<Rigidbody>();

        _body.isKinematic = !isLocalPlayer;

        if (!isLocalPlayer)
        {
            return;
        }

        localPlayer = this;
    }*/

    void Update()
    {
/*        if (!isLocalPlayer)
        {
            return;
        }*/

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (!owner.isLocalPlayer)
        {
            return;
        }

        var grounded = Grounded();
        _playerAnimator.SetBool(_inAirHash, !grounded);
        _inAirTimer += Time.deltaTime;
        if (grounded)
        {
            _inAirTimer = 0;
        }

        //jump
        if (Input.GetKey(KeyCode.Space) && Time.time - _lastJumpTime >= JumpCooldown && grounded)
        {
            _lastJumpTime = Time.time;
            var velocity = _body.velocity;
            velocity.y = JumpVelocity;
            _body.velocity = velocity;
        }

        _playerAnimator.SetBool(_isJumpingHash, Time.time - _lastJumpTime < 0.85f && !grounded);

        var horizontalVelocity = _body.velocity;
        horizontalVelocity.y = 0;
        var mag = horizontalVelocity.magnitude;

        if (mag > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(horizontalVelocity);
        }

        _playerAnimator.SetBool(_isRunningHash, mag > 1f);
        
        //print(_playerAnimator.GetBool(_isRunningHash) + " " + _playerAnimator.GetBool(_inAirHash) + " " + _playerAnimator.GetBool(_isJumpingHash));
    }

    void FixedUpdate()
    {
        /*        if (!isLocalPlayer)
                {
                    return;
                }*/
        
        if (!owner.isLocalPlayer)
        {
            return;
        }

        var targetDirection = GetTargetDirection();
        targetDirection.y = 0;
        var targetVelocity = targetDirection * TargetSpeed;
        targetVelocity.y = 0;

        var velocityDifference = targetVelocity - _body.velocity;
        velocityDifference = Vector3.ClampMagnitude(velocityDifference, MaxVelocityChange);
        velocityDifference.y = 0;

        _body.velocity += velocityDifference;


        if (!Input.GetKey(KeyCode.Space) && !Grounded() || _body.velocity.y <= 0)
        {
            _body.velocity += 25f * Time.deltaTime * Vector3.down;
        }

        if (_body.velocity.y < -MaxFallSpeed)
        {
            _body.velocity = new Vector3(_body.velocity.x, -MaxFallSpeed, _body.velocity.z);
        }

        _body.angularVelocity = Vector3.zero;

    }

    private bool Grounded()
    {
        var center = transform.TransformPoint(new Vector3(0, -0.8f, 0));
        var halfExtends = new Vector3(0.25f, 0.05f, 0.25f);
        return Physics.CheckBox(center, halfExtends, Quaternion.identity, _allButPlayerMask,
            QueryTriggerInteraction.Ignore);
    }

    private Vector3 GetTargetDirection()
    {
        var targetVelocity = Vector3.zero;

        if (Input.GetKey(KeyCode.A))
            targetVelocity.x -= 1f;
        if (Input.GetKey(KeyCode.D))
            targetVelocity.x += 1f;
        if (Input.GetKey(KeyCode.S))
            targetVelocity.z -= 1f;
        if (Input.GetKey(KeyCode.W))
            targetVelocity.z += 1f;

        targetVelocity = Vector3.Normalize(targetVelocity);

        float targetAngle = Mathf.Atan2(targetVelocity.x, targetVelocity.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        return targetVelocity.magnitude>=0.1f ? moveDirection.normalized: Vector3.zero;
    }

    //Scrapping for now since the ball will still push the player/ block movement
    //When the rolling ball hits the character
/*    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "rollingBall")
        {
            Debug.Log("Player Stunned");
            hit = true;
            _playerAnimator.SetBool(_isStunnedHash, true);
        }
    }*/

}