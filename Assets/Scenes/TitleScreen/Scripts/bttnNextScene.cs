using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class bttnNextScene : MonoBehaviour {

	public void changeScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
        //Application.LoadLevel(sceneNumber);
    }
}
