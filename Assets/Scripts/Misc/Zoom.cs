using Cinemachine;
using UnityEngine;

public class Zoom : MonoBehaviour
{
    
    [SerializeField] private float zoomMultiplier = 4f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 15f;
    [SerializeField] private float smoothTime = 0.25f;

    private float zoom;
    private float velocity = 0f;

    /*[SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float xBorder = 50f;
    [SerializeField] private float yBorder = 50f;*/

    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    /*private Vector3 initialPosition;
    private Vector3 originPosition;
    private Vector3 difference;
    private bool dragging;*/

    void Start()
    {
        // initialPosition = transform.position;
        // zoom = Camera.main.orthographicSize;
        zoom = virtualCamera.m_Lens.OrthographicSize;
    }

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        zoom -= scroll * zoomMultiplier;

        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);

        virtualCamera.m_Lens.OrthographicSize = Mathf.SmoothDamp(virtualCamera.m_Lens.OrthographicSize, zoom, ref velocity, smoothTime);
    }

    /*void LateUpdate()
    {

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 tempVect = new Vector3(h, v, 0);
        tempVect = tempVect.normalized * moveSpeed * Time.deltaTime;

        Vector3 newPosition = transform.position + tempVect;
        if (Math.Abs(newPosition.x) < xBorder && Math.Abs(newPosition.y) < yBorder)
        {
            transform.position = newPosition;
        }
    }*/
}
