using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mirror;
using UnityEngine;

public class ThirdPersonMovement : NetworkBehaviour
{
    [SyncVar] public NetworkIdentity owner;
    
    public CharacterController controller;
    

    // have camera follow player
    public Transform cam;

    public float speed = 8f;
    public float jumpHeight = 3f;

    [HideInInspector] public Vector3 velocity;
    public float gravity = -9.81f;

    // smooth out angles of rotation for player
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    // variables for keeping player on ground
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    bool isGrounded;

    // health variable initialization
    public int currentHealth = 0;
    public int maxHealth = 100;

    // public HealthBar healthBar;
    public CinemachineFreeLook freeLook;

    public Snowball snowball;

    public bool hit;

    public int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (!owner.isLocalPlayer)
        {
            return;
        }
        
        currentHealth = maxHealth;
        // healthBar.SetMaxHealth(maxHealth);
    }

    public override void OnStartClient()
    {
        if (!owner.isLocalPlayer)
        {
            return;
        }

        cam = GameObject.Find("Main Camera Snow").transform;
        freeLook = GameObject.Find("Third Person Camera").GetComponent<CinemachineFreeLook>();
        freeLook.Follow = transform;
        freeLook.LookAt = transform;

        StartCoroutine(LookForStartPosition());
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

    // Update is called once per frame
    void Update()
    {
        if (!owner.isLocalPlayer)
        {
            return;
        }
        
        //Debug.Log(currentHealth);
        // create a sphere at bottom of player to see if it hits the ground
        isGrounded = Physics.CheckBox(groundCheck.position, groundCheck.localScale / 2f, groundCheck.rotation);

        // check if on the ground
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");


        // normalize so diagonal movement is same speed
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if(direction.magnitude >= 0.1f)
        {
            // angle in radians so convert to degrees, point in direction player is traveling
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

            // angle of rotation for player
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // move in right direction while taking into account the rotation of the camera
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            
        }

        // implement gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // jumping
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        

        // test function works
        /*if (Input.GetKeyDown(KeyCode.B))
        {
            Damage(10);
        }
        */

        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        var camOn = Cursor.lockState == CursorLockMode.None ? 0 : 1;
        freeLook.m_XAxis.m_MaxSpeed = camOn * 450;
        freeLook.m_YAxis.m_MaxSpeed = camOn * 4;

        /*if (currentHealth == 0)
        {
            controller.enabled = false;
        }
        */
    }

    void OnCollisionEnter(Collision col)
    {
        if (!owner.isLocalPlayer)
        {
            return;
        }
        
        if (col.gameObject.name == "Snowball 1")
        {
            Damage(20);
            hit = true;
        }

        // get network identity and damage the player based off that identity
        // increase score for player that threw the snowball



        if(GetCurrentHealth() == 0 && gameObject.name == "Third Person Player")
        {
            StartCoroutine(Gameover());
        }

        IEnumerator Gameover()
        {
            yield return new WaitForSeconds(2.3f);
            Destroy(gameObject);
        }
    }

    


    public void Damage(int damage)
    {
        currentHealth -= damage;
        // healthBar.SetHealth(currentHealth);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}