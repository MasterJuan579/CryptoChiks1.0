using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;

public class TiendaVidasController : MonoBehaviour
{
    private Button botonComprar1;
    private Button botonComprar2;
    private Button botonComprar3;
    private Label textoVidas;
    private Label textoMonedas;
    private Button botonVolver;
    private VisualElement iconoVida;
    private VisualElement PanelCompra;

    private VisualElement panelPausa;
    private Button botonMenuPausa;
    private Button botonSalirMenu;
    private Button botonTienda;
    private Button botonCerrarPausa;
    private Slider sliderVolumen;

    private int monedas;
    private int vidas;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        botonComprar1 = root.Q<Button>("BotonComprar1Vida");
        botonComprar2 = root.Q<Button>("BotonComprar2Vidas");
        botonComprar3 = root.Q<Button>("BotonComprar3Vidas");
        PanelCompra = root.Q<VisualElement>("PanelCompra");
        textoVidas = root.Q<Label>("TextoVidas");
        textoMonedas = root.Q<Label>("TextoMonedas");
        botonVolver = root.Q<Button>("Continuar");
        iconoVida = root.Q<VisualElement>("IconoVida"); // usa el name de tu UXML

        
        if (iconoVida != null)
        {
            StartCoroutine(Palpitar(iconoVida));
        }
        
        // Panel de Pausa y sus componentes
        panelPausa = root.Q<VisualElement>("PanelPausa");
        botonMenuPausa = root.Q<Button>("BotonMenuPausa");
        botonSalirMenu = root.Q<Button>("BotonSalirMenu");
        botonTienda = root.Q<Button>("BotonTienda");
        botonCerrarPausa = root.Q<Button>("BotonCerrarPausa");
        sliderVolumen = root.Q<Slider>("SliderVolumen");

        // Restaurar volumen desde PlayerPrefs
        float volumenGuardado = PlayerPrefs.GetFloat("volumenJuego", 1f);
        AudioListener.volume = volumenGuardado;
        sliderVolumen.value = volumenGuardado;

        // Mostrar panel de pausa
        botonMenuPausa.clicked += () =>
        {
            panelPausa.style.display = DisplayStyle.Flex;
            PanelCompra.style.display = DisplayStyle.None;
            botonVolver.style.display = DisplayStyle.None;
            Time.timeScale = 0f;
        };

        // Cerrar menú de pausa
        botonCerrarPausa.clicked += () =>
        {
            panelPausa.style.display = DisplayStyle.None;
            PanelCompra.style.display = DisplayStyle.Flex;
            botonVolver.style.display = DisplayStyle.Flex;
            Time.timeScale = 1f;
        };

        // Volver al menú principal
        botonSalirMenu.clicked += () =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("HomePage"); // Asegúrate que esta escena existe
        };

        // Ir a la tienda
        botonTienda.clicked += () =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Tienda"); // Asegúrate que esta escena está en Build Settings
        };

        // Cambiar volumen con el slider
        sliderVolumen.RegisterValueChangedCallback(evt =>
        {
            AudioListener.volume = evt.newValue;
            PlayerPrefs.SetFloat("volumenJuego", evt.newValue); // Guardar volumen
            PlayerPrefs.Save();
            Debug.Log("🎚 Volumen actualizado a: " + evt.newValue);
        });


        botonComprar1.clicked += () => ComprarVidas(1, 100);
        botonComprar2.clicked += () => ComprarVidas(2, 180);
        botonComprar3.clicked += () => ComprarVidas(3, 250);
        botonVolver.clicked += () => SceneManager.LoadScene("HomePage");

        StartCoroutine(CargarMonedas());
        StartCoroutine(CargarVidas());
    }

    void ComprarVidas(int cantidad, int costo)
    {
        if (monedas >= costo)
        {
            monedas -= costo;
            vidas += cantidad;
            ActualizarCorazones();
            ActualizarMonedas();

            StartCoroutine(GuardarMonedas());
            StartCoroutine(GuardarVidas());
            Debug.Log($"✅ Compraste {cantidad} vida(s) por {costo} monedas.");
        }
        else
        {
            Debug.Log("❌ No tienes suficientes monedas.");
        }
    }

    private IEnumerator CargarMonedas()
    {
        string url = "https://kagxbqdi2jtdrme6la324jh4xq0jwtfj.lambda-url.us-east-1.on.aws/";
        UnityWebRequest request = UnityWebRequest.Get(url + SesionManager.instancia.idUsuario);

        yield return request.SendWebRequest();
        if(request.result == UnityWebRequest.Result.Success)
        {
            Monedas respuesta = JsonUtility.FromJson<Monedas>(request.downloadHandler.text);
            monedas = respuesta.monedas;
            ActualizarMonedas();
        }
        else
        {
            Debug.LogError("Error al Cargar Monedas: " + request.error);
        }
    }

        void ActualizarMonedas()
        {
            textoMonedas.text = $"x{monedas}";

            if (monedas < 100)
            {
                textoMonedas.style.color = new StyleColor(Color.red);
                botonComprar1.SetEnabled(false);
                botonComprar2.SetEnabled(false);
                botonComprar3.SetEnabled(false);
            }
            else if(monedas < 180){
                botonComprar1.SetEnabled(true);
                botonComprar2.SetEnabled(false);
                botonComprar3.SetEnabled(false);
            }
            else if(monedas < 250){
                botonComprar1.SetEnabled(true);
                botonComprar2.SetEnabled(true);
                botonComprar3.SetEnabled(false);
            }
            else
            {
                textoMonedas.style.color = new StyleColor(Color.white);
                botonComprar1.SetEnabled(true);
                botonComprar2.SetEnabled(true);
                botonComprar3.SetEnabled(true);
            }
        }

    private IEnumerator CargarVidas()
    {
        string url = "https://f7p7f4gsbplwy2yg4cjhkmizfy0pummo.lambda-url.us-east-1.on.aws/";
        UnityWebRequest request = UnityWebRequest.Get(url + SesionManager.instancia.idUsuario);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Vidas respuesta = JsonUtility.FromJson<Vidas>(request.downloadHandler.text);
            vidas = respuesta.vidas;
            ActualizarCorazones();
        }
        else
        {
            Debug.LogError("❌ Error al cargar vidas: " + request.error);
        }
    }

    void ActualizarCorazones()
    {
        textoVidas.text = $"x{vidas}";
    }

    private IEnumerator GuardarMonedas()
    {
        string url = "https://c7crqmpribvrys2kd4o3ye72ri0fjodl.lambda-url.us-east-1.on.aws/";
        Monedas nueva = new Monedas { id_usuario = SesionManager.instancia.idUsuario, monedas = monedas };
        string json = JsonUtility.ToJson(nueva);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ Error al guardar monedas: " + request.error);
        }
    }

    private IEnumerator GuardarVidas()
    {
        string url = "https://el5ocffsuae7gq4vbvnddty2oi0wlerm.lambda-url.us-east-1.on.aws/";
        Vidas nueva = new Vidas { id_usuario = SesionManager.instancia.idUsuario, vidas = vidas };
        string json = JsonUtility.ToJson(nueva);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ Error al guardar vidas: " + request.error);
        }
    }

    private IEnumerator Palpitar(VisualElement icono)
    {
        while (true)
        {
            icono.style.scale = new Scale(new Vector2(1.2f, 1.2f));
            yield return new WaitForSeconds(0.4f);
            icono.style.scale = new Scale(new Vector2(1f, 1f));
            yield return new WaitForSeconds(0.4f);
        }
    }

    [System.Serializable]
    public class Vidas
    {
        public int id_usuario;
        public int vidas;
    }

    [System.Serializable]
    public class Monedas
    {
        public int id_usuario;
        public int monedas;
    }

}
