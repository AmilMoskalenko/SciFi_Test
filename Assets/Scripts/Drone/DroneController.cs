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
    private LineRenderer _lineRenderer;
    private bool _showPath;
    private float _retargetTimer;
    private const float RetargetInterval = 1f;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _lineRenderer = GetComponent<LineRenderer>();
        if (_lineRenderer == null)
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.positionCount = 0;
        _lineRenderer.widthMultiplier = 0.1f;
        _lineRenderer.enabled = false;
    }

    public void Initialize(Team t, Transform baseZ)
    {
        _team = t;
        _baseZone = baseZ;
        UpdateSpeed();
        StartCoroutine(DroneLoop());
    }

    private void Update()
    {
        if (_showPath && _agent != null && _agent.hasPath)
        {
            _lineRenderer.enabled = true;
            _lineRenderer.positionCount = _agent.path.corners.Length;
            _lineRenderer.SetPositions(_agent.path.corners);
        }
        else if (_lineRenderer != null)
        {
            _lineRenderer.enabled = false;
        }
    }

    private IEnumerator DroneLoop()
    {
        while (true)
        {
            if (_currentState == DroneState.Idle)
            {
                _currentTarget = ResourceManager.Instance.FindClosestAvailable(transform.position, _team);
                if (_currentTarget != null)
                {
                    _currentTarget.Reserve(_team);
                    _agent.SetDestination(_currentTarget.transform.position);
                    _currentState = DroneState.MovingToResource;
                }
            }
            else if (_currentState == DroneState.MovingToResource)
            {
                if (_currentTarget == null)
                {
                    _currentState = DroneState.Idle;
                }
                else if (_currentTarget == null || !ResourceManager.Instance.ContainsResource(_currentTarget))
                {
                    _currentState = DroneState.Idle;
                }
                else if (Vector3.Distance(transform.position, _currentTarget.transform.position) < 1f)
                {
                    ResourceManager.Instance.OccupyResource(_currentTarget);
                    _currentState = DroneState.Collecting;
                    yield return new WaitForSeconds(2f);
                    if (_currentTarget != null && _currentTarget.gameObject != null)
                    {
                        ResourceManager.Instance.CollectResource(_currentTarget);
                        _currentTarget.Unreserve();
                    }
                    _agent.SetDestination(_baseZone.position);
                    _currentState = DroneState.Returning;
                }
                else
                {
                    _retargetTimer += Time.deltaTime;
                    if (_retargetTimer >= RetargetInterval)
                    {
                        _retargetTimer = 0f;
                        var newTarget = ResourceManager.Instance.FindClosestAvailable(transform.position, _team);
                        if (newTarget != null && newTarget != _currentTarget)
                        {
                            float currentDist = Vector3.Distance(transform.position, _currentTarget.transform.position);
                            float newDist = Vector3.Distance(transform.position, newTarget.transform.position);
                            if (newDist + 0.1f < currentDist)
                            {
                                _currentTarget.Unreserve();
                                _currentTarget = newTarget;
                                _currentTarget.Reserve(_team);
                                _agent.SetDestination(_currentTarget.transform.position);
                            }
                        }
                    }
                }
            }
            else if (_currentState == DroneState.Returning)
            {
                if (Vector3.Distance(transform.position, _baseZone.position) < 2f)
                {
                    PlayUnloadEffect();
                    BaseZone bz = _baseZone.GetComponent<BaseZone>();
                    bz.OnResourceDelivered();
                    _currentState = DroneState.Delivering;
                    yield return new WaitForSeconds(0.5f);
                    _currentState = DroneState.Idle;
                }
            }
            AvoidanceSystem.ApplySeparation(this);
            yield return null;
        }
    }

    public void UpdateSpeed()
    {
        if (_agent != null)
            _agent.speed = GameManager.Instance.DroneSpeed;
    }

    public void SetShowPath(bool show)
    {
        _showPath = show;
        if (!show && _lineRenderer != null)
            _lineRenderer.enabled = false;
    }

    private void PlayUnloadEffect()
    {
        var particles = GameManager.Instance.GetBaseParticles(_team);
        if (particles != null)
            particles.Play();
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
