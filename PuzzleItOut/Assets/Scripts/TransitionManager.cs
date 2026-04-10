using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{

    public static TransitionManager instance;

    public Animator animator;

    public bool trigger;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }


        
    }
    private void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("GameToShopTransition") && stateInfo.normalizedTime >= 1)
        {
            //loadShopScene
            SceneManager.LoadScene(4);
        }
       

    }


    public void ActivateTransition(string parameter)
    {
        animator.SetTrigger(parameter);
    }


    

}
