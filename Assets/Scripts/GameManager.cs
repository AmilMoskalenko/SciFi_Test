using System.Collections.Generic;
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

    private List<GameObject> _redDrones = new List<GameObject>();
    private List<GameObject> _blueDrones = new List<GameObject>();

    public int DronesPerTeam
    {
        get => _dronesPerTeam;
        set
        {
            if (_dronesPerTeam != value)
            {
                _dronesPerTeam = value;
                UpdateDrones();
            }
        }
    }

    public float DroneSpeed
    {
        get => _droneSpeed;
        set
        {
            if (_droneSpeed != value)
            {
                _droneSpeed = value;
                UpdateAllDronesSpeed();
            }
        }
    }

    public float ResourceSpawnInterval
    {
        get => _resourceSpawnInterval;
        set
        {
            if (_resourceSpawnInterval != value)
            {
                _resourceSpawnInterval = value;
                ResourceManager.Instance.UpdateSpawnInterval(_resourcePrefab, _resourceSpawnInterval);
            }
        }
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
        UpdateDrones();
    }

    public void SpawnDrones(Team team, Transform baseZone)
    {
        GameObject droneGO = Instantiate(_dronePrefab);
        droneGO.transform.position = baseZone.position + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
        droneGO.GetComponent<Renderer>().material = team == Team.Red ? _redMaterial : _blueMaterial;
        var controller = droneGO.GetComponent<DroneController>();
        controller.Initialize(team, baseZone);
        if (team == Team.Red)
            _redDrones.Add(droneGO);
        else
            _blueDrones.Add(droneGO);
    }

    public void UpdateDrones()
    {
        UpdateTeamDrones(Team.Red, _redBase, _redDrones);
        UpdateTeamDrones(Team.Blue, _blueBase, _blueDrones);
    }

    private void UpdateTeamDrones(Team team, Transform baseZone, List<GameObject> droneList)
    {
        while (droneList.Count < _dronesPerTeam)
        {
            SpawnDrones(team, baseZone);
        }
        while (droneList.Count > _dronesPerTeam)
        {
            var drone = droneList[droneList.Count - 1];
            droneList.RemoveAt(droneList.Count - 1);
            Destroy(drone);
        }
    }

    private void UpdateAllDronesSpeed()
    {
        foreach (var drone in _redDrones)
        {
            var controller = drone.GetComponent<DroneController>();
            if (controller != null)
                controller.UpdateSpeed();
        }
        foreach (var drone in _blueDrones)
        {
            var controller = drone.GetComponent<DroneController>();
            if (controller != null)
                controller.UpdateSpeed();
        }
    }

    public void SetShowPathForAllDrones(bool show)
    {
        foreach (var drone in _redDrones)
        {
            var controller = drone.GetComponent<DroneController>();
            if (controller != null)
                controller.SetShowPath(show);
        }
        foreach (var drone in _blueDrones)
        {
            var controller = drone.GetComponent<DroneController>();
            if (controller != null)
                controller.SetShowPath(show);
        }
    }
}
