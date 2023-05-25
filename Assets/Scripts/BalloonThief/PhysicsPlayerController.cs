using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsPlayerController : MonoBehaviour
{
    // 2D Player controller based on physics (rigidbody2D)
    // This is a simple controller that can move left and right and jump.

    public Transform feet;

    public float moveSpeed = 20f;
    public float jumpForce = 8f;
    public float jumpHeight = 2f;
    public float jumpTime = 0.5f;
    bool isJumping;
    bool isGrounded;

    private void Update()
    {


          // Move left and right
        float x = Input.GetAxisRaw("Horizontal");
        Vector2 move = new Vector2(x * moveSpeed * GetComponent<Rigidbody2D>().mass, 0);
        //if axis is 0, the player is not moving, so add force in the opposite direction to slow down
        if (x == 0)
        {
            //log 
            Debug.Log("not moving");
            //GetComponent<Rigidbody2D>().AddForce(new Vector2(-GetComponent<Rigidbody2D>().velocity.x, 0));
            //set the friction of the capsule collisder to 0 so that the player can stick to surfaces
            GetComponent<CapsuleCollider2D>().sharedMaterial.friction = 1f;
        } else
        {
              //set the friction of the capsule collisder to 1 so that the player can slide
            GetComponent<CapsuleCollider2D>().sharedMaterial.friction = 0.0f;
        }

        //add force to the rigidbody
        GetComponent<Rigidbody2D>().AddForce(move);

        //if the player is not grounded, check if they are close enough to the ground to be considered grounded
        if (!isGrounded)
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
            //add upwards velocity to the rigidbody
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, jumpForce * GetComponent<Rigidbody2D>().mass);
            //get objects that are touching the player's feet that have a rigidbody and apply opposite jump force to them
            Collider2D[] colliders = Physics2D.OverlapCircleAll(feet.position, 0.1f);
            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject != gameObject && collider.GetComponent<Rigidbody2D>() != null)
                {
                    collider.GetComponent<Rigidbody2D>().velocity = new Vector2(collider.GetComponent<Rigidbody2D>().velocity.x, -jumpForce * GetComponent<Rigidbody2D>().mass);
                }
            }
            StartCoroutine(HoldDownJump());
        }
    }

    IEnumerator HoldDownJump()
    {
        //while the jump button is held down, keep applying force to the rigidbody
        float time = 0;
        while (Input.GetButton("Jump") && time < jumpTime)
        {
            time += Time.deltaTime;
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce);
            yield return null;
        }
        isJumping = false;
    }

}