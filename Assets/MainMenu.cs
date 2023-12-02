using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{ 
    
    public void EstudiarButtonClicked()
    {
        SceneManager.LoadScene("Estudio");
        
    }

    public void JugarFacilButtonClicked()
    {
        Global.dificultad = 0;
        SceneManager.LoadScene("Game");
        
    }

    public void JugarMedioButtonClicked()
    {
        Global.dificultad = 1;
        SceneManager.LoadScene("Game");
        
    }

    public void JugarDificilButtonClicked()
    {
        Global.dificultad = 2;
        SceneManager.LoadScene("Game");
        
    }
}

