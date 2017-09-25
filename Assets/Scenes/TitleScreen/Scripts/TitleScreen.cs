using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    public Texture backgroundTexture;
    
    private void OnGUI()
    {
        // Display Background Texture
        //GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), backgroundTexture);
    }
}
