using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Settings")]
    [SerializeField] private int _dronesPerTeam;
    [SerializeField] private float _droneSpeed;
    [SerializeField] private float _resourceSpawnInterval;
    [Header("Field")]
    [SerializeField] private Transform _redBase;
    [SerializeField] private Transform _blueBase;
    [SerializeField] private GameObject _dronePrefab;
    [SerializeField] private GameObject _resourcePrefab;
    [SerializeField] private Material _redMaterial;
    [SerializeField] private Material _blueMaterial;

    public int DronesPerTeam
    {
        get => _dronesPerTeam;
        set => _dronesPerTeam = value;
    }
    public float DroneSpeed
    {
        get => _droneSpeed;
        set => _droneSpeed = value;
    }
    public float ResourceSpawnInterval
    {
        get => _resourceSpawnInterval;
        set => _resourceSpawnInterval = value;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        int initialResourceCount = _dronesPerTeam * 2;
        for (int i = 0; i < initialResourceCount; i++)
        {
            ResourceManager.Instance.SpawnResource(_resourcePrefab);
        }
        ResourceManager.Instance.StartSpawning(_resourcePrefab, _resourceSpawnInterval);
        for (int i = 0; i < _dronesPerTeam; i++)
        {
            SpawnDrones(Team.Red, _redBase);
            SpawnDrones(Team.Blue, _blueBase);
        }
    }

    public void SpawnDrones(Team team, Transform baseZone)
    {
        GameObject droneGO = Instantiate(_dronePrefab);
        droneGO.transform.position = baseZone.position + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
        droneGO.GetComponent<Renderer>().material = team == Team.Red ? _redMaterial : _blueMaterial;
        var controller = droneGO.GetComponent<DroneController>();
        controller.Initialize(team, baseZone);
    }
}
