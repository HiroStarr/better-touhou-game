using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneClickLoader : MonoBehaviour
{
    public string sceneToLoad = "Tutorial";

    void OnMouseDown()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
