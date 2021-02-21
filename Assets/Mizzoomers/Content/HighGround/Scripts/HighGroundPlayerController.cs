using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HighGroundPlayerController : NetworkBehaviour
{
    public static HighGroundPlayerController localPlayer;
    
    private Rigidbody _body;

    private const float TargetSpeed = 7.5f;
    private const float MaxVelocityChange = 1.25f;
    private const float MaxFallSpeed = 10f;

    private float _lastJumpTime;
    private const float JumpCooldown = 0.5f;
    private const float JumpVelocity = 10f;

    private int _allButPlayerMask;

    void Start()
    {
        _body = GetComponent<Rigidbody>();
        _allButPlayerMask = ~LayerMask.GetMask("Player");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        
        if(!_body)
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
        
        if (Input.GetKey(KeyCode.Space) && Time.time - _lastJumpTime >= JumpCooldown && Grounded())
        {
            _lastJumpTime = Time.time;
            var velocity = _body.velocity;
            velocity.y = JumpVelocity;
            _body.velocity = velocity;
        }
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
    }

    private bool Grounded()
    {
        var center = transform.TransformPoint(new Vector3(0, -1f, 0));
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