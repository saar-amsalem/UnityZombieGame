using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class zombieController : MonoBehaviour
{
    public Transform cameraTransform;
    public NavMeshAgent navMeshAgent;
    public float startWaitTime = 1;
    public float timeToRotate = 1;
    public float speedWalk = 2;
    public float speedRun = 4;
    public float viewRadius = 20;
    public float viewAngle = 90;
    public LayerMask playerMask;
    public LayerMask ObstacleMask;
    public float meshResolution = 1f;
    public int edgeIterations = 4;
    public float edgeDistance = 0.5f;
    public Transform[] waypoints;
    int m_CurrentWaypointIndex;
    Vector3 playerLastPosition = Vector3.zero;
    Vector3 m_PlayerPosition;

    float m_WaitTime;
    float m_TimeToRotate;
    bool m_PlayerInRange;
    bool m_PlayerNear;
    bool m_IsPatrol;
    bool m_CaughtPlayer;

    
    bool dead = false;

    // public float zombieSpeed = 3;
    // Vector3 dest;

    private void Awake()
    {
        GetComponent<Animation>().Play("Z_Idle");
        navMeshAgent = GetComponent<NavMeshAgent>();
        m_PlayerPosition = Vector3.zero;
        m_IsPatrol = true;
        m_CaughtPlayer = false;
        m_WaitTime = startWaitTime;
        m_TimeToRotate = timeToRotate;
        m_CurrentWaypointIndex = 0;
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedWalk;
        navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);

    }

    // Update is called once per frame
    void Update()
    {
        if(!dead) {

            EnviromentView();
            if(!m_IsPatrol){
                Chasing();
            }
            else{
                Patroling();
            }

            // if(Vector3.Distance(cameraTransform.position,transform.position) > 3f) {
            //     updateZombieMovement();
            // }
        }
    }

    // void updateZombieMovement(){
    //     dest = new Vector3(cameraTransform.position.x, 0f, cameraTransform.position.z);
    //     transform.position = Vector3.MoveTowards(transform.position, dest, zombieSpeed*Time.deltaTime);
    //     transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(cameraTransform.position - transform.position), zombieSpeed*Time.deltaTime);
    // }

    private void Chasing(){
        m_PlayerNear = false;
        playerLastPosition = Vector3.zero;
        if(!m_CaughtPlayer){
            Move(speedRun);
            GetComponent<Animation>().Play("Z_Run_InPlace");
            navMeshAgent.SetDestination(m_PlayerPosition);
        }
        if(navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance){
            if(m_WaitTime <= 0 && !m_CaughtPlayer && Vector3.Distance(transform.position, cameraTransform.position) >= 6f){
                m_IsPatrol = true;
                m_PlayerNear = false;
                Move(speedWalk);
                GetComponent<Animation>().Play("Z_Walk_InPlace");
                m_TimeToRotate = timeToRotate;
                m_WaitTime = startWaitTime;
                navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
            }
            else{
                if(Vector3.Distance(transform.position, cameraTransform.position) >= 3f){
                    Stop();
                    m_WaitTime -= Time.deltaTime;
                }
            }
        }
    }

    public void Patroling(){
        if(m_PlayerNear){
            if(m_TimeToRotate <= 0){
                Move(speedWalk);
                GetComponent<Animation>().Play("Z_Walk_InPlace");
                LookingPlayer(playerLastPosition);
            }
            else{
                Stop();
                m_TimeToRotate -= Time.deltaTime;
            }
        }
        else{
            m_PlayerNear = false;
            playerLastPosition = Vector3.zero;
            navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
            if(navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance){
                if(m_WaitTime <= 0){
                    NextPoint();
                    Move(speedWalk);
                    GetComponent<Animation>().Play("Z_Walk_InPlace");
                    m_WaitTime = startWaitTime;
                }
                else{
                    Stop();
                    m_WaitTime -= Time.deltaTime;
                }
            }
        }
    }

    void Move(float speed){
            navMeshAgent.isStopped = false;
            navMeshAgent.speed = speed;
    }

    void Stop(){
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = 0;
    }

    public void NextPoint(){
        m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
        navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
    }

    void CaughtPlayer(){
        m_CaughtPlayer = true;
    }

    void LookingPlayer(Vector3 player){
        navMeshAgent.SetDestination(player);
        if(Vector3.Distance(transform.position, player) <= 0.3){
            if(m_WaitTime <= 0){
                m_PlayerNear = false;
                Move(speedWalk);
                GetComponent<Animation>().Play("Z_Walk_InPlace");
                navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
                m_WaitTime = startWaitTime;
                m_TimeToRotate = timeToRotate;
            }
            else{
                Stop();
                m_WaitTime -= Time.deltaTime;
            }
        }
    }

    void EnviromentView(){
        Collider[] PlayerInRange = Physics.OverlapSphere(transform.position, viewRadius, playerMask);
        for(int i = 0; i < PlayerInRange.Length; i++){
            Transform player = PlayerInRange[i].transform;
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            if(Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2){
                float dstToPlayer = Vector3.Distance(transform.position, player.position);
                if(!Physics.Raycast(transform.position, dirToPlayer, dstToPlayer, ObstacleMask)){
                    m_PlayerInRange = true;
                    m_IsPatrol = false;
                }
                else{
                    m_PlayerInRange = false;
                }
            }
            if(Vector3.Distance(transform.position, player.position) > viewRadius){
                m_PlayerInRange = false;
            }
            if(m_PlayerInRange){
                m_PlayerPosition = new Vector3(player.transform.position.x, 0f, player.transform.position.z + 2);
            }
        }
        
    }

    

    

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ammo")
        {
            if(!dead){
                GetComponent<Animation>().Play("Z_FallingBack");
                dead = true;
            }
        }
    }
}
