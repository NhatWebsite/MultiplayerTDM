using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerCotrolls playerCotrolls;
    AnimatorManager animatorManager;
    PlayerMovement playerMovement;
    public Vector2 movementInput;
    public Vector2 cameraMovementInput;
    public float verticalInput;
    public float horizontalInput;
    public float cameraInputX;
    public float cameraInputY;
    

    public float movementAmount;

    [Header("Input Buttons Flags")]
    public bool bInput;
    public bool jumpInput;
    public bool fireInput;
    public bool reloadInput;
    public bool scopeInput;
    //kiem tra player 
    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerMovement = GetComponent<PlayerMovement>();
    }
    private void OnEnable()
    {
        if(playerCotrolls == null)
        {
            playerCotrolls = new PlayerCotrolls();

            playerCotrolls.PlayerMomoment.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerCotrolls.PlayerMomoment.CameraMovement.performed += i => cameraMovementInput = i.ReadValue<Vector2>();
            playerCotrolls.PlayerActions.B.performed += i=>bInput = true;
            playerCotrolls.PlayerActions.B.canceled += i=>bInput = false;
            playerCotrolls.PlayerActions.Jump.performed += i => jumpInput = true;
            playerCotrolls.PlayerActions.Fire.performed += i => fireInput = true;
            playerCotrolls.PlayerActions.Fire.canceled += i => fireInput = false;
            playerCotrolls.PlayerActions.Reload.performed += i => reloadInput = true;
            playerCotrolls.PlayerActions.Scope.performed += i => scopeInput = true;
            playerCotrolls.PlayerActions.Scope.canceled += i => scopeInput = false;
            //PlayerActions.Scope.performed += i => scopeInput = false;

        }

        playerCotrolls.Enable();

    }
    private void OnDisable()
    {
        playerCotrolls.Disable();
    }

   public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleSprintingInput();
        HandleJumpingInput();
    }
    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraInputX = cameraMovementInput.x;
        cameraInputY = cameraMovementInput.y;

        movementAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animatorManager.ChangeAnimatorValue(0, movementAmount, playerMovement.isSprinting);
    }
    private void HandleSprintingInput()
    {
        if (bInput && movementAmount > 0.5)
        {
            playerMovement.isSprinting = true;
        }
        else
        {
            playerMovement.isSprinting = false;
        }
    }
    private void HandleJumpingInput()
    {
        if (jumpInput) {
            jumpInput= false;
            playerMovement.HandleJumping();


        }
    }

}
