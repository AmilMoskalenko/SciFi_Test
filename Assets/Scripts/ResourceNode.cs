using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public bool IsReserved => _isReserved;
    private bool _isReserved = false;

    public void Reserve()
    {
        _isReserved = true;
    }
}
