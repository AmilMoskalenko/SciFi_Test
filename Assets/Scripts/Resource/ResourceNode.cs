using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public Team? ReservedByTeam { get; private set; }

    public bool IsReservedByTeam(Team team) => ReservedByTeam == team;
    public bool IsReserved => ReservedByTeam != null;

    public void Reserve(Team team)
    {
        ReservedByTeam = team;
    }

    public void Unreserve()
    {
        ReservedByTeam = null;
    }
}
