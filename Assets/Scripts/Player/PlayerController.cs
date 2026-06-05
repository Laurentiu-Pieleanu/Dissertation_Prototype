using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Integrations.Match3;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CombatController combatController;
    public Transform target;
    public Transform cameraTransform;

    public float moveSpeed = 4f;
    public float dashForce = 15f;

    public float dashCD = 1.2f;

    public float dashDuration = 0.3f;

    private Rigidbody rb;
    private Animator animator;

    private Vector3 moveDirection;
    private bool isDashing = false;
    private float lastDashTime = -999f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        
        if(combatController == null)
            combatController = GetComponent<CombatController>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleMovementInput();
        HandleCombatInput();
        HandleDashInput();
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            Move();
        }
    }

    private void HandleMovementInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        
        cameraForward.y = 0;
        camRight.y = 0;
        
        cameraForward.Normalize();
        camRight.Normalize();
        
        moveDirection = (cameraForward * v + camRight * h).normalized;
        
        animator.SetBool("Walking", moveDirection.magnitude > 0f);
    }

    private void Move()
    {
        Vector3 newPosition = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);

        if (moveDirection.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }
    
    private void HandleCombatInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            combatController.StartLightAttack(target);
        }

        if (Input.GetMouseButtonDown(1))
        {
            combatController.StartHeavyAttack(target);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            combatController.Block();
        }
    }

    private void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            TryDash();
        }
    }

    private void TryDash()
    {
        if(Time.time - lastDashTime < dashCD)
            return;

        Vector3 dashDirection = moveDirection;
        
        if(dashDirection.sqrMagnitude < 0.001f)
            dashDirection = transform.forward;
        
        StartCoroutine(Dash(dashDirection));
    }

    private IEnumerator Dash(Vector3 direction)
    {
        isDashing = true;
        lastDashTime = Time.time;

        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            rb.MovePosition(rb.position + direction * dashForce * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
        
        isDashing = false;
    }
}
