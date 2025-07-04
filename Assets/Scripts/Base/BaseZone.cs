using UnityEngine;

public class BaseZone : MonoBehaviour
{
    [SerializeField] private Team _team;
    private int _collectedCount;

    public void OnResourceDelivered()
    {
        _collectedCount++;
        UIManager.Instance.UpdateScore(_team, _collectedCount);
    }
}
