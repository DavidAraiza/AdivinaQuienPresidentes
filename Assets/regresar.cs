using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class regresar : MonoBehaviour
{
    

    public void RegresarButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
        
    }

    
}
