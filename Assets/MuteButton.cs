using UnityEngine;
using UnityEngine.UI;

public class MuteButton : MonoBehaviour
{
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;
    public Image buttonImage;
    private bool isSoundOn = true;

    public GameObject audio_source;


    private AudioSource audioSource; // Referencia al componente AudioSource en la Main Camera
    

    private void Start()
    {
        buttonImage.sprite = soundOnSprite;
        audioSource = audio_source.GetComponent<AudioSource>();// Accede al componente AudioSource de audio_source
    }

    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;
        if (isSoundOn)
        {
            buttonImage.sprite = soundOnSprite;
            audioSource.mute = false; // Activa el sonido de fondo
        }
        else
        {
            buttonImage.sprite = soundOffSprite;
            audioSource.mute = true; // Desactiva el sonido de fondo
        }
    }
}

