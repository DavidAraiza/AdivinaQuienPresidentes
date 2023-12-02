using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class mouseScript : MonoBehaviour
{
    
    public GameObject spriteBlanco;
    public GameObject presidentName;
    public TextMeshProUGUI presidentNameText;
    public string presidentNameString;

    public void Start()
    {
        spriteBlanco.SetActive(false);
        presidentName.SetActive(false);
    }

    public void OnMouseEnter()
    {
        spriteBlanco.SetActive(true);
        presidentName.SetActive(true);
    }

    public void OnMouseExit()
    {
        spriteBlanco.SetActive(false);
        presidentName.SetActive(false);
    }
    

}


