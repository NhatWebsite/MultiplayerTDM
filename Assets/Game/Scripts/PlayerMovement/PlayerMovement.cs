using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class PlayerMovement : MonoBehaviour
{
    [Header("Play Health")]
    const float maxHealth = 150f;
    public float currentHealth;
    public Slider healthbarSlider;
    public GameObject playerUI;

    [Header("Ref & Physics")]



    InputManager inputManager;
    PlayerManager playerManager;
    PlayerControllerManager playerControllerManager;
    AnimatorManager animatorManager;
    Vector3 moveDirection;
    Transform cameragameObject;
    Rigidbody playerRigidbody;

    [Header("Falling and Landing")]
   [SerializeField] private float inAirTimer;
    [SerializeField] private float leapingVerlocity;
    [SerializeField] private float fallingVerlocity;
    [SerializeField] private float rayCastHeightOffset = 0.5f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement flags")]
    public bool isMoving;

    public bool isSprinting;
    public bool isGrounded;
    public bool isJumping;  

    [Header("Movement values")]
    public float movementSpeed = 2f;
    public float rotationSpeed = 13f;

    public float sprintingSpeed = 7f;
    [Header("Jump Var")]
    public float jumpHeight = 4f;
    public float gravityIntensity = -15f;

    PhotonView view;
    public int playerTeam;
    private void Awake()
    {
        currentHealth=maxHealth;
        animatorManager = GetComponent<AnimatorManager>();
        inputManager = GetComponent<InputManager>();
        playerManager = GetComponent<PlayerManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        cameragameObject = Camera.main.transform;
        view = GetComponent<PhotonView>();
        playerControllerManager = PhotonView.Find((int)view.InstantiationData[0]).GetComponent<PlayerControllerManager>();

        healthbarSlider.minValue = 0f;
        healthbarSlider.maxValue = maxHealth;
        healthbarSlider.value = currentHealth;
    }

    private void Start()
    {
        if (!view.IsMine)
        {
            Destroy(playerRigidbody);
            Destroy(playerUI);
        }
        if (view.Owner.CustomProperties.ContainsKey("Team"))
        {
            int team = (int)view.Owner.CustomProperties["Team"];
            playerTeam = team;
        }
    }
    public void HandleAllMovement()
    {
        HandleFallingAndLanding();
        if (playerManager.isInteracting)
        {
            return;
        }
        HandleMovement();
        HandleRotation();

    }
    //di chuyen trai phai
    public void HandleMovement()
    {
        if (isJumping) { return; }
        moveDirection = new Vector3(cameragameObject.forward.x,0f,cameragameObject.forward.z)*inputManager.verticalInput;
        moveDirection = moveDirection + cameragameObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();

        moveDirection.y = 0;

        if (isSprinting)
        {
            moveDirection = moveDirection * sprintingSpeed;
        }
        else
        {
            if (inputManager.movementAmount >= 0.5f)
            {
                moveDirection = moveDirection * sprintingSpeed;
                isMoving = true;
            }
            if (inputManager.movementAmount <= 0f)
            {
                
                isMoving = false;
            }
        }
        Vector3 movementVelocity = moveDirection;
        playerRigidbody.velocity = movementVelocity;

    }
    //chuyen dong xoay nguoi
    public void HandleRotation()
    {
        if (isJumping) { return; }
        Vector3 targetDirection = Vector3.zero;
        targetDirection = cameragameObject.forward * inputManager.verticalInput;
        targetDirection= targetDirection + cameragameObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;
        
        if(targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation,targetRotation, rotationSpeed*Time.deltaTime);

        transform.rotation = playerRotation;
    }
    void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        Vector3 targetPosition;
        rayCastOrigin.y = rayCastOrigin.y + rayCastHeightOffset;
        targetPosition = transform.position;
        if (!isGrounded && isJumping)
        {
            if (!playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnim("Falling", true);

            }
            inAirTimer = inAirTimer + Time.deltaTime;
            playerRigidbody.AddForce(transform.forward*leapingVerlocity);
            playerRigidbody.AddForce(-Vector3.up * fallingVerlocity * inAirTimer);

        }
        if(Physics.SphereCast(rayCastOrigin, 0.2f,-Vector3.up, out hit, groundLayer))
        {
            if(!isGrounded && !playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnim("Landing", true);
            }
            Vector3 rayCasthitPoint = hit.point;
            targetPosition.y = rayCasthitPoint.y;
            inAirTimer = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        if (isGrounded && !isJumping)
        {
            if (playerManager.isInteracting || inputManager.movementAmount > 0)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
            }
            else {
                transform.position = targetPosition;
            }
        }
    }

    public void HandleJumping()
    {
        if (isGrounded)
        {
            animatorManager.animator.SetBool("isJumping", true);
            animatorManager.PlayTargetAnim("Jump", false);

            float jumpingVelocity =Mathf.Sqrt(-2* gravityIntensity*jumpHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y=jumpingVelocity;
            playerRigidbody.velocity= playerVelocity;

            isJumping = false;  
        }
    }

    public void SetIsJumping(bool isJumping) {
        
        this.isJumping = isJumping;
    }

    public void ApplyDamage(float damageValue)
    {
        Debug.Log("+++Da co dame " + damageValue);
        view.RPC("RPC_TakeDamage", RpcTarget.All, damageValue);
    }
    [PunRPC]
    void RPC_TakeDamage(float damage)
    {

        if (!view.IsMine) {
        
            return;
        }

        currentHealth-=damage;
        Debug.Log("+++CurrentHealth " + currentHealth);
        healthbarSlider.value = currentHealth;
        if(currentHealth <= 0)
        {
            Die();
        }

        Debug.Log("Damage Taken" + damage);
        Debug.Log("current Health" + currentHealth );
    }
    private void Die() {
        playerControllerManager.Die();
        ScoreBoard.instance.PlayerDied(playerTeam);
    }
}
