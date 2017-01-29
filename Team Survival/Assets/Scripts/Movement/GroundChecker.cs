using UnityEngine;

public class GroundChecker {
    private static LayerMask TerrainLayerMask = 1 << 8;
    private static Vector3 RayUp = Vector3.up * 0.25f;
    private static Vector3 RayDown = Vector3.down;
    private const float RayLength = 1.0f;

    private bool _cachedIsGrounded = false;
    private bool _hasIsGroundedCache = false;

    private Transform _transform;

    public GroundChecker(Transform transform)
    {
        _transform = transform;
    }

    public bool CalculateIsGrounded()
    {
        if (_hasIsGroundedCache)
            return _cachedIsGrounded;

        Vector3 pos = _transform.position + RayUp;
        Vector3 right = _transform.right * 0.25f;
        Vector3 forward = _transform.forward * 0.25f;

        bool grounded = CastRay(new Ray(pos - right + forward, RayDown));
        if (!grounded)
            grounded = CastRay(new Ray(pos + right + forward, RayDown));
        if (!grounded)
            grounded = CastRay(new Ray(pos - right - forward, RayDown));
        if (!grounded)
            grounded = CastRay(new Ray(pos + right - forward, RayDown));

        _cachedIsGrounded = grounded;
        _hasIsGroundedCache = true;

        return grounded;
    }

    public void ResetCache() {
        _cachedIsGrounded = false;
        _hasIsGroundedCache = false;
    }

    private bool CastRay(Ray ray)
    {
        return Physics.Raycast(ray, RayLength, TerrainLayerMask);
    }
}
