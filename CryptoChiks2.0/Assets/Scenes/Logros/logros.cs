using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

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
        // Agrega más si necesitas colores distintos por logro
    };

    private Button botonPregunta;
    private VisualElement panelMensaje;
    private Button botonCerrarMensaje;
    private Button botonRegreso;

    private Dictionary<string, VisualElement> panelesFelicitacion = new();
    private Dictionary<string, Button> botonesCerrarFelicitacion = new();

    void Start()
    {
        if (SesionManager.instancia == null)
        {
            Debug.LogError("❌ SesionManager no está inicializado.");
            return;
        }

        root = GetComponent<UIDocument>().rootVisualElement;

        // Botón de regreso
        botonRegreso = root.Q<Button>("Botonregreos");
        if (botonRegreso != null)
        {
            botonRegreso.clicked += () =>
            {
                SceneManager.LoadScene("HomePage");
            };
        }

        // Panel de bienvenida
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

        // Inicializar paneles de felicitación para Logro1 - Logro12 y Extra1 - Extra3
        for (int i = 1; i <= 12; i++)
        {
            string panelId = $"PanelFelicitacion{i}";
            string cerrarId = $"CerrarFelicitacion{i}";
            VisualElement panel = root.Q<VisualElement>(panelId);
            Button cerrar = root.Q<Button>(cerrarId);

            if (panel != null && cerrar != null)
            {
                panel.style.display = DisplayStyle.None;
                panelesFelicitacion[panelId] = panel;
                botonesCerrarFelicitacion[panelId] = cerrar;

                cerrar.clicked += () => panel.style.display = DisplayStyle.None;
            }
        }

        for (int i = 1; i <= 3; i++)
        {
            string panelId = $"PanelFelicitacionextra{i}";
            string cerrarId = $"CerrarFelicitacionextra{i}";
            VisualElement panel = root.Q<VisualElement>(panelId);
            Button cerrar = root.Q<Button>(cerrarId);

            if (panel != null && cerrar != null)
            {
                panel.style.display = DisplayStyle.None;
                panelesFelicitacion[panelId] = panel;
                botonesCerrarFelicitacion[panelId] = cerrar;

                cerrar.clicked += () => panel.style.display = DisplayStyle.None;
            }
        }

        StartCoroutine(CargarProgresoYActualizarLogros());
    }

    private IEnumerator CargarProgresoYActualizarLogros()
    {
        string url = "https://oewpzv2scmv3ot75p4c7t66gem0tyywb.lambda-url.us-east-1.on.aws/";
        UnityWebRequest request = UnityWebRequest.Get(url + "?id_usuario=" + SesionManager.instancia.idUsuario + "&id_curso=" + idCurso);

        Debug.Log("🔄 Consultando progreso...");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ProgresoCurso progreso = JsonUtility.FromJson<ProgresoCurso>(request.downloadHandler.text);
            int leccionCompletada = progreso.id_leccion;

            Debug.Log($"📥 Lección completada: {leccionCompletada}");
            ActualizarLogros(leccionCompletada);
        }
        else
        {
            Debug.LogError("❌ Error al cargar progreso: " + request.error);
        }
    }

    private void ActualizarLogros(int leccionCompletada)
    {
        // Mostrar logros normales
        for (int i = 1; i <= 12; i++)
        {
            string nombreLogro = "Logro" + i;
            Button boton = root.Q<Button>(nombreLogro);
            if (boton == null) continue;

            if (leccionCompletada >= i)
            {
                boton.style.display = DisplayStyle.Flex;
                if (coloresOriginales.ContainsKey(nombreLogro))
                    boton.style.backgroundColor = new StyleColor(coloresOriginales[nombreLogro]);

                int logroIndex = i;
                boton.clicked += () =>
                {
                    string panelId = $"PanelFelicitacion{logroIndex}";
                    if (panelesFelicitacion.ContainsKey(panelId))
                        panelesFelicitacion[panelId].style.display = DisplayStyle.Flex;
                };
            }
            else
            {
                boton.style.display = DisplayStyle.None;
            }
        }

        // Mostrar logros extra
        Dictionary<string, int> extrasPorCurso = new()
        {
            { "Extra1", 4 },
            { "Extra2", 8 },
            { "Extra3", 12 }
        };

        foreach (var extra in extrasPorCurso)
        {
            Button botonExtra = root.Q<Button>(extra.Key);
            if (botonExtra == null) continue;

            if (leccionCompletada >= extra.Value)
            {
                botonExtra.style.display = DisplayStyle.Flex;

                string panelId = $"PanelFelicitacion{extra.Key.ToLower()}";
                botonExtra.clicked += () =>
                {
                    if (panelesFelicitacion.ContainsKey(panelId))
                        panelesFelicitacion[panelId].style.display = DisplayStyle.Flex;
                };
            }
            else
            {
                botonExtra.style.display = DisplayStyle.None;
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
