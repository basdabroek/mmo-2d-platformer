using System;
using UnityEngine;

public class PlatformerCharacter2D : MonoBehaviour
{
    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up

    private float m_MaxSpeed = 10f;
    private float m_JumpForce = 400f;
    private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
    private Transform m_CeilingCheck;   // A position marking where to check for ceilings
    private Animator m_Anim;            // Reference to the player's animator component.
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private bool m_Jump;
    private float direction;
    private bool jumping;
    private bool crouching;
    private bool grounded;            // Whether or not the player is grounded.

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
    /// Fixeds the update.
    /// </summary>
    private void FixedUpdate()
    {
        Move();
        Jump();
    }

    /// <summary>
    /// Update this instance.
    /// </summary>
    void Update()
    {
        Animations();
        direction = Input.GetAxis("Horizontal"); // later in een user input class / inputmanager
        crouching = Input.GetKey(KeyCode.LeftControl);
        if (Input.GetButtonDown("Jump"))
            jumping = true;
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
        return crouch;
    }

    /// <summary>
    /// Updates the facing.
    /// </summary>
    private void UpdateFacing()
    {
        if ((direction > 0 && !m_FacingRight) || (direction < 0 && m_FacingRight)) {
            Flip();
        }
    }

    /// <summary>
    /// Jump this instance.
    /// </summary>
    private void Jump()
    {
        if (IsGrounded() && jumping) {
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            jumping = false;
        }
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
    private void Move()
    {
        if (direction != 0) {
            m_Rigidbody2D.velocity = new Vector2(direction * m_MaxSpeed, m_Rigidbody2D.velocity.y);
            UpdateFacing();
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
        m_Anim.SetFloat("Speed", Mathf.Abs(direction));
    }
}
