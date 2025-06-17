using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private Slider _droneCountSlider;
    [SerializeField] private Slider _droneSpeedSlider;
    [SerializeField] private Slider _resourceIntervalSlider;
    [SerializeField] private Toggle _showPathToggle;

    [SerializeField] private TextMeshProUGUI _redScoreText;
    [SerializeField] private TextMeshProUGUI _blueScoreText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _droneCountSlider.onValueChanged.AddListener(v => GameManager.Instance.DronesPerTeam = (int)v);
        _droneSpeedSlider.onValueChanged.AddListener(v => GameManager.Instance.DroneSpeed = v);
        _resourceIntervalSlider.onValueChanged.AddListener(v => GameManager.Instance.ResourceSpawnInterval = v);
        _showPathToggle.onValueChanged.AddListener(v => GameManager.Instance.SetShowPathForAllDrones(v));
    }

    public void UpdateScore(Team team, int value)
    {
        if (team == Team.Red)
            _redScoreText.text = $"Red: {value}";
        else
            _blueScoreText.text = $"Blue: {value}";
    }
}
