
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using TMPro;
using System.Linq;
using UnityEngine.EventSystems;

[System.Serializable]
public class PresidenteData
{
    public string nombre;
    public string image;
    public List<string> pistas;
    public List<string> sonido;
}

public class PresidentList
{
    public List<PresidenteData> presidentes;
}

public class Game : MonoBehaviour
{
    public GameObject tarjetaPrefab;
    public string respuesta;
    public string presi;
    public List<string> pistas;
    public List<int> n_pista = new List<int> { 0, 1, 2, 3 };
    public List<string> pistas_sound;
    public int errores = 0;
    public bool partida_ganada = false;

    public bool mute_pistas = false;
    public const int TARJETAS_TABLERO = 14;

    //public Text pistasLabel;
    //public Text rachaLabel;
    public Transform tablero;
    public AudioSource pista_fx;
    //public Text points_lbl;

    //public GameObject Puntos_label;
    public GameObject points_lbl;
    public GameObject pistasLabel;
    public GameObject rachaLabel;
    public GameObject WinScreen;

    //private List<Dictionary<string, object>> arch;
    //private Dictionary<string, object> doc;
    private Dictionary<string, PresidenteData> doc;


    private int racha = 0;

    
    void Start()
    { 

        List<PresidenteData> presidenteDataList = LoadFromJson();

        doc = new Dictionary<string, PresidenteData>();

        foreach (var presidente in presidenteDataList)
        {
            doc[presidente.nombre] = presidente;
        }

        //ImprimirLlaves(doc);

        //pistasLabel.transform.parent.GetComponent<ScrollRect>().movementType = ScrollRect.MovementType.Clamped;

        points_lbl.GetComponent<TextMeshProUGUI>().text = "Puntos: " + Global.points.ToString();
        RachaUpdate();
        NewGame();
    }
    // Función para imprimir las llaves por si se quiere revisar que esten correctas:
    /*
    void ImprimirLlaves(Dictionary<string, PresidenteData> diccionario)
    {
        foreach (string key in diccionario.Keys)
        {
            Debug.Log("Clave: " + key);
        }
    }
    */

    void NewGame()
    {
        Debug.Log("\nNuevo Juego");
        pistasLabel.GetComponent<TextMeshProUGUI>().text = "";
        errores = 0;
        pistas = new List<string>();
        partida_ganada = false;
        GameObject.Find("Errores_label").GetComponent<TextMeshProUGUI>().text = "Errores: " + errores + "/3";
        WinScreen.SetActive(false);

        foreach (Transform child in tablero)
        {
            Destroy(child.gameObject);
        }

        Random.InitState(System.Environment.TickCount);

        List<string> keys = new List<string>(doc.Keys);
        int randIndex = Random.Range(0, keys.Count);

        presi = keys[randIndex];
        keys.RemoveAt(randIndex);

        List<string> opciones = new List<string> { presi };
        pistas = pistas = doc[presi].pistas;
        //new List<string>((doc[presi] as Dictionary<string, PresidenteData>)["pistas"] as List<string>);
        n_pista = new List<int>(Enumerable.Range(0, pistas.Count));

        PresidenteData presiData = null; // Asignación predeterminada

        if (doc[presi] is PresidenteData data)
        {
            presiData = data;
            if (presiData.sonido != null)
            {
                pistas_sound = new List<string>(presiData.sonido);
            }

            respuesta = presiData.nombre;
            Debug.Log(respuesta);
        }


        for (int i = 0; i < TARJETAS_TABLERO - 1; i++)
        {
            int randInd = Random.Range(0, keys.Count);
            opciones.Add(keys[randInd]);
            keys.RemoveAt(randInd);
        }

        opciones = ShuffleList(opciones);

        
        foreach (string opcion in opciones)
        {
            GameObject T = Instantiate(tarjetaPrefab);
            T.GetComponent<Image>().enabled = true;
            T.GetComponent<mouseScript>().enabled = true;
            T.GetComponent<EventTrigger>().enabled = true;
            

            // Encuentra el hijo que contiene el componente Image
            Transform imageChild = T.transform.Find("spriteNegro");
            // Obtiene el componente Image
            Image imageComponent = imageChild.GetComponent<Image>();
            // Activa el componente Image
            imageComponent.enabled = true;


            //Activa el componente TextMeshPro
            TextMeshProUGUI textComponent = T.GetComponentInChildren<TextMeshProUGUI>(true);
            if (textComponent != null)
            {
                textComponent.enabled = true;
            }

            string pNombre = doc[opcion].nombre;
            string image_path = doc[opcion].image;
            string pathSinExtension = image_path.Replace(".png", "");
            //Debug.Log(pNombre);
            //Debug.Log(image_path);
            
            textComponent.text = pNombre;

            // Carga la imagen utilizando Resources.Load<Sprite>()
            Sprite sprite = Resources.Load<Sprite>(pathSinExtension);
            if (sprite != null)
            {
                T.GetComponent<Image>().sprite = sprite;
            }
            else
            {
                Debug.LogError("No se pudo cargar el sprite en la ruta: " + pathSinExtension);
            }
            
            
            T.transform.SetParent(tablero, false);

        }


        if (Global.dificultad == 0)
        {
            //ShowAllPistas();
            SetPista();
            SetPista();
            SetPista();
        }
        else if (Global.dificultad == 1)
        {
            SetPista();
            SetPista();
        }
        else if (Global.dificultad == 2)
        {
            n_pista = ShuffleList(n_pista);
            SetPista();
        }
    }
    
    public void CheckAnswer(GameObject presiName)//, string btn_name)
    {
        string nombre = presiName.GetComponent<TextMeshProUGUI>().text;

        if (partida_ganada)
        {
            return;
        }

        if (nombre == respuesta)
        {
            racha+=1;
            Debug.Log("Correcto");
            DarPuntos();

            points_lbl.GetComponent<TextMeshProUGUI>().text = "Puntos: " + Global.points.ToString();

            GameWon();
        }
        else
        {
            racha = 0;
            Debug.Log("Incorrecto");
            Global.points -= 5;
            points_lbl.GetComponent<TextMeshProUGUI>().text = "Puntos: " + Global.points.ToString();

            PlaySound("error");
            errores++;
            GameObject.Find("Errores_label").GetComponent<TextMeshProUGUI>().text = "Errores " + errores + "/3";
            if (errores == 3)
            {
                GameLost();
                return;
            }
            presiName.transform.parent.GetComponent<EventTrigger>().enabled = false;
            
            SetPista();
        }

        RachaUpdate();
    }

    void DarPuntos()
    {
        float mult = 1f;
        if (Global.dificultad == 0)
        {
            mult = 1f;
        }
        else if (Global.dificultad == 1 && racha >= 2)
        {
            mult = 1.2f;
        }
        else if (Global.dificultad == 2 && racha >= 3)
        {
            mult = 1.5f;
        }
        else
        {
            mult = 1f;
        }

        Global.points += (int)(10 * mult);
    }

    void RachaUpdate()
    {
        rachaLabel.GetComponent<TextMeshProUGUI>().text = "Racha: " + racha.ToString();
    }

    void GameLost()
    {
        NewGame();
    }

    void GameWon()
    {
        partida_ganada = true;
        WinScreen.SetActive(true);
        PlaySound("Win");
        ShowAllPistas();
    }

    void ShowAllPistas()
    {
        while (n_pista.Count > 0)
        {
            SetPista();
        }
    }

    void SetPista()
    {
        if (n_pista.Count == 0)
        {
            return;
        }

        if (!string.IsNullOrEmpty(pistasLabel.GetComponent<TextMeshProUGUI>().text))
        {
            pistasLabel.GetComponent<TextMeshProUGUI>().text += "\n\n";
        }

        pistasLabel.GetComponent<TextMeshProUGUI>().text += pistas[n_pista[0]];
        string audioSinExtension = pistas_sound[n_pista[0]].Replace(".wav", "");
        pista_fx.clip = Resources.Load<AudioClip>("audios/" + audioSinExtension);
        //Debug.Log(pistas_sound[n_pista[0]]);
        n_pista.RemoveAt(0);

        //pista_fx.Play();
        if (!mute_pistas && !partida_ganada)
        {
            pista_fx.Play();
        }
    }

    public void OnNuevoJuegoButtonDown()
    {
        NewGame();
    }

    void PlaySound(string sonido)
    {
        switch (sonido)
        {
            case "Win":
                AudioClip winClip = Resources.Load<AudioClip>("sonidos/Sonido Victoria");
                if (winClip != null)
                {
                    AudioSource.PlayClipAtPoint(winClip, Camera.main.transform.position);
                }
                break;
            case "error":
                AudioClip errorClip = Resources.Load<AudioClip>("sonidos/Sonido Error");
                if (errorClip != null)
                {
                    AudioSource.PlayClipAtPoint(errorClip, Camera.main.transform.position);
                }
                break;
        }
    }


    public List<PresidenteData> LoadFromJson()
    {
        string json = File.ReadAllText(Application.dataPath + "/archivo_info.json");
        List<PresidenteData> data = new List<PresidenteData>();

        try
        {
            PresidentList presidentList = JsonUtility.FromJson<PresidentList>(json);
            data = presidentList.presidentes;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error loading data from JSON: " + e.Message);
        }

        return data;
    }


    // Función de mezcla para listas
    List<T> ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

}

