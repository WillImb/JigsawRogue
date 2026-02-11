using UnityEngine;
using UnityEngine.InputSystem;

public class Piece : MonoBehaviour
{
    private Camera cam;
    private Vector3 offset;
    private bool dragging;

    void Awake()
    {
        cam = Camera.main;
    }

    void Start()
    {
        InputManager.Instance.Gameplay.Click.started += StartDrag;
        InputManager.Instance.Gameplay.Click.canceled += EndDrag;
    }

    void OnDestroy()
    {
        if (InputManager.Instance == null) return;

        InputManager.Instance.Gameplay.Click.started -= StartDrag;
        InputManager.Instance.Gameplay.Click.canceled -= EndDrag;
    }

    void Update()
    {
        if (!dragging) return;

        Vector2 screenPos = InputManager.Instance.Gameplay.Point.ReadValue<Vector2>();
        Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;

        transform.position = worldPos + offset;
    }

    void StartDrag(InputAction.CallbackContext ctx)
    {
        Vector2 screenPos = InputManager.Instance.Gameplay.Point.ReadValue<Vector2>();
        Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        if (!hit || hit.transform != transform) return;

        offset = transform.position - (Vector3)worldPos;
        dragging = true;
    }

    void EndDrag(InputAction.CallbackContext ctx)
    {
        if (!dragging) return;

        dragging = false;
        BoardManager.Instance.TryPlacePiece(this);
    }

    public void LockToSlot(Transform slot)
    {
        transform.SetParent(slot);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}
