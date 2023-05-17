using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
   public void changeScene(int sceneID) {
    UnityEngine.SceneManagement.SceneManager.LoadScene(sceneID);
   }

   public void quitGame() {
    Application.Quit();
   }
}
