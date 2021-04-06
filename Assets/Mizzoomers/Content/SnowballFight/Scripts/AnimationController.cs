using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    Animator animator;
    int isRunningHash;
    // int isJumpingHash;
    int inAirHash;
    int throwHash;

    public ThirdPersonMovement controller;
    public Transform groundCheck;

    
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isRunningHash = Animator.StringToHash("isRunning");
        // isJumpingHash = Animator.StringToHash("isJumping");
        inAirHash = Animator.StringToHash("inAir");
        throwHash = Animator.StringToHash("throw");
    }

    // Update is called once per frame
    void Update()
    {
        var inAir = !Physics.CheckBox(groundCheck.position, groundCheck.localScale / 2f, groundCheck.rotation);
        animator.SetBool(inAirHash, inAir);

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical);

        var running = direction.magnitude > 0.01f;
        animator.SetBool(isRunningHash, running);

        if (Input.GetKeyDown("f"))
        {
            animator.SetBool(throwHash, true);
            StartCoroutine(DisableThrow());
        }

        IEnumerator DisableThrow()
        {
            yield return new WaitForSeconds(1.0f);
            animator.SetBool(throwHash, false);
        }

        
    }
}