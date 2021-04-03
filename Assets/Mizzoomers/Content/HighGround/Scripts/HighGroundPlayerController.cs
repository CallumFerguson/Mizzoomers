using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HighGroundPlayerController : NetworkBehaviour
{
    public static HighGroundPlayerController localPlayer;

    private Rigidbody _body;

    private const float TargetSpeed = 5f;
    private const float MaxVelocityChange = 1.25f;
    private const float MaxFallSpeed = 25f;

    private float _lastJumpTime = -JumpCooldown;
    private const float JumpCooldown = 0.5f;
    private const float JumpVelocity = 5f;

    private float _lastPushTime = -PushCooldown;
    private const float PushCooldown = 0.3f;
    private const float PushRadius = 3f / 2f;

    private float _lastBlockTime = -BlockCooldown;
    private const float BlockCooldown = 0.3f;

    private int _playerMask;
    private int _allButPlayerMask;

    private Animator _playerAnimator;
    private int _isRunningHash;
    private int _inAirHash;
    private int _isJumpingHash;

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

    public override void OnStartClient()
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
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        var grounded = Grounded();
        _playerAnimator.SetBool(_inAirHash, !grounded);

        //jump
        if (Input.GetKey(KeyCode.Space) && Time.time - _lastJumpTime >= JumpCooldown && grounded)
        {
            _lastJumpTime = Time.time;
            var velocity = _body.velocity;
            velocity.y = JumpVelocity;
            _body.velocity = velocity;
        }

        _playerAnimator.SetBool(_isJumpingHash, Time.time - _lastJumpTime < 0.85f && !grounded);

        //push
        if (Input.GetKeyDown(KeyCode.Return) && Time.time - _lastPushTime >= PushCooldown)
        {
            _lastPushTime = Time.time;
            CmdPushPlayers();
        }

        //block
        if (Input.GetKeyDown(KeyCode.RightShift) && Time.time - _lastBlockTime >= BlockCooldown)
        {
            _lastBlockTime = Time.time;
            CmdBlock();
        }


        var horizontalVelocity = _body.velocity;
        horizontalVelocity.y = 0;
        var mag = horizontalVelocity.magnitude;

        if (mag > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(horizontalVelocity);
        }

        _playerAnimator.SetBool(_isRunningHash, mag > 1f);
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
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

    [Command]
    private void CmdPushPlayers()
    {
        var hits = Physics.OverlapSphere(connectionToClient.identity.transform.position, PushRadius, _playerMask);
        for (int i = 0; i < hits.Length; i++)
        {
            var hit = hits[i].attachedRigidbody;
            if (hit)
            {
                var hitIdentity = hit.GetComponent<NetworkIdentity>();
                if (hitIdentity && hitIdentity != netIdentity)
                {
                    var hitController = hit.GetComponent<HighGroundPlayerController>();
                    if (hitController && hitIdentity.connectionToClient != null)
                    {
                        hitController.RpcGetPushed(hitIdentity.connectionToClient, transform.position);
                    }
                }
            }
        }
    }

    [TargetRpc]
    private void RpcGetPushed(NetworkConnection target, Vector3 position)
    {
        var distance = Vector3.Distance(position, transform.position);
        var forceLerp = 1 - (distance / PushRadius);
        forceLerp = Mathf.Max(forceLerp, 0.15f);
        var forceDirection = (transform.position - position).normalized;
        forceDirection.y += 0.5f;
        _body.AddForce(forceLerp * 1750f * forceDirection.normalized, ForceMode.Impulse);
    }

    [Command]
    private void CmdBlock()
    {
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

        //the camera is rotated 45 degrees, so this make the direction relative to the camera
        var rotationFix = Quaternion.AngleAxis(45f, Vector3.up);

        return rotationFix * targetVelocity;
    }
}