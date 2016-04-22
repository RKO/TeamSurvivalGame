using UnityEngine;

public static class Extentions
{
    /// <summary>
    /// Returns a random world point inside the given BoxCollider
    /// </summary>
    public static Vector3 GetRandomPointInCollider(this BoxCollider area)
    {
        var bounds = area.bounds;
        var center = bounds.center;

        var x = Random.Range(center.x - bounds.extents.x, center.x + bounds.extents.x);
        var z = Random.Range(center.z - bounds.extents.z, center.z + bounds.extents.z);

        return new Vector3(x, area.transform.position.y, z);
    }
}
