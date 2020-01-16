using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CombatStats))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AgentLinkMover))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemyAttack))]
public class EnemyAI : MonoBehaviour 
{
	[SerializeField]
	private float visionDistance = 0f;
	[SerializeField]
	private float roamDistance = 0f;

	[SerializeField]
	private AIState state;
    private AIState previousState;
    [SerializeField]
	private Vector3 originPosition;

    //TODO: Combat stats should only really be accessed through enemy attack
	private CombatStats combatStats;

	//TODO: Build this into a generic action system that will perform functionality based on state
	private float actionPauseTime = 0f;
	private float actionTimer = 0f;

    private float initialStoppingDistance;
	private NavMeshAgent agent;
	private Animator anim;

	private GameObject playerObject;

	void Start () 
	{
		originPosition = transform.position;
		anim = this.GetComponent<Animator> ();
		combatStats = this.GetComponent<CombatStats> ();
		agent = this.GetComponent<NavMeshAgent> ();
		agent.SetDestination(this.GetRandomDestination ());
        initialStoppingDistance = agent.stoppingDistance;
		actionPauseTime = Random.Range (3, 7);
		playerObject = GameObject.FindGameObjectWithTag ("Player");
        transform.Find("FloatingCanvas").gameObject.SetActive(false);
    }

    void Update()
    {
        if (GameObject.FindGameObjectWithTag("Player")
            .GetComponent<CombatStats>().CurrentHealth <= 0)
        {
            this.state = AIState.Roaming;
        }

        if (combatStats.CurrentHealth <= 0)
        {
            this.state = AIState.Dead;
        }

        if(combatStats.CurrentHealth != combatStats.MaxHealth)
        {
            transform.Find("FloatingCanvas").gameObject.SetActive(true);
        }
    }


	void FixedUpdate () 
	{
		switch (state) 
		{
		case AIState.Roaming:
			RunRoamingState ();
			break;
		case AIState.Following:
			RunFollowingState ();
			break;
		case AIState.Attacking:
			RunAttackingState ();
			break;
        case AIState.Dead:
            RunDeadState();
            break;
		}

		anim.SetFloat ("Forward", agent.velocity.magnitude);
	}

	private void RunRoamingState()
	{
		Vector3 destination = agent.destination;
		if (transform.position.DistanceXZ (destination) <= agent.stoppingDistance) 
		{
			actionTimer += Time.fixedDeltaTime;
			if (actionTimer >= actionPauseTime)
			{
				agent.SetDestination (this.GetRandomDestination ());
				actionPauseTime = Random.Range (3, 7);
				actionTimer = 0.0f;
			}
		}

		this.LookForPlayer ();
	}

	private void LookForPlayer()
	{
		if (Vector3.Distance (transform.position, playerObject.transform.position) < visionDistance) 
		{
			Vector3 directionToPlayer = playerObject.transform.position - transform.position;
			RaycastHit hit;
			if (Physics.Raycast (transform.position, directionToPlayer, out hit, visionDistance, 1 << 10 | 1 << 9)) 
			{
				if (hit.collider.CompareTag ("Player")) 
				{
                    state = AIState.Following;
                    agent.destination = hit.collider.transform.position;					
				}	
			}
		}
	}

	private void RunFollowingState()
	{
		//Check distance from player
		float distance = Vector3.Distance (transform.position, playerObject.transform.position);
		if (distance > visionDistance) 
		{
			agent.destination = originPosition;
            agent.stoppingDistance = initialStoppingDistance;
            state = AIState.Roaming;
			return;
		} 
		else if (distance <= combatStats.AttackDistance) 
		{
			agent.destination = transform.position;
            agent.stoppingDistance = combatStats.AttackDistance;
            state = AIState.Attacking;
		} 
		else 
		{
			agent.destination = playerObject.transform.position;
		}
	}

	private void RunAttackingState()
	{
		float distance = Vector3.Distance (transform.position, playerObject.transform.position);
		if (distance > combatStats.AttackDistance) 
		{
			state = AIState.Following;
			return;
		}

        transform.LookAt(playerObject.transform);
        this.GetComponent<EnemyAttack>().Attack();	
	}

    private bool killed = false;
    private void RunDeadState()
    {
        if(!killed)
        {
            killed = true;
            agent.velocity = Vector3.zero;
            anim.SetTrigger("Die");                  
            Destroy(this.gameObject, 5f);
        }
    }

	private Vector3 GetRandomDestination()
	{
		Vector3 randomPoint = originPosition + (Random.insideUnitSphere * roamDistance);
   
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 10f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        
        return GetRandomDestination();
	}


	public AIState GetState()
	{
		return state;
	}
		
	public enum AIState
	{
		Roaming,
		Following,
		Attacking,
        Dead
	}
}