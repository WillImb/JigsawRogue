using UnityEngine;

/*
 * Author(s): Anthony L, 
 * Date: 6.22.26
 * Notes:
 * - Made this class DontDestroyOnLoad so it is always available for reassignment in piece class
 */
public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public InputSystem_Actions Input;
    public InputSystem_Actions.GameplayActions Gameplay;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Input = new InputSystem_Actions();
        Gameplay = Input.Gameplay;
    }

    void OnEnable()
    {
        Gameplay.Enable();
    }

    void OnDisable()
    {
        Gameplay.Disable();
    }
}