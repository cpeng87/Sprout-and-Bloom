using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string plantSceneName;

    public void PlantSwapScene()
    {
        SceneManager.LoadScene(plantSceneName);
    }
}
