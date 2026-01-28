using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //Game State
    public bool isPaused;
    public enum gameState
    {
        fight,
        shop
    }
    
    public int turn;  //0 Player's turn ;1 Enemy's Turn ???

    //Player
    float health;
    float gold; //temp currency

    //Enemy
    //Ref to enemy Class



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
          PauseGame();

        }
    }

    private void PauseGame()
    {
        isPaused = !isPaused;
    }


}
