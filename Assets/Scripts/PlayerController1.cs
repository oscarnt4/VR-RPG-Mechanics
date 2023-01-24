using System;
using System.Collections;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

[RequireComponent(typeof(XROrigin))]
[RequireComponent(typeof(ActionBasedContinuousMoveProvider))]
[RequireComponent(typeof(CharacterController))]

public class PlayerController1 : MonoBehaviour
{
    [Header("Controller Inputs")]
    [SerializeField] InputActionProperty rightThumbstick;
    [SerializeField] InputActionProperty sprintButton;
    [SerializeField] InputActionProperty moveAction;

    [Header("Camera")]
    [SerializeField] Camera mainCamera;

    XROrigin xrRig;
    ActionBasedContinuousMoveProvider moveProvider;
    CharacterController characterController;

    [Header("Movement")]
    [SerializeField] bool isSprinting;
    [SerializeField] bool isGrounded;
    [SerializeField] float walkSpeed = 3f;
    [SerializeField] float sprintSpeed = 5f;
    [SerializeField] float slopeForce = 5f;

    [Header("Jump")]
    [SerializeField] bool canJump;
    [SerializeField] float jumpHeight = 2f;

    [Header("Dash")]
    [SerializeField] bool canDash = true;
    [SerializeField] float dashDistance = 30f;
    [SerializeField] float dashTime = 0.2f;

    float playerVerticalVelocity;
    Vector2 rightStickVector;


    void Start()
    {
        xrRig = GetComponent<XROrigin>();
        moveProvider = GetComponent<ActionBasedContinuousMoveProvider>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        UpdateCollider();
        isGrounded = characterController.isGrounded || characterController.velocity.y == 0;
        rightStickVector = rightThumbstick.action.ReadValue<Vector2>();
        Jump();
        if (rightStickVector.y < -0.5 && canDash && isGrounded) StartCoroutine(Backdash());
        Sprint();
        ApplyGravity();
    }

    void UpdateCollider()
    {
        Vector3 center = xrRig.CameraInOriginSpacePos;
        characterController.center = new Vector3(center.x, xrRig.CameraInOriginSpaceHeight / 2, center.z);
        characterController.height = xrRig.CameraInOriginSpaceHeight;
    }

    void Jump()
    {
        if (isGrounded)
        {
            if (rightStickVector.y > 0.5)
            {
                playerVerticalVelocity = Mathf.Sqrt(jumpHeight * -Physics.gravity.y);
            }
            else
            {
                playerVerticalVelocity = 0;
            }
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

        RaycastHit slopeHit;
        OnSlope(out slopeHit);
        Vector3 dashDirection = Vector3.ProjectOnPlane(-new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z).normalized, slopeHit.normal).normalized;

        float startTime = Time.time;

        while (Time.time < startTime + dashTime)
        {
            characterController.Move(dashDirection * dashDistance / dashTime * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        canDash = true;
    }

    private bool OnSlope()
    {
        return OnSlope(out RaycastHit slopeHit);
    }

    private bool OnSlope(out RaycastHit slopeHit)
    {
        Physics.Raycast(transform.position, Vector3.down, out slopeHit, characterController.height / 2f + 0.3f);
        if (slopeHit.normal != Vector3.up)
        {
            return true;
        }
        return false;
    }

    private void ApplyGravity()
    {
        if (OnSlope() && rightStickVector.y <= 0.5)
        {
            characterController.Move(Vector3.down * characterController.height / 2f * slopeForce * Time.deltaTime);
        }
        else
        {
            playerVerticalVelocity += Physics.gravity.y * Time.deltaTime;
            characterController.Move(Vector3.up * playerVerticalVelocity * Time.deltaTime);
        }
    }

}

