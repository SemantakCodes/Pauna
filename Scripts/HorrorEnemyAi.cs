using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class HorrorEnemyAi : MonoBehaviour
{
    public enum AIState { Patrol, Idle, Stare, Chase, Investigate, Kill }
    
    [Header("Current State")]
    public AIState currentState = AIState.Patrol;

    [Header("References")]
    public Transform player;
    public Transform head; 
    public Animator enemyAnimator; 
    public Camera jumpscareCamera; // Drag the animated camera here
    
    [Header("Patrol & Idle Settings")]
    public Transform[] waypoints;
    public float patrolSpeed = 1.5f;
    public float minIdleTime = 2f;
    public float maxIdleTime = 5f;
    
    [Header("Chase & Kill Settings")]
    public float chaseSpeed = 5.5f;
    public float stareDuration = 1.2f;
    public float killDistance = 1.8f; 
    public string gameOverSceneName = "GameOverMenu"; 
    public float jumpscareDuration = 2.0f; 
    
    [Header("Senses (Raycast & Sight)")]
    public float detectionRadius = 15f;
    [Range(0, 360)] public float fieldOfViewAngle = 110f;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;

    [Header("Audio Settings")]
    public AudioClip[] footstepSounds;
    public float walkStepInterval = 0.6f;
    public float runStepInterval = 0.3f;
    public AudioClip ambientLoop;
    public AudioClip jumpscareScream;

    // Internal Variables
    private NavMeshAgent agent;
    private AudioSource audioSource;
    private int currentWaypointIndex;
    private float idleTimer;
    private float stareTimer;
    private float stepTimer;
    private Vector3 lastKnownPlayerPosition;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        if (head == null) head = transform;

        if (ambientLoop != null)
        {
            audioSource.clip = ambientLoop;
            audioSource.loop = true;
            audioSource.spatialBlend = 1f; 
            audioSource.Play();
        }

        GoToNextWaypoint();
    }

    private void Update()
    {
        if (currentState == AIState.Kill) return;

        // NEW: Update Animator with current speed
        if (enemyAnimator != null)
        {
            // agent.velocity.magnitude will be 0 when idle, 1.5 when walking, 5.5 when chasing
            enemyAnimator.SetFloat("Speed", agent.velocity.magnitude);
        }

        if (Vector3.Distance(transform.position, player.position) <= killDistance)
        {
            StartCoroutine(JumpscareRoutine());
            return;
        }

        HandleFootsteps();

        switch (currentState)
        {
            case AIState.Patrol:
                PatrolBehavior();
                CheckForPlayer();
                break;
            case AIState.Idle:
                IdleBehavior();
                CheckForPlayer();
                break;
            case AIState.Stare:
                StareBehavior();
                break;
            case AIState.Chase:
                ChaseBehavior();
                break;
            case AIState.Investigate:
                InvestigateBehavior();
                CheckForPlayer();
                break;
        }
    }

    // --- BEHAVIORS ---

    private void PatrolBehavior()
    {
        agent.speed = patrolSpeed;
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentState = AIState.Idle;
            idleTimer = Random.Range(minIdleTime, maxIdleTime);
        }
    }

    private void IdleBehavior()
    {
        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0f) GoToNextWaypoint();
    }

    private void StareBehavior()
    {
        agent.isStopped = true;
        
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0; 
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToPlayer), Time.deltaTime * 10f);

        stareTimer -= Time.deltaTime;
        if (stareTimer <= 0f)
        {
            agent.isStopped = false;
            currentState = AIState.Chase;
        }
    }

    private void ChaseBehavior()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);

        if (!CanSeePlayer())
        {
            lastKnownPlayerPosition = player.position;
            currentState = AIState.Investigate;
        }
    }

    private void InvestigateBehavior()
    {
        agent.speed = patrolSpeed;
        agent.SetDestination(lastKnownPlayerPosition);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            idleTimer = 2f; 
            currentState = AIState.Idle; 
        }
    }

    private IEnumerator JumpscareRoutine()
    {
    currentState = AIState.Kill;
    agent.isStopped = true;

    // 1. Disable Player Movement
    MonoBehaviour playerMovement = player.GetComponent<MonoBehaviour>(); 
    if (playerMovement != null) playerMovement.enabled = false;

    // 2. SWITCH CAMERAS
    if (Camera.main != null) Camera.main.gameObject.SetActive(false); // Turn off player cam
    if (jumpscareCamera != null) jumpscareCamera.gameObject.SetActive(true); // Turn on monster cam

    // 3. Play Animation & Sound
    if (enemyAnimator != null) enemyAnimator.SetTrigger("Jumpscare");
    if (jumpscareScream != null) audioSource.PlayOneShot(jumpscareScream);

    // 4. Wait for the scare to finish
    yield return new WaitForSeconds(jumpscareDuration);

    SceneManager.LoadScene(gameOverSceneName); 
    }

    private void HandleFootsteps()
    {
        if (agent.velocity.magnitude > 0.1f && footstepSounds.Length > 0)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
                audioSource.PlayOneShot(clip, 0.5f);
                stepTimer = (currentState == AIState.Chase) ? runStepInterval : walkStepInterval;
            }
        }
        else
        {
            stepTimer = 0f; 
        }
    }

    private void GoToNextWaypoint()
    {
        if (waypoints.Length == 0) return;

        currentState = AIState.Patrol;
        int newIndex = Random.Range(0, waypoints.Length);
        if (newIndex == currentWaypointIndex && waypoints.Length > 1)
        {
            newIndex = (newIndex + 1) % waypoints.Length; 
        }
        
        currentWaypointIndex = newIndex;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    private void CheckForPlayer()
    {
        if (CanSeePlayer())
        {
            currentState = AIState.Stare;
            stareTimer = stareDuration;
            agent.ResetPath(); 
        }
    }

    private bool CanSeePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRadius)
        {
            Vector3 targetPosition = player.position + Vector3.up * 1f; 
            Vector3 directionToPlayer = (targetPosition - head.position).normalized;
            
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleToPlayer < fieldOfViewAngle / 2f)
            {
                if (Physics.Raycast(head.position, directionToPlayer, out RaycastHit hit, detectionRadius, playerLayer | obstacleLayer))
                {
                    if (((1 << hit.collider.gameObject.layer) & playerLayer) != 0)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}