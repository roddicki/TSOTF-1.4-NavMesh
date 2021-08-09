using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]

public class LocomotionSimpleAgentTwo : MonoBehaviour
{
    Animator anim;
    NavMeshAgent agent;
    Vector3 worldDeltaPosition;
    Vector2 groundDeltaPosition;
    Vector2 velocity = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
    }

    // Update is called once per frame
    void Update()
    {
        worldDeltaPosition = agent.nextPosition - transform.position;
        groundDeltaPosition.x = Vector3.Dot(transform.right, worldDeltaPosition);
        groundDeltaPosition.y = Vector3.Dot(transform.forward, worldDeltaPosition);
        //velocity = (Time.deltaTime > 1e-5f) ? groundDeltaPosition / Time.deltaTime : velocity = Vector2.zero;

        if(Time.deltaTime > 1e-5f) 
        {
            velocity = groundDeltaPosition / Time.deltaTime;
        }
        else {
            velocity = Vector2.zero;
        }

        bool shouldMove = velocity.magnitude > 0.5f && agent.remainingDistance > agent.radius;

		// Update animation parameters
		anim.SetBool("move", shouldMove);
		anim.SetFloat ("velx", velocity.x);
		anim.SetFloat ("vely", velocity.y);
    }

    void OnAnimatorMove()
    {
        if(anim != null)
        {
            transform.position = agent.nextPosition - 0.1f*worldDeltaPosition;
        }
        
    }
}
