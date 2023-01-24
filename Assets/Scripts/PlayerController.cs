using System;
using System.Collections;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ActionBasedContinuousMoveProvider))]
public class PlayerController : MonoBehaviour
{
    [Header("Controller Inputs")]
    [SerializeField] InputActionProperty rightThumbstick;
    [SerializeField] InputActionProperty sprintButton;
    [SerializeField] InputActionProperty moveAction;
    [SerializeField] InputActionProperty grabButton;

    [Header("Camera")]
    [SerializeField] Camera mainCamera;

    XROrigin xrRig;
    Rigidbody myRigidbody;
    CapsuleCollider myCapsuleCollider;
    ActionBasedContinuousMoveProvider moveProvider;
    Vector2 stickVector;
    CharacterController characterController;

    [Header("Walk/Sprint")]
    [SerializeField] bool isSprinting;
    [SerializeField] float walkSpeed = 3f;
    [SerializeField] float sprintSpeed = 5f;

    [Header("Jump")]
    [SerializeField] bool canJump;
    [SerializeField] float jumpForce = 500f;

    [Header("Dash")]
    [SerializeField] bool canDash = true;
    [SerializeField] float dashPower = 30f;
    [SerializeField] bool isGrounded;


    void Start()
    {
        xrRig = GetComponent<XROrigin>();
        moveProvider = GetComponent<ActionBasedContinuousMoveProvider>();
        myRigidbody = GetComponent<Rigidbody>();
        myCapsuleCollider = GetComponent<CapsuleCollider>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        UpdateCollider();
        isGrounded = OnGround();
        stickVector = rightThumbstick.action.ReadValue<Vector2>();
        Jump();
        if (stickVector.y < -0.5 && canDash && isGrounded) StartCoroutine(Backdash());
        Sprint();
    }

    void UpdateCollider()
    {
        Vector3 center = xrRig.CameraInOriginSpacePos;
        myCapsuleCollider.center = new Vector3(center.x, xrRig.CameraInOriginSpaceHeight / 2, center.z);
        myCapsuleCollider.height = xrRig.CameraInOriginSpaceHeight;
    }

    void Jump()
    {
        if (!isGrounded) { canJump = true; return; }
        if (isGrounded && canJump && stickVector.y > 0.5)
        {
            canJump = false;
            myRigidbody.AddForce(Vector3.up * jumpForce);
        }
    }

    void Sprint()
    {
        if (sprintButton.action.ReadValue<float>() > 0 && isGrounded)
        {
            isSprinting = true;
        }
        if (isSprinting)
        {
            float moveValueY = moveAction.action.ReadValue<Vector2>().y;
            if (moveValueY > 0.9)
            {
                moveProvider.moveSpeed = sprintSpeed;
            }
            else
            {
                moveProvider.moveSpeed = walkSpeed;
                isSprinting = false;
            }
        }
    }

    private IEnumerator Backdash()
    {
        canDash = false;
        Vector3 dashDirection = -new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z).normalized;
        myRigidbody.velocity = dashDirection * dashPower;
        yield return new WaitForSeconds(0.2f);
        myRigidbody.velocity = new Vector3(0, 0, 0);
        yield return new WaitForSeconds(0.5f);
        canDash = true;
    }

    private bool OnGround()
    {
        float playerHeight = myCapsuleCollider.height;
        float playerRadius = myCapsuleCollider.radius;
        // return Physics.Raycast(new Vector3(transform.position.x, transform.position.y + myCapsuleCollider.height, transform.position.z), Vector3.down, myCapsuleCollider.height);
        return Physics.SphereCast(this.transform.position + Vector3.up * (playerHeight - playerRadius), playerRadius, Vector3.down, out RaycastHit hitInfo, playerHeight - 2 * playerRadius);
    }

    private bool OnSlope()
    {
        return true;
    }

}

