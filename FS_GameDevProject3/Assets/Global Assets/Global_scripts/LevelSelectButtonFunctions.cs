using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    /// <summary>
    /// GoToScene
    /// Enters a scene at id.
    /// </summary>
    /// <param name="id">The scene index</param>
    public void GoToScene(int id)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(id);
    }


    public void OpenOptionsMenu()
    {
        //
    }
}
