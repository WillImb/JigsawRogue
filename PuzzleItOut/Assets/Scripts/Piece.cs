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

    [SerializeField]
    private PieceScriptable pieceData;
    private sideType[] sides;

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
        InputManager.Instance.Gameplay.RotateClockwise.performed += HandleRotateClockwise;
        InputManager.Instance.Gameplay.RotateCounterClockwise.performed += HandleRotateCounterClockwise;

        //Side Data
        sides = new sideType[4];
        sides[0] = pieceData.north;
        sides[1] = pieceData.east;
        sides[2] = pieceData.south;
        sides[3] = pieceData.west;
    }

    void OnDestroy()
    {
        if (InputManager.Instance == null) return;

        InputManager.Instance.Gameplay.Click.started -= StartDrag;
        InputManager.Instance.Gameplay.Click.canceled -= EndDrag;
        InputManager.Instance.Gameplay.RotateClockwise.performed -= HandleRotateClockwise;
        InputManager.Instance.Gameplay.RotateCounterClockwise.performed -= HandleRotateCounterClockwise;
    }

    void Update()
    {
        if (!dragging) return;

        Vector2 screenPos = InputManager.Instance.Gameplay.Point.ReadValue<Vector2>();
        Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;

        transform.position = worldPos + offset;
    }

    public sideType[] GetAllSides()
    {
        return sides;
    }

    public sideType GetSide(Direction direction)
    {
        return sides[(int)direction];
    }

    public void RotatePieceClockwise()
    {
        sideType temp = sides[0]; // N side

        sides[0] = sides[3]; // W becomes old N
        sides[3] = sides[2]; // S becomes old W
        sides[2] = sides[1]; // E becomes old S
        sides[1] = temp;     // N becomes old E
        transform.Rotate(Vector3.forward * -90f);
    }

    public void RotatePieceCounterClockwise()
    {
        sideType temp = sides[0]; // N side

        sides[0] = sides[1]; // E becomes old N
        sides[1] = sides[2]; // S becomes old E
        sides[2] = sides[3]; // W becomes old S
        sides[3] = temp;     // N becomes old W
        transform.Rotate(Vector3.forward * 90f);
    }

    void StartDrag(InputAction.CallbackContext ctx)
    {
        Vector2 screenPos = InputManager.Instance.Gameplay.Point.ReadValue<Vector2>();
        Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        if (!hit || hit.transform != transform) return;

        offset = transform.position - (Vector3)worldPos;
        dragging = true;
        if (isPlaced == true)
        {
            isPlaced = false;
            BoardManager.Instance.RemovePiece(this);
        }
    }

    void EndDrag(InputAction.CallbackContext ctx)
    {
        if (!dragging) return;

        dragging = false;
        if (BoardManager.Instance.TryPlacePiece(this))
        {
            isPlaced = true;
            return;
        }
        SnapToDefault();
    }
    void HandleRotateClockwise(InputAction.CallbackContext ctx)
    {
        if (!dragging) return;
        RotatePieceClockwise();
    }

    void HandleRotateCounterClockwise(InputAction.CallbackContext ctx)
    {
        if (!dragging) return;
        RotatePieceCounterClockwise();
    }

    public void LockToSlot(Transform slot)
    {
        transform.SetParent(slot);
        transform.localPosition = Vector3.zero;
    }

    //Temp Code Again for Hand
    public void SnapToDefault()
    {
        transform.SetParent(originalSlot);
        transform.localPosition = originalPosition;
    }
}
