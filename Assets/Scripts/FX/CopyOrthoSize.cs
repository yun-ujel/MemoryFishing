using UnityEngine;

[RequireComponent (typeof(Camera))]
public class CopyOrthoSize : MonoBehaviour
{
    [SerializeField] private Camera referenceCamera;

    private Camera selfCamera;

    private void Start()
    {
        selfCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        selfCamera.orthographicSize = referenceCamera.orthographicSize;
    }
}
