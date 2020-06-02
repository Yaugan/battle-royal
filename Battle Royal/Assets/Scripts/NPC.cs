using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Creating states for the npc
public enum State
{
    Wander,
    Detect,
    Chase,
    ChaseAndAttack,
    Attack,
    Escape
}


public class NPC : MonoBehaviour
{
    //creating a variable to access the state enum
    private State currentState;

    [Header("LayerMasking")]
    [SerializeField] private LayerMask layerMask;

    [Header("NeededStuff")]
    [SerializeField] private float stoppingDistance = 1f;
    [SerializeField] private float rayDistance = 10f;
    [SerializeField] private float attackRange = 3f;

    [Header("SelfMadeNeededStuff")]
    [SerializeField] private float chaseRange = 6f;
    [SerializeField] private float chaseAndAttackRange = 4f;
    [SerializeField] private float detectedRange = 9f;

    [Header("SelfMadeStuff")]
    [SerializeField] private float rangeValue = 4.5f;
    [SerializeField] private float forwardRange = 4f;
    [SerializeField] private float translateRate = 5f;
    [SerializeField] private int rayCount = 24;
    [SerializeField] private float aggroRadius = 10f;
    [SerializeField] private float escapeSpeed = 5f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float speed = 3f;

    private Vector3 destination;
    private Vector3 direction;
    private Quaternion desiredRotation;
    private NPC targetNPC;
    private Player targetPlayer;
    

    private void Update()
    {      


        switch (currentState)
        {
            case State.Wander:
                { 
                    //needs the destination to wander
                    if (NeedsDestination())
                    {
                        //gets the destination to wander to
                        GetsDestination();
                    }

                    transform.rotation = desiredRotation;

                    transform.Translate(Vector3.forward * Time.deltaTime * 5f);

                    var rayColor = IsPathBlocked() ? Color.red : Color.green;
                    Debug.DrawRay(transform.position, direction * rayDistance, rayColor);

                    while (IsPathBlocked())
                    {
                        Debug.Log("Path Blocked");
                        GetsDestination();
                    }

                    var targetToAggroNPC = CheckForAggro();
                    if (targetToAggroNPC != null)
                    {
                        targetNPC = targetToAggroNPC.GetComponent<NPC>();
                        currentState = State.Detect;
                        Debug.Log("Changing to Detect State for NPC");
                    }

                    var targetToAggroPlayer = CheckForAggro();
                    if (targetToAggroPlayer != null)
                    {
                        targetPlayer = targetToAggroPlayer.GetComponent<Player>();//issue
                        currentState = State.Detect;
                        Debug.Log("Changing to Detect State for Player");
                    }

                    break;
                }
            case State.Detect:
                {
                    if (targetPlayer == null && targetNPC == null)
                    {
                        currentState = State.Wander;
                    }

                    //if (targetPlayer.tag == "Player")
                    //{
                        if (Vector3.Distance(transform.position, targetPlayer.transform.position) <= detectedRange)
                        {
                            int selection = Random.Range(-10, 10);
                            if (selection < 0)
                            {
                                currentState = State.Chase;
                                Debug.Log("Player Detected, going to Chase State");
                            }
                            else
                            {
                                currentState = State.Wander;
                                Debug.Log("Player Detected, avoid and going to Wander State");
                            }
                        }
                    //}

                    //else if (targetNPC.tag == "NPC")
                    //{
                        if (Vector3.Distance(transform.position, targetNPC.transform.position) <= detectedRange)
                        {
                            int selection = Random.Range(-10, 10);
                            if (selection > 0)
                            {
                                currentState = State.Chase;
                                Debug.Log("NPC Detected, going to Chase State");
                            }
                            else
                            {
                                currentState = State.Wander;
                                Debug.Log("NPC Detected, avoid and going to Wander State");
                            }
                        }
                    //}

                    break;
                }
            case State.Chase:
                {
                    if (targetPlayer == null && targetNPC == null)
                    {
                        currentState = State.Wander;
                        return;
                    }

                    transform.LookAt(targetPlayer.transform);
                    transform.Translate(Vector3.forward * Time.deltaTime * translateRate);

                    if (targetPlayer.tag == "Player")
                    {
                        if (Vector3.Distance(transform.position, targetPlayer.transform.position) <= attackRange)
                        {
                            currentState = State.Attack;
                        }
                        else if (Vector3.Distance(transform.position, targetPlayer.transform.position) <= chaseAndAttackRange)
                        {
                            currentState = State.ChaseAndAttack;
                        }
                    }

                    else if (targetNPC.tag == "NPC")
                    {
                        if (Vector3.Distance(transform.position, targetNPC.transform.position) < attackRange)
                        {
                            currentState = State.Attack;
                        }
                        else if (Vector3.Distance(transform.position, targetNPC.transform.position) < chaseAndAttackRange)
                        {
                            currentState = State.ChaseAndAttack;
                        }
                    }
                    else if (Vector3.Distance(transform.position, targetNPC.transform.position) < attackRange)
                    {
                        currentState = State.Attack;
                    }

                    break;
                }
            case State.ChaseAndAttack:
                {

                    break;
                }
            case State.Attack:
                {

                    break;
                }
            case State.Escape:
                {

                    break;
                }
        }
    }

    private bool IsPathBlocked()
    {
        Ray ray = new Ray(transform.position, direction);
        var hitSomething = Physics.RaycastAll(ray, rayDistance, layerMask);
        return hitSomething.Any();
    }

    private void GetsDestination()
    {
        Vector3 testPosition = (transform.position + (transform.forward * forwardRange)) + new Vector3(Random.Range(-rangeValue, rangeValue), 0f, Random.Range(-rangeValue, rangeValue));

        destination = new Vector3(testPosition.x, 1f, testPosition.z);

        direction = Vector3.Normalize(destination - transform.position);
        direction = new Vector3(direction.x, 0f, direction.z);
        desiredRotation = Quaternion.LookRotation(direction);
    }

    private bool NeedsDestination()
    {
        if (destination == Vector3.zero)
        {
            return true;
        }

        var distance = Vector3.Distance(transform.position, destination);
        if (distance <= stoppingDistance)
        {
            return true;
        }

        return false;
    }


    [Header("Quaternions")]
    [SerializeField]
    Quaternion startingAngle = Quaternion.AngleAxis(-60, Vector3.up);
    [SerializeField]
    Quaternion stepAngle = Quaternion.AngleAxis(5, Vector3.up);

    private Transform CheckForAggro()
    {
        RaycastHit hit;
        var angle = transform.rotation * startingAngle;
        var direction = angle * Vector3.forward;
        var pos = transform.position;
        for (var i = 0; i < rayCount; i++)
        {
            if (Physics.Raycast(pos, direction, out hit, aggroRadius))
            {
                var npc = hit.collider.GetComponent<NPC>();
                var player = hit.collider.GetComponent<Player>(); //issues

                if (npc != null)
                {
                    Debug.DrawRay(pos, direction * hit.distance, Color.red);
                    return (npc.transform);
                }
                else if (player != null)
                {
                    Debug.DrawRay(pos, direction * hit.distance, Color.blue);
                    return (player.transform);
                }
                else
                {
                    Debug.DrawRay(pos, direction * hit.distance, Color.yellow);
                }
            }
            else
            {
                Debug.DrawRay(pos, direction * aggroRadius, Color.white);
            }
            direction = stepAngle * direction;
        }

        return null;
    }
}











//    /*
//    private void Start()
//    {
//        attack = GetComponent<Attack>();
//    }*/

//    private void Update()
//    {
//        switch (currentState)
//        {
//            case State.Wander:
//                {
//                    //needs the destination to wander
//                    if (NeedsDestination())
//                    {
//                        //gets the destination to wander to
//                        GetsDestination();
//                    }


//                    transform.rotation = desiredRotation;

//                    transform.Translate(Vector3.forward* Time.deltaTime* 5f);

//                    var rayColor = IsPathBlocked() ? Color.red : Color.green;
//Debug.DrawRay(transform.position, direction* rayDistance, rayColor);

//                    while (IsPathBlocked())
//                    {
//                        Debug.Log("Path Blocked");
//                        GetsDestination();
//                    }

//                    var targetToAggroNPC = CheckForAggro();
//                    if (targetToAggroNPC != null)
//                    {
//                        targetNPC = targetToAggroNPC.GetComponent<NPC>();
//                        currentState = State.ChaseNPC;
//                    }

//                    var targetToAggroPlayer = CheckForAggro();
//                    if (targetToAggroPlayer != null)
//                    {
//                        targetPlayer = targetToAggroPlayer.GetComponent<Player>();//issue
//                        currentState = State.ChasePlayer;
//                    }

//                    break;
//                }

//            case State.ChaseNPC:
//                {
//                    if (targetNPC == null)
//                    {
//                        currentState = State.Wander;
//                        return;
//                    }

//                    transform.LookAt(targetNPC.transform);
//                    transform.Translate(Vector3.forward * Time.deltaTime * translateRate);

//                    if (Vector3.Distance(transform.position, targetNPC.transform.position) < attackRange)
//                    {
//                        currentState = State.AttackNPC;
//                    }

//                    break;
//                }

//            case State.ChasePlayer:
//                {
                    //if (targetPlayer == null)
                    //{
                    //    currentState = State.Wander;
                    //    return;
                    //}

                    //transform.LookAt(targetPlayer.transform);
                    //transform.Translate(Vector3.forward* Time.deltaTime* translateRate);

                    //if (Vector3.Distance(transform.position, targetPlayer.transform.position) < attackRange)
                    //{
                    //    currentState = State.AttackPlayer;
                    //}

                    //break;
//                }

//            case State.AttackNPC:
//                {
//                    if (targetNPC != null)
//                    {
//                        Destroy(targetNPC.gameObject);
//                        //call attack script
//                        //attack.OnTriggerEnter(targetNPC.GetComponent<Collider>());
//                        Debug.Log(targetNPC.gameObject);
//                    }

//                    //play laser beam

//                    currentState = State.Wander;

//                    break;
//                }

//            case State.AttackPlayer:
//                {
//                    if (targetPlayer != null)
//                    {
//                        Destroy(targetPlayer.gameObject);
//                        //Call attack script
//                        //attack.OnTriggerEnter(targetPlayer.GetComponent<Collider>());
//                        Debug.Log(targetPlayer.gameObject);
//                    }

//                    //play laser beam

//                    currentState = State.Wander;

//                    break;
//                }
//        }
//    }

//private bool IsPathBlocked()
//{
//    Ray ray = new Ray(transform.position, direction);
//    var hitSomething = Physics.RaycastAll(ray, rayDistance, layerMask);
//    return hitSomething.Any();
//}

//private void GetsDestination()
//{
//    Vector3 testPosition = (transform.position + (transform.forward * forwardRange)) + new Vector3(Random.Range(-rangeValue, rangeValue), 0f, Random.Range(-rangeValue, rangeValue));

//    destination = new Vector3(testPosition.x, 1f, testPosition.z);

//    direction = Vector3.Normalize(destination - transform.position);
//    direction = new Vector3(direction.x, 0f, direction.z);
//    desiredRotation = Quaternion.LookRotation(direction);
//}

//private bool NeedsDestination()
//{
//    if (destination == Vector3.zero)
//    {
//        return true;
//    }

//    var distance = Vector3.Distance(transform.position, destination);
//    if (distance <= stoppingDistance)
//    {
//        return true;
//    }

//    return false;
//}

//[Header("Quaternions")]
//[SerializeField]
//Quaternion startingAngle = Quaternion.AngleAxis(-60, Vector3.up);
//[SerializeField]
//Quaternion stepAngle = Quaternion.AngleAxis(5, Vector3.up);

//private Transform CheckForAggro()
//{
//    RaycastHit hit;
//    var angle = transform.rotation * startingAngle;
//    var direction = angle * Vector3.forward;
//    var pos = transform.position;
//    for (var i = 0; i < rayCount; i++)
//    {
//        if (Physics.Raycast(pos, direction, out hit, aggroRadius))
//        {
//            var npc = hit.collider.GetComponent<NPC>();
//            var player = hit.collider.GetComponent<Player>(); //issues

//            if (npc != null)
//            {
//                Debug.DrawRay(pos, direction * hit.distance, Color.red);
//                return (npc.transform);
//            }
//            else if (player != null)
//            {
//                Debug.DrawRay(pos, direction * hit.distance, Color.blue);
//                return (player.transform);
//            }
//            else
//            {
//                Debug.DrawRay(pos, direction * hit.distance, Color.yellow);
//            }
//        }
//        else
//        {
//            Debug.DrawRay(pos, direction * aggroRadius, Color.white);
//        }
//        direction = stepAngle * direction;
//    }

//    return null;
//}
//}