using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRigidBody : MonoBehaviour
{
    Rigidbody rbody;
    public float SpeedAcrossGround;
    // character animation
    AnimateMe AnimationController;
    float TimeCounter = 0;
    public bool Standstill;

    // Start is called before the first frame update
    void Start() {
        rbody = this.GetComponent<Rigidbody>();
        AnimationController = GetComponent<AnimateMe>();
        SpeedAcrossGround = 6f;
        Standstill = false;
    }

    // Update is called once per frame
    void FixedUpdate() {
        TimeCounter += Time.deltaTime;
        float horizontal = Mathf.Cos(TimeCounter);
        float vertical = Mathf.Sin(TimeCounter);

        // get up and down arrow key input
        if(Input.GetAxis ("Horizontal") > 0f || Input.GetAxis ("Vertical") > 0f) {
            horizontal = Input.GetAxis ("Horizontal");
            vertical = Input.GetAxis ("Vertical");
        }
        
        // Set velocity
        Vector3 fVelocity = new Vector3 (horizontal, 0, vertical);

        // stand still if checked / true
        if(Standstill){
            fVelocity = Vector3.zero;
        }

        // orient rigidbody toward movment
        if(fVelocity != Vector3.zero) {
            transform.forward = fVelocity;
        }

        

        // set velocity of rigidbody
        rbody.velocity = fVelocity * SpeedAcrossGround;

        // run the amination script to animate character
        AnimationController.UpdateAnimator(AnimationController.CalculateSpeed());
    }
}
