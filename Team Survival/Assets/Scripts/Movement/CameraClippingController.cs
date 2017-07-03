using UnityEngine;

public class CameraClippingController : MonoBehaviour
{
    private static LayerMask TerrainLayerMask = 1 << 8;

    [SerializeField]
    private Transform CameraTrans;

    private Vector3 _originalCameraPosition;
    private float _originalDistance;

	private void Start () {
        _originalCameraPosition = CameraTrans.localPosition;
        _originalDistance = Vector3.Distance(transform.localPosition, _originalCameraPosition);
    }
	
	private void Update () {

        Vector3 dir = transform.TransformPoint(_originalCameraPosition) - transform.position;

        Ray ray = new Ray(transform.position, dir);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _originalDistance, TerrainLayerMask))
        {
            CameraTrans.localPosition = transform.InverseTransformPoint(hit.point);
        }
        else {
            CameraTrans.localPosition = _originalCameraPosition;
        }
	}
}
