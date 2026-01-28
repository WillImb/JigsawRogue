using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    

    public void SetScene(int index)
    {
       SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(index));
    }
    
}
