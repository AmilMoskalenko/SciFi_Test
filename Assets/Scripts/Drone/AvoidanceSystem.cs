using UnityEngine;

public static class AvoidanceSystem
{
    public static void ApplySeparation(DroneController drone)
    {
        Collider[] neighbors = Physics.OverlapSphere(drone.transform.position, 1f);
        foreach (var n in neighbors)
        {
            if (n.TryGetComponent<DroneController>(out var other) && other != drone)
            {
                Vector3 dir = drone.transform.position - other.transform.position;
                drone.transform.position += dir.normalized * Time.deltaTime * 1f;
            }
        }
    }
}
