using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PhysicsPlayerController : MonoBehaviour
{
    // 2D Player controller based on physics (rigidbody2D)
    // This is a simple controller that can move left and right and jump.

    public Transform feet;

    public float moveSpeed = 20f;
    public float jumpForce = 200f;
    public float jumpHeight = 2f;
    public float jumpTime = 0.5f;

    //max acceleration of the player
    public float maxAcceleration = 10f;
    //max speed of the player
    public float maxSpeed = 10f;
    //max speed of the player when jumping
    public float maxJumpSpeed = 3f;

    //deceleration of the player when not moving
    public float deceleration = 10f;

    //downward force applied to the player when they let go of the jump button
    public float downForceOnJumpRelease = 5f;

    bool isJumping = false;
    bool isGrounded = false;

    private void Update()
    {

        // Double check if the player is grounded


        // --- HORIZONTAL MOVEMENT ---

        // Move left and right
        float x = Input.GetAxisRaw("Horizontal");
        Vector2 move = new Vector2(x * moveSpeed * GetComponent<Rigidbody2D>().mass, 0);
        //accelerate the player twice as fast when their velocity is in the opposite direction of the input
        if (Mathf.Sign(x) != Mathf.Sign(GetComponent<Rigidbody2D>().velocity.x))
        {
            move *= 2;
        }
        //add force to the rigidbody until the max speed is reached
        if (Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) < maxSpeed)
        {
            GetComponent<Rigidbody2D>().AddForce(move);
        }


        //deceleration and friction
        //if axis is 0, the player is not moving, so add force in the opposite direction to slow down
        if (x == 0)
        { 
            GetComponent<Rigidbody2D>().AddForce(new Vector2(-GetComponent<Rigidbody2D>().velocity.x * deceleration, 0));
            //set the friction of the capsule collisder to 0 so that the player can stick to surfaces
            GetComponent<CapsuleCollider2D>().sharedMaterial.friction = 1f;
        } 
        else 
        {
              //set the friction of the capsule collisder to 1 so that the player can slide
            GetComponent<CapsuleCollider2D>().sharedMaterial.friction = 0.0f;
        }

        //if the player is not grounded, check if they are close enough to the ground to be considered grounded
        if (!isGrounded && !isJumping)
        {
            //check if the player is touching something below them without raycast
            Collider2D[] colliders = Physics2D.OverlapCircleAll(feet.position, 0.2f);
            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject != gameObject)
                {
                    isGrounded = true;
                    //log the name of the colliders that the player is touching
                    Debug.Log(collider.gameObject.name);
                }
            }
        }

        // Jump
        if (Input.GetButtonDown("Jump") && !isJumping && isGrounded)
        {
            isJumping = true;
            isGrounded = false;
            //add upwards velocity to the rigidbody as addforce
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce * GetComponent<Rigidbody2D>().mass);

            //GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, jumpForce * GetComponent<Rigidbody2D>().mass);
            
            //get objects that are touching the player's feet that have a rigidbody and apply opposite jump force to them
            Collider2D[] colliders = Physics2D.OverlapCircleAll(feet.position, 0.1f);
            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject != gameObject && collider.GetComponent<Rigidbody2D>() != null)
                {
                    //collider.GetComponent<Rigidbody2D>().velocity = new Vector2(collider.GetComponent<Rigidbody2D>().velocity.x, -jumpForce * GetComponent<Rigidbody2D>().mass);
                    //add force to other rigidbody 
                    collider.GetComponent<Rigidbody2D>().AddForce(Vector2.down * jumpForce * GetComponent<Rigidbody2D>().mass);
                }
            }
            StartCoroutine(HoldDownJump(GetComponent<Rigidbody2D>().velocity.y));
        }
    }

    IEnumerator HoldDownJump(float oldUpVelocity)
    {
        //set the gravity scale to 0 so that the player does not fall while holding down the jump button
        GetComponent<Rigidbody2D>().gravityScale = 0;
        //while the jump button is held down, keep applying force to the rigidbody 
        float time = 0;
        while (Input.GetButton("Jump") && time < jumpTime)
        {
            time += Time.deltaTime;
            //gradually increase gravity scale so that the player falls faster, where the closer time is to jumpTime, the faster the player falls
            GetComponent<Rigidbody2D>().gravityScale = Mathf.Lerp(0, 1, time / jumpTime);
            yield return null;
        }
        //counteract the y velocity of the player when the let go so that the player's jump height is consistent
        //if the current velocity is greater than the old velocity, then the player is still moving up, so add force downwards
        if (GetComponent<Rigidbody2D>().velocity.y > oldUpVelocity)
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * -1 * (GetComponent<Rigidbody2D>().velocity.y - oldUpVelocity) * downForceOnJumpRelease  * GetComponent<Rigidbody2D>().mass);

        //set the gravity scale back to 1 so that the player falls
        GetComponent<Rigidbody2D>().gravityScale = 1;
        isJumping = false;
    }

}