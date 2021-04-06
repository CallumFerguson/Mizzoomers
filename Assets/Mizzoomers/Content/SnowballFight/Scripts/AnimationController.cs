using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    Animator animator;
    int isRunningHash;
    int isJumpingHash;
    int inAirHash;
    int throwHash;

    

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");
        inAirHash = Animator.StringToHash("inAir");
        throwHash = Animator.StringToHash("throw");
    }

    // Update is called once per frame
    void Update()
    {
        bool forward = Input.GetKey("w");
        bool right = Input.GetKey("d");
        bool left = Input.GetKey("a");
        bool backward = Input.GetKey("s");

        bool jump = Input.GetKey(KeyCode.Space);

        bool throwSnowball = Input.GetKey("f");

        bool isRunning = animator.GetBool(isRunningHash);
        bool isJumping = animator.GetBool(isJumpingHash);
        bool inAir = animator.GetBool(inAirHash);
        bool throwing = animator.GetBool(throwHash);



        
        if (throwSnowball && !throwing)
        {
            animator.SetBool(throwHash, true);
        }
        


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

        if((jump && isRunning) || (jump && !isRunning))
        {
            animator.SetBool(isJumpingHash, true);
            animator.SetBool(inAirHash, true);
        }

        if(inAir && isJumping)
        {
            animator.SetBool(isJumpingHash, false);
            animator.SetBool(inAirHash, false);
        }
        

        

    }

    
}
