using UnityEngine;
using UnityEngine.InputSystem;

public class Piece : MonoBehaviour
{
    private Camera cam;
    private Vector3 offset;
    private bool dragging;
    private bool isPlaced;
    public PieceScriptable pieceData;
    private sideType[] sides;
    private int pieceLevel;
    public Sprite baseSprite;

    void Awake()
    {
        cam = Camera.main;
    }

    // this combo of onEnable and onDisable fix the issue where piece dragging would stop working after changing scenes
    void OnEnable()
    {
        cam = Camera.main;

        InputManager.Instance.Gameplay.Click.started += StartDrag;
        InputManager.Instance.Gameplay.Click.canceled += EndDrag;
        InputManager.Instance.Gameplay.RotateClockwise.performed += HandleRotateClockwise;
        InputManager.Instance.Gameplay.RotateCounterClockwise.performed += HandleRotateCounterClockwise;
    }

    void OnDisable()
    {
        if (InputManager.Instance == null) return;

        InputManager.Instance.Gameplay.Click.started -= StartDrag;
        InputManager.Instance.Gameplay.Click.canceled -= EndDrag;
        InputManager.Instance.Gameplay.RotateClockwise.performed -= HandleRotateClockwise;
        InputManager.Instance.Gameplay.RotateCounterClockwise.performed -= HandleRotateCounterClockwise;
    }

    void Start()
    {

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
            BoardManager.instance.RemovePiece(this);
        }
    }

    void EndDrag(InputAction.CallbackContext ctx)
    {
        if (!dragging) return;

        dragging = false;
        if (BoardManager.instance.TryPlacePiece(this))
        {
            isPlaced = true;
            DeckManager.instance.RemoveFromHand(this);
            return;
        }
        DeckManager.instance.ReturnToHand(this);
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

    public void CopyFrom(Piece other)
    {
        if (other == null) return;

        // Copy scriptable reference
        this.pieceData = other.pieceData;

        // Copy level if you use it
        this.pieceLevel = other.pieceLevel;

        // Rebuild sides from data 
        sides = new sideType[4];
        sides[0] = pieceData.north;
        sides[1] = pieceData.east;
        sides[2] = pieceData.south;
        sides[3] = pieceData.west;

        // Reset transform for UI
        transform.rotation = Quaternion.identity;

        // Disable dragging for UI copies
        dragging = false;
        isPlaced = false;
    }
}
