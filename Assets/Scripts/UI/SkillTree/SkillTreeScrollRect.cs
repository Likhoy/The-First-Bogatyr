using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillTreeScrollRect : ScrollRect
{
    [SerializeField] float _maxZoom = 2f;
    [SerializeField] float _zoomLerpSpeed = 10f;
    float _currentZoom = 1f;
    Vector2 _startPinchCenterPosition;
    Vector2 _startPinchScreenPosition;
    float _mouseWheelSensitivity = 1f;
    private float _minZoom;

    private RectTransform _viewport;
    private RectTransform _rectTransform;
    private Vector2 _initialContentPosition;

    protected override void Awake()
    {
        _viewport = viewport;
        _rectTransform = GetComponent<RectTransform>();

        if (_viewport == null)
        {
            Debug.LogError("Viewport is null.");
        }
        if (_rectTransform == null)
        {
            Debug.LogError("RectTransform is null.");
        }

        if (content != null)
        {
            _initialContentPosition = content.anchoredPosition; 
        }
        else
        {
            Debug.LogError("Content is null.");
        }
    }

    private void OnEnable()
    {
        ResetZoomAndPosition();
        CalculateMinZoom();
    }

    private void Update()
    {
        float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scrollWheelInput) > float.Epsilon)
        {
            _currentZoom *= 1 + scrollWheelInput * _mouseWheelSensitivity;
            _currentZoom = Mathf.Clamp(_currentZoom, _minZoom, _maxZoom);
            _startPinchScreenPosition = (Vector2)Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(content, _startPinchScreenPosition, null, out _startPinchCenterPosition);
            Vector2 pivotPosition = new Vector3(content.pivot.x * content.rect.size.x, content.pivot.y * content.rect.size.y);
            Vector2 posFromBottomLeft = pivotPosition + _startPinchCenterPosition;
            SetPivot(content, new Vector2(posFromBottomLeft.x / content.rect.width, posFromBottomLeft.y / content.rect.height));
        }

        if (Mathf.Abs(content.localScale.x - _currentZoom) > 0.001f)
            content.localScale = Vector3.Lerp(content.localScale, Vector3.one * _currentZoom, _zoomLerpSpeed * Time.deltaTime);
    }


    static void SetPivot(RectTransform rectTransform, Vector2 pivot)
    {
        if (rectTransform == null) return;

        Vector2 size = rectTransform.rect.size;
        Vector2 deltaPivot = rectTransform.pivot - pivot;
        Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y) * rectTransform.localScale.x;
        rectTransform.pivot = pivot;
        rectTransform.localPosition -= deltaPosition;
    }

    private void CalculateMinZoom()
    {
        if (_viewport == null || content == null)
        {
            _minZoom = 0.1f;
            return;
        }

        float widthRatio = _viewport.rect.width / content.rect.width;
        float heightRatio = _viewport.rect.height / content.rect.height;

        _minZoom = Mathf.Max(widthRatio, heightRatio);

        _minZoom = Mathf.Max(_minZoom, .1f);
    }
    public void RecalculateMinZoom()
    {
        CalculateMinZoom();
    }

    private void ResetZoomAndPosition()
    {
        _currentZoom = 1f;
        content.localScale = Vector3.one;
        content.anchoredPosition = _initialContentPosition;

        normalizedPosition = new Vector2(0.5f, 0.5f);
    }
}
