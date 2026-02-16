using UnityEngine;
using UnityEngine.InputSystem;

public class Piece : MonoBehaviour
{
    private Camera cam;
    private Vector3 offset;
    private bool dragging;
    private bool isPlaced;

    //This is all temporary to get snapping back to hand working, will change once we
    //add a handmanager or something of that sort
    private Transform originalSlot;
    private Vector3 originalPosition;

    void Awake()
    {
        cam = Camera.main;
    }
    
    void Start()
    {

        originalSlot = transform.parent;
        originalPosition = transform.localPosition;
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
        if(isPlaced==true)
        {
            isPlaced = false;
            BoardManager.Instance.RemovePiece(this);
        }
    }

    void EndDrag(InputAction.CallbackContext ctx)
    {
        if (!dragging) return;

        dragging = false;
        if(BoardManager.Instance.TryPlacePiece(this)) 
        {
            isPlaced = true;
            return;
        }
        SnapToDefault();
    }

    public void LockToSlot(Transform slot)
    {
        transform.SetParent(slot);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    //Temp Code Again for Hand
    public void SnapToDefault()
    {
        transform.SetParent(originalSlot);
        transform.localPosition = originalPosition;
        transform.localRotation = Quaternion.identity;
    }
}
