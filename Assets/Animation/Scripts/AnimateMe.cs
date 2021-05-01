using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateMe : MonoBehaviour
{
	Animator anim;
	public bool IsPanting;
	public bool IsPushing;
	public float AnimationSpeed;
	public float Health;
	public float Direction;
	private int PronePose;
	private int PantingPose;
	public float AnimationSpeedMultiplier;
	private Vector3 PreviousFramePosition;
	// rotation variables 
	Vector3 oldForward;
	Rigidbody rBody;

	// Start is called before the first frame update
	void Start(){	
		oldForward = transform.forward;
		// needs an animator component to work
		rBody = GetComponent<Rigidbody>();
		anim = GetComponent<Animator>();
		PreviousFramePosition = transform.localPosition;
		AnimationSpeedMultiplier = 0.15f;
		AnimationSpeed = 0f;
		// set prone pose
		PronePose = Random.Range (1, 4);
		PantingPose = Random.Range(0,3);
	}

	void FixedUpdate(){
		// update the animation movement
		//calculateDirection();
		//UpdateAnimator(CalculateSpeed());
		Panting();

		// pushing
		Pushing();
	}

	// calculate turning left or right
	public float calculateDirection() {
		Vector3 cross = Vector3.Cross(oldForward, transform.forward);

		if (cross.y > 0f) {
			Direction = 1.0f;
			// if stationary don't push just turn
			// This doesn't really work as the character is not trully stationary when turning
			if (rBody.velocity == Vector3.zero){
				IsPushing = false;
			}
			
		} 
		else if (cross.y < 0f) {
			Direction = -1.0f;
			// if stationary don't push just turn
			// This doesn't really work as the character is not trully stationary when turning
			if (rBody.velocity == Vector3.zero){
				IsPushing = false;
			}
		} 
		else {
			Direction = 0.0f;
		}    
    	oldForward = transform.forward;
		return Direction;
	}

	// calculate speed from distance travelled 
	public float CalculateSpeed(){
		// calculate speed from distance moved since last update
		float movementPerFrame = Vector3.Distance(PreviousFramePosition, transform.localPosition);
		AnimationSpeed = (movementPerFrame / Time.deltaTime) * AnimationSpeedMultiplier;
		PreviousFramePosition = transform.localPosition;
		//Debug.Log (Speed);
		return AnimationSpeed;
	}

	// speed should be between 0-1
	// animations on the controlller do not use root motion so the character is only moved by physics
	// there are no exit times on the animations so they respond immediately to the speed condition on each transition
	public void UpdateAnimator(float AnimationSpeed){
		anim.SetFloat("Speed", AnimationSpeed);
		// Animation thresholds (in Animator > Speed)
		// > 0.03 walk > 0.6 > run
		//
		// adjust the speed of the animation - default is 1
		// walking
		// 0.1 > 0.3speed (*4) - speed up the walking results in 0.4 > 1.2 speed 
		// 0.4 > 1.2speed // 0.5 > 1.5speed
		// running
		// 0.7 > 0.5speed // 0.9 >1speed
		//if(AnimationSpeed > 0.03 && AnimationSpeed < 0.9) {
		//anim.speed = AnimationSpeed*5f;
		//} else if(AnimationSpeed >= 0.9) {
		//anim.speed = AnimationSpeed*1f;
		//} else {
		//anim.speed = 1;
		//}
	}

	// make character kneel and pant
	public void Panting() {
		if (IsPanting) {
			anim.SetFloat("IsPantingFloat", PantingPose);
			anim.SetBool("IsPanting", true);
		}
		else {
			anim.SetBool("IsPanting", false);
		}
	}

	// make character push
	public void Pushing() {
		if (IsPushing) {
			anim.SetBool("IsPush", true);
		}
		else {
			anim.SetBool("IsPush", false);
		}
	}

	// animate direction of character in idle state 
	public void UpdateAnimatorDirection(float Direction) {
		anim.SetFloat("Direction", Direction);
	}

	// make character prone
	public void MakeProne () {
		// send int 1-4 for prone poses
		anim.SetInteger("IsProneInt", PronePose);
	}
}