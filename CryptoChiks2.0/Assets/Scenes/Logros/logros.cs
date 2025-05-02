using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class LogrosController : MonoBehaviour
{
    private VisualElement root;

    private Dictionary<string, Color> coloresOriginales = new Dictionary<string, Color>()
    {
        { "Logro1", new Color32(199, 165, 0, 255) },
        { "Logro2", new Color32(164, 156, 156, 255) },
        { "Logro3", new Color32(56, 56, 56, 255) },
        { "Logro4", new Color32(218, 215, 54, 255) },
        { "Logro5", new Color32(200, 200, 80, 255) },
        { "Logro6", new Color32(180, 160, 90, 255) },
        { "Logro7", new Color32(170, 140, 120, 255) },
        { "Logro8", new Color32(160, 180, 120, 255) },
        { "Logro9", new Color32(150, 150, 150, 255) },
        { "Logro10", new Color32(140, 140, 100, 255) },
        { "Logro11", new Color32(130, 110, 160, 255) },
        { "Logro12", new Color32(120, 170, 140, 255) }
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

        // Paneles de felicitación normales
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

        // Paneles de felicitación extra
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
        yield return null;

        int leccionCompletada = SesionManager.instancia.idLeccion;
        Debug.Log($"📘 Progreso global desde SesionManager: lección {leccionCompletada}");

        ActualizarLogros(leccionCompletada);
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

        // Mostrar logros extra por curso completo
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
