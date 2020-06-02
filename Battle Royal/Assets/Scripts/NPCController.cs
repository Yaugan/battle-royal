using UnityEngine;
using System.Linq;


public enum NPCState
{
    Wander,
    Chase,
    Attack
}

public class NPCController : MonoBehaviour
{

    [SerializeField] private LayerMask _layermask;

    [SerializeField] private float _attackRange = 3f;
    [SerializeField] private float _rayDistance = 5.0f;
    [SerializeField] private float _stoppingDistance = 1.5f;

    private Vector3 _destination;
    private Quaternion _desiredRotation;
    private Vector3 _direction;
    private NPCController _target;
    //private PlayerMovement _playerTarget;
    private NPCState _currentState;

    //NPC Attacking each other try on 
    //private TankAI npcAttack;


    private void Update()
    {
        switch (_currentState)
        {
            case NPCState.Wander:
                {
                    if (NeedsDestination())
                    {
                        GetDestination();
                    }

                    transform.rotation = _desiredRotation;

                    transform.Translate(Vector3.forward * Time.deltaTime * 5f);

                    var rayColor = IsPathBlocked() ? Color.red : Color.green;
                    Debug.DrawRay(transform.position, _direction * _rayDistance, rayColor);

                    while (IsPathBlocked())
                    {
                        Debug.Log("Path Blocked");
                        GetDestination();
                    }

                    var targetToAggro = CheckForAggro();
                    if (targetToAggro != null)
                    {
                        _target = targetToAggro.GetComponent<NPCController>();
                        _currentState = NPCState.Chase;
                    }

                    break;
                }
            case NPCState.Chase:
                {
                    if (_target == null)
                    {
                        _currentState = NPCState.Wander;
                        return;
                    }

                    transform.LookAt(_target.transform);
                    transform.Translate(Vector3.forward * Time.deltaTime * 5f);

                    if (Vector3.Distance(transform.position, _target.transform.position) < _attackRange)
                    {
                        _currentState = NPCState.Attack;
                    }

                    break;
                }

            case NPCState.Attack:
                {
                    if (_target != null)
                    {
                        //attack the other npc...
                        //npcAttack.GetComponent<TankAI>().StartFiring();
                       // npcAttack.GetComponent<TankAI>().StopFiring();
                        Debug.Log(_target.gameObject);
                    }

                    //play laser beam

                    _currentState = NPCState.Wander;
                    break;
                }
        }
    }

    private bool IsPathBlocked()
    {
        Ray ray = new Ray(transform.position, _direction);
        var hitSomething = Physics.RaycastAll(ray, _rayDistance, _layermask);
        return hitSomething.Any();
    }

    private void GetDestination()
    {
        Vector3 testPosition = (transform.position + (transform.forward * 4f)) + new Vector3(Random.Range(-4.5f, 4.5f), 0f, Random.Range(-4.5f, 4.5f));

        _destination = new Vector3(testPosition.x, 1f, testPosition.z);

        _direction = Vector3.Normalize(_destination - transform.position);
        _direction = new Vector3(_direction.x, 0f, _direction.z);
        _desiredRotation = Quaternion.LookRotation(_direction);
    }

    private bool NeedsDestination()
    {
        if (_destination == Vector3.zero)
        {
            return true;
        }

        var distance = Vector3.Distance(transform.position, _destination);
        if (distance <= _stoppingDistance)
        {
            return true;
        }

        return false;
    }

    Quaternion startingAngle = Quaternion.AngleAxis(-60, Vector3.up);
    Quaternion stepAngle = Quaternion.AngleAxis(5, Vector3.up);

    private Transform CheckForAggro()
    {
        float aggroRadius = 5f;

        RaycastHit hit;
        var angle = transform.rotation * startingAngle;
        var direction = angle * Vector3.forward;
        var pos = transform.position;
        for (var i = 0; i < 24; i++)
        {
            if (Physics.Raycast(pos, direction, out hit, aggroRadius))
            {
                var npc = hit.collider.GetComponent<NPCController>();
                //var player = hit.collider.GetComponent<PlayerMovement>();
                if (npc != null) // && drone.Team != gameObject.GetComponent<Drone>().Team)
                {
                    Debug.DrawRay(pos, direction * hit.distance, Color.red);
                    return (npc.transform);
                }/*
                else if (player != null)
                {
                    Debug.DrawRay(pos, direction * hit.distance, Color.red);
                    return (player.transform);
                }*/
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