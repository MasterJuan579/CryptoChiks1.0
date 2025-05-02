using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;

public class LogrosController : MonoBehaviour
{
    private int idCurso = 1;
    private VisualElement root;

    private Dictionary<string, Color> coloresOriginales = new Dictionary<string, Color>()
    {
        { "Logro1", new Color32(199, 165, 0, 255) },
        { "Logro2", new Color32(164, 156, 156, 255) },
        { "Logro3", new Color32(56, 56, 56, 255) },
        { "Logro4", new Color32(218, 215, 54, 255) }
    };

    // Bienvenida
    private Button botonPregunta;
    private VisualElement panelMensaje;
    private Button botonCerrarMensaje;

    // Felicitaciones
    private Dictionary<int, VisualElement> panelesFelicitacion = new();
    private Dictionary<int, Button> botonesCerrarFelicitacion = new();

    void Start()
    {
        if (SesionManager.instancia == null)
        {
            Debug.LogError("❌ SesionManager no está inicializado.");
            return;
        }

        root = GetComponent<UIDocument>().rootVisualElement;

        // Inicializar bienvenida
        botonPregunta = root.Q<Button>("pregunta");
        panelMensaje = root.Q<VisualElement>("PanelMensaje");
        botonCerrarMensaje = root.Q<Button>("CerrarMensaje");

        if (botonPregunta != null && panelMensaje != null && botonCerrarMensaje != null)
        {
            botonPregunta.clicked += () =>
            {
                panelMensaje.style.display = DisplayStyle.Flex;
            };

            botonCerrarMensaje.clicked += () =>
            {
                panelMensaje.style.display = DisplayStyle.None;
            };
        }

        // Inicializar paneles de felicitación
        for (int i = 1; i <= 4; i++)
        {
            string panelName = $"PanelFelicitacion{i}";
            string closeName = $"CerrarFelicitacion{i}";

            var panel = root.Q<VisualElement>(panelName);
            var cerrar = root.Q<Button>(closeName);

            if (panel != null && cerrar != null)
            {
                panel.style.display = DisplayStyle.None;
                panelesFelicitacion[i] = panel;
                botonesCerrarFelicitacion[i] = cerrar;

                int index = i;
                cerrar.clicked += () =>
                {
                    panelesFelicitacion[index].style.display = DisplayStyle.None;
                };
            }
        }

        // Cargar progreso desde backend
        StartCoroutine(CargarProgresoYActualizarLogros());
    }

    private IEnumerator CargarProgresoYActualizarLogros()
    {
        string url = "https://oewpzv2scmv3ot75p4c7t66gem0tyywb.lambda-url.us-east-1.on.aws/";
        UnityWebRequest request = UnityWebRequest.Get(url + "?id_usuario=" + SesionManager.instancia.idUsuario + "&id_curso=" + idCurso);

        Debug.Log("🔄 Consultando progreso de logros...");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ProgresoCurso progreso = JsonUtility.FromJson<ProgresoCurso>(request.downloadHandler.text);
            int leccionCompletada = progreso.id_leccion;

            Debug.Log($"📥 Progreso recibido: lección {leccionCompletada}");
            ActualizarLogros(leccionCompletada);
        }
        else
        {
            Debug.LogError("❌ Error al cargar progreso: " + request.error);
        }
    }

    private void ActualizarLogros(int leccionCompletada)
    {
        for (int i = 1; i <= 4; i++)
        {
            string nombreLogro = "Logro" + i;
            Button botonLogro = root.Q<Button>(nombreLogro);
            if (botonLogro == null)
            {
                Debug.LogWarning($"⚠️ Botón {nombreLogro} no encontrado.");
                continue;
            }

            if (leccionCompletada >= i)
            {
                botonLogro.style.display = DisplayStyle.Flex;
                botonLogro.style.backgroundColor = new StyleColor(coloresOriginales[nombreLogro]);

                int logroIndex = i;
                botonLogro.clicked += () =>
                {
                    if (panelesFelicitacion.ContainsKey(logroIndex))
                    {
                        panelesFelicitacion[logroIndex].style.display = DisplayStyle.Flex;
                    }
                };
            }
            else
            {
                botonLogro.style.display = DisplayStyle.None;
            }
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
