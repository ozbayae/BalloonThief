using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonController : MonoBehaviour
{
    // Where the mouse clicks, create end attachment point for the balloon, and attach the balloon to it
    // the balloon and the attachment point are connected by a spring joint
    // if the object under the mouse is a valuable object, nest the attachment point under the valuable object, so that the balloon will carry the valuable object
    

    //balloon prefab
    public GameObject balloonPrefab;

    private void Update()
    {
        //check if the mouse is clicked
        if(Input.GetMouseButtonDown(0))
        {
            //get the mouse position
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //create a new game object at the mouse position
            GameObject newAttachmentPoint = new GameObject();
            //call it spawned attachment point
            newAttachmentPoint.name = "Spawned Attachment Point";
            //set z position to 1
            newAttachmentPoint.transform.position = new Vector3(mousePos.x, mousePos.y, 1);
            //add rigidbody to the new game object
            newAttachmentPoint.AddComponent<Rigidbody2D>();
            //set the position of the new game object to the mouse position
            //create a new balloon
            GameObject newBalloon = Instantiate(balloonPrefab);
            //set the position of the new balloon to the mouse position + 1 units up
            newBalloon.transform.position = mousePos + new Vector3(0, 1, 1);
            //set the balloon's spring joint's connected body to the new game object
            newBalloon.GetComponent<SpringJoint2D>().connectedBody = newAttachmentPoint.GetComponent<Rigidbody2D>();
            //set the balloon's spring joint's connected anchor to the new game object's position
            //newBalloon.GetComponent<SpringJoint2D>().connectedAnchor = newAttachmentPoint.transform.position;
            //set the balloon's spring joint's anchor to the balloon's position
            //newBalloon.GetComponent<SpringJoint2D>().anchor = newBalloon.transform.position;
            //disable auto configure distance of the balloon's spring joint
            newBalloon.GetComponent<SpringJoint2D>().autoConfigureDistance = false;
            //set the balloon's spring joint's distance to 2
            newBalloon.GetComponent<SpringJoint2D>().distance = 0.5f;

            //set the mass of the spawned attachment point to 0.1
            newAttachmentPoint.GetComponent<Rigidbody2D>().mass = 1f;
            //add new attachment point's mass to the balloon's mass
            newBalloon.GetComponent<Rigidbody2D>().mass += newAttachmentPoint.GetComponent<Rigidbody2D>().mass;

            //get balloon rope script
            Rope balloonRope = newBalloon.GetComponent<Rope>();
            //set the start of the rope to the new game object
            balloonRope.start = newAttachmentPoint.transform;
            //set the end of the rope to the balloon
            balloonRope.end = newBalloon.transform;


            //check if the object under the mouse is a valuable object
            if (Physics2D.OverlapPoint(mousePos, LayerMask.GetMask("Valuable")))
            {
                //if it is, set the new game object's parent to the object under the mouse
                newAttachmentPoint.transform.SetParent(Physics2D.OverlapPoint(mousePos, LayerMask.GetMask("Valuable")).transform);
                //make a fixed joint between the new game object and the object under the mouse
                HingeJoint2D fixedJoint = newAttachmentPoint.AddComponent<HingeJoint2D>();
                fixedJoint.connectedBody = Physics2D.OverlapPoint(mousePos, LayerMask.GetMask("Valuable")).GetComponent<Rigidbody2D>();
                //fixedJoint.connectedAnchor = Physics2D.OverlapPoint(mousePos, LayerMask.GetMask("Valuable")).transform.position;
                //fixedJoint.anchor = newAttachmentPoint.transform.position;
            }
        }
    }
}
