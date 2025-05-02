using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System.ComponentModel.Design.Serialization;
using UnityEngine.SceneManagement;

public class Curso1Controller : MonoBehaviour
{
    private int totalLecciones = 4;
    private int idCurso = 1;

    // Menú de pausa
    private VisualElement panelPausa;
    private Button botonMenuPausa;
     private Button Logros;
    private Button botonSalirMenu;
    private Button botonTienda;
    private Button botonCerrarPausa;
    private Slider sliderVolumen;
    private Button botonSiguienteCurso;

    // ID del curso que representa esta escena

    void Start()
    {
        if (SesionManager.instancia == null)
        {
            Debug.LogError("❌ SesionManager no está inicializado.");
            return;
        }

        var root = GetComponent<UIDocument>().rootVisualElement;
        botonSiguienteCurso = root.Q<Button>("SiguienteCurso");
        botonSiguienteCurso.clicked += () => {SceneManager.LoadScene("Curso2");};

        Debug.Log("🟢 Curso1Controller START ejecutado correctamente para el usuario: " + SesionManager.instancia.idUsuario);

        StartCoroutine(CargarProgresoYActualizarBotones());
    }

    private IEnumerator CargarProgresoYActualizarBotones()
    {
        string url = "https://oewpzv2scmv3ot75p4c7t66gem0tyywb.lambda-url.us-east-1.on.aws/";
        UnityWebRequest request = UnityWebRequest.Get(url + "?id_usuario=" + SesionManager.instancia.idUsuario + "&id_curso=" + idCurso);

        Debug.Log("🔄 Consultando progreso actual...");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ProgresoCurso progreso = JsonUtility.FromJson<ProgresoCurso>(request.downloadHandler.text);
            int leccionCompletada = progreso.id_leccion;

            if (leccionCompletada <= 0)
            {
                Debug.LogWarning("⚠️ El progreso recibido tiene una lección inválida.");
            }

            Debug.Log($"📥 Progreso recibido: id_usuario={progreso.id_usuario}, id_curso={progreso.id_curso}, id_leccion={progreso.id_leccion}");
            ActualizarUI(leccionCompletada);
        }
        else
        {
            Debug.LogError("❌ Error al cargar progreso: " + request.error);
        }
    }

    private void IrAExplicacion(int leccion)
    {
        Debug.Log($"➡️ Cargando Lección {leccion}");
        SesionManager.instancia.idLeccion = leccion;
        SesionManager.instancia.idCurso = idCurso;
        SceneManager.LoadScene("ExplicacionLeccion" + leccion);
    }

    private void ActualizarUI(int ultimaLeccionCompletada)
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        for (int i = 1; i <= totalLecciones; i++)
        {
            Button boton = root.Q<Button>("Leccion" + i);
            if (boton == null)
            {
                Debug.LogWarning($"⚠️ Botón 'Leccion{i}' no encontrado.");
                continue;
            }

            int leccion = i;

            if (ultimaLeccionCompletada == 0 && i == 1)
            {
                boton.style.backgroundColor = new StyleColor(Color.blue);
                boton.SetEnabled(true);
            }
            else if (i <= ultimaLeccionCompletada)
            {
                boton.style.backgroundColor = new StyleColor(Color.green);
                boton.SetEnabled(true);
            }
            else if (i == ultimaLeccionCompletada + 1)
            {
                boton.style.backgroundColor = new StyleColor(Color.blue);
                boton.SetEnabled(true);
            }
            else
            {
                boton.style.backgroundColor = new StyleColor(Color.gray);
                boton.SetEnabled(false);
            }

            if (boton.enabledSelf)
            {
                boton.clicked += () => IrAExplicacion(leccion);
            }

            if (ultimaLeccionCompletada >= totalLecciones)
            {
                Debug.Log("🎉 Curso completado, actualizando estado...");
                botonSiguienteCurso.style.display = DisplayStyle.Flex;
            }

        }

        // 🎮 MENÚ DE PAUSA
        panelPausa = root.Q<VisualElement>("PanelPausa");
        botonMenuPausa = root.Q<Button>("BotonMenuPausa");
        botonSalirMenu = root.Q<Button>("BotonSalirMenu");
        botonTienda = root.Q<Button>("BotonTienda");
        botonCerrarPausa = root.Q<Button>("BotonCerrarPausa");
        sliderVolumen = root.Q<Slider>("SliderVolumen");
        Logros = root.Q<Button>("Logros");

        
        

        // Mostrar menú de pausa
        botonMenuPausa.clicked += () =>
        {
            panelPausa.style.display = DisplayStyle.Flex;
            Time.timeScale = 0f;
            HabilitarBotonesLeccion(false);
        };

        // Cerrar menú de pausa
        botonCerrarPausa.clicked += () =>
        {
            panelPausa.style.display = DisplayStyle.None;
            Time.timeScale = 1f;
            HabilitarBotonesLeccion(true);
        };

        // Volver al menú principal
        botonSalirMenu.clicked += () =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("HomePage");
        };

        // Ir a la tienda
        botonTienda.clicked += () =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Tienda");
        };

        Logros.clicked += () =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Logros");
        };

        // Slider de volumen
        float volumenGuardado = PlayerPrefs.GetFloat("volumenJuego", 1f);
        AudioListener.volume = volumenGuardado;
        sliderVolumen.value = volumenGuardado;

        sliderVolumen.RegisterValueChangedCallback(evt =>
        {
            AudioListener.volume = evt.newValue;
            PlayerPrefs.SetFloat("volumenJuego", evt.newValue);
            PlayerPrefs.Save();
            Debug.Log("🎚 Volumen actualizado a: " + evt.newValue);
        });
    }

    private void HabilitarBotonesLeccion(bool habilitar)
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        for (int i = 1; i <= totalLecciones; i++)
        {
            Button boton = root.Q<Button>("Leccion" + i);
            if (boton != null)
                boton.SetEnabled(habilitar);
        }
    }


    [System.Serializable]
    public class ProgresoCurso
    {
        public int id_usuario;
        public int id_curso;
        public int id_leccion;
    }
}
