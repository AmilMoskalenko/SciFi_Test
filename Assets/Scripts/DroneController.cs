using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DroneController : MonoBehaviour
{
    public Team Team => _team;
    private Team _team;
    public Transform BaseZone => _baseZone;
    private Transform _baseZone;
    public float Speed => GameManager.Instance.DroneSpeed;

    private DroneState _currentState = DroneState.Idle;
    private ResourceNode _currentTarget;
    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void Initialize(Team t, Transform baseZ)
    {
        _team = t;
        _baseZone = baseZ;
        StartCoroutine(DroneLoop());
    }

    private IEnumerator DroneLoop()
    {
        while (true)
        {
            if (_currentState == DroneState.Idle || _currentState == DroneState.Delivering)
            {
                _currentTarget = ResourceManager.Instance.FindClosestAvailable(transform.position);
                if (_currentTarget != null)
                {
                    _currentTarget.Reserve();
                    _agent.SetDestination(_currentTarget.transform.position);
                    _currentState = DroneState.MovingToResource;
                }
            }
            else if (_currentState == DroneState.MovingToResource)
            {
                if (Vector3.Distance(transform.position, _currentTarget.transform.position) < 1f)
                {
                    _currentState = DroneState.Collecting;
                    yield return new WaitForSeconds(2f);
                    ResourceManager.Instance.CollectResource(_currentTarget);
                    _agent.SetDestination(_baseZone.position);
                    _currentState = DroneState.Returning;
                }
            }
            else if (_currentState == DroneState.Returning)
            {
                if (Vector3.Distance(transform.position, _baseZone.position) < 1f)
                {
                    _currentState = DroneState.Delivering;
                    yield return new WaitForSeconds(0.5f);
                }
            }
            yield return null;
        }
    }
}


public enum DroneState
{
    Idle,
    Finding,
    MovingToResource,
    Collecting,
    Returning,
    Delivering
}

public enum Team
{
    Red,
    Blue
}
