using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    Animator animator;
    int isRunningHash;
    int isJumpingHash;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");
    }

    // Update is called once per frame
    void Update()
    {
        bool forward = Input.GetKey("w");
        bool right = Input.GetKey("d");
        bool left = Input.GetKey("a");
        bool backward = Input.GetKey("s");

        bool jump = Input.GetKey("space");

        bool isRunning = animator.GetBool(isRunningHash);
        bool isJumping = animator.GetBool(isJumpingHash);

        if (forward && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
        }

        if (!forward && isRunning)
        {
            animator.SetBool(isRunningHash, false);
        }

        if (right)
        {
            animator.SetBool(isRunningHash, true);
        }

        if (left)
        {
            animator.SetBool(isRunningHash, true);
        }

        if (backward)
        {
            animator.SetBool(isRunningHash, true);
        }

        if (jump && !isJumping)
        {
            animator.SetBool(isJumpingHash, true);
        }

    }
}
