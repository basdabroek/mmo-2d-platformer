using System;
using UnityEngine;
using System.Collections;

public class PlatformerCharacter2D : MonoBehaviour
{

    public bool UpdateCharacter { get; private set; }

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up

    private float m_MaxSpeed = 10f;
    private float m_JumpForce = 40f;
    private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
    private Transform m_CeilingCheck;   // A position marking where to check for ceilings
    private Animator m_Anim;            // Reference to the player's animator component.
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private bool m_Jump;
    private bool jumping;
    private bool crouching;
    private bool grounded;            // Whether or not the player is grounded.

    private float moveValue;

    void Start()
    {
        InputManager.instance.Horizontal += Move;
        InputManager.instance.Jump += Jump;
        InputManager.instance.Crouch += Crouch;
        InputManager.instance.Stand += Stand;

        Resume(); // start character loop
    }


    /// <summary>
    /// Awake this instance.
    /// </summary>
    private void Awake()
    {
        m_GroundCheck = transform.Find("GroundCheck"); // refactorable finding the minmax collider y value
        m_CeilingCheck = transform.Find("CeilingCheck");
        m_Anim = GetComponent<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

   
    /// <summary>
    /// Update this instance.
    /// </summary>
    private IEnumerator UpdateLoop()
    {
        while(UpdateCharacter)
        {
            Animations();
            yield return null;
        }
    }

    public void Pause()
    {
        UpdateCharacter = false;
    }

    public void Resume()
    {
        UpdateCharacter = true;
        StartCoroutine(UpdateLoop());
    }
     

    /// <summary>
    /// Determines whether this instance is grounded.
    /// </summary>
    /// <returns><c>true</c> if this instance is grounded; otherwise, <c>false</c>.</returns>
    public bool IsGrounded()
    {
        grounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius);
        for (int i = 0; i < colliders.Length; i++) {
            if (colliders[i].gameObject != gameObject)
                grounded = true;
        }
        return grounded;
    }

    /// <summary>
    /// Determines whether this instance is crouching.
    /// </summary>
    /// <returns><c>true</c> if this instance is crouching; otherwise, <c>false</c>.</returns>
    public bool IsCrouching()
    {
        bool crouch = false;
        if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius) || crouching) {
            crouch = true;
        }
        return crouching;
    }

    /// <summary>
    /// Updates the facing.
    /// </summary>
    private void UpdateFacing()
    {
        if ((moveValue > 0 && !m_FacingRight) || (moveValue < 0 && m_FacingRight)) {
            Flip();
        }
    }

    /// <summary>
    /// Jump method
    /// </summary>
    private void Jump()
    {
        if (IsGrounded()) {
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
    }

    /// <summary>
    /// Crouch method
    /// </summary>
    private void Crouch()
    {
        crouching = true;
    }


    private void Stand()
    {
        crouching = false;
    }

    private void Idle()
    {

    }

    /// <summary>
    /// Flip this instance.
    /// </summary>
    private void Flip()
    {
        m_FacingRight = !m_FacingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    /// <summary>
    /// Move this instance.
    /// </summary>
    private void Move(float val)
    {
        if (val != 0) {
            m_Rigidbody2D.velocity = new Vector2(val * m_MaxSpeed, m_Rigidbody2D.velocity.y);
            moveValue = val;
            UpdateFacing();
        }
        else {
            moveValue = 0;
        }
    }

    /// <summary>
    /// Animations this instance.
    /// </summary>
    private void Animations()
    {
        m_Anim.SetBool("Ground", IsGrounded());
        m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);
        m_Anim.SetBool("Crouch", IsCrouching());
        m_Anim.SetFloat("Speed", Mathf.Abs(moveValue));
    }
}
