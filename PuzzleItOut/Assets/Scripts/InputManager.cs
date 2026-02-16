using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public InputSystem_Actions Input;
    public InputSystem_Actions.GameplayActions Gameplay;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Input = new InputSystem_Actions();
        Gameplay = Input.Gameplay;
    }

    void OnEnable() => Gameplay.Enable();
    void OnDisable() => Gameplay.Disable();
}
