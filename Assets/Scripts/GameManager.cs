using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Settings")]
    [SerializeField] private int _dronesPerTeam;
    [SerializeField] private float _droneSpeed;
    [SerializeField] private float _resourceSpawnInterval;
    [SerializeField] private Transform _redBase;
    [SerializeField] private Transform _blueBase;
    [SerializeField] private GameObject _dronePrefab;
    [SerializeField] private GameObject _resourcePrefab;

    public float DroneSpeed => _droneSpeed;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ResourceManager.Instance.StartSpawning(_resourcePrefab, _resourceSpawnInterval);
        SpawnDrones(Team.Red, _dronesPerTeam, _redBase);
        SpawnDrones(Team.Blue, _dronesPerTeam, _blueBase);
    }

    public void SpawnDrones(Team team, int count, Transform baseZone)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject droneGO = Instantiate(_dronePrefab);
            var controller = droneGO.GetComponent<DroneController>();
            controller.Initialize(team, baseZone);
        }
    }
}
