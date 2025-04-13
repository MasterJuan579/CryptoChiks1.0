using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GestorCuestionario : MonoBehaviour
{
    public int idUsuario = 1;
    public int idCuestionario = 1;
    private int preguntaActual = 0;
    private List<Pregunta> preguntas;

    private Label preguntaTexto;
    private Button[] botones = new Button[3];
    private Label resultadoTexto;
    private VisualElement[] corazones = new VisualElement[3];

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        preguntaTexto = root.Q<Label>("PreguntaTexto");
        botones[0] = root.Q<Button>("Opcion1");
        botones[1] = root.Q<Button>("Opcion2");
        botones[2] = root.Q<Button>("Opcion3");
        resultadoTexto = root.Q<Label>("ResultadoTexto");

        corazones[0] = root.Q<VisualElement>("Corazon1");
        corazones[1] = root.Q<VisualElement>("Corazon2");
        corazones[2] = root.Q<VisualElement>("Corazon3");

        StartCoroutine(CargarPreguntas());
    }

    IEnumerator CargarPreguntas()
    {
        UnityWebRequest request = UnityWebRequest.Get($"http://localhost:3000/preguntas/{idCuestionario}");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = "{\"preguntas\":" + request.downloadHandler.text + "}";
            Debug.Log("✅ Preguntas recibidas: " + json);

            PreguntaLista lista = JsonUtility.FromJson<PreguntaLista>(json);
            preguntas = new List<Pregunta>(lista.preguntas);
            MostrarPregunta();
        }
        else
        {
            Debug.LogError("❌ Error al cargar preguntas: " + request.error);
        }
    }

    void MostrarPregunta()
    {
        if (preguntaActual >= preguntas.Count)
        {
            preguntaTexto.text = "¡Cuestionario terminado!";
            foreach (var btn in botones) btn.visible = false;
            return;
        }

        var pregunta = preguntas[preguntaActual];
        preguntaTexto.text = pregunta.texto;

        for (int i = 0; i < botones.Length; i++)
        {
            var opcion = pregunta.opciones[i];
            botones[i].text = opcion.texto;
            botones[i].clicked -= MostrarPregunta; // evitar duplicados
            botones[i].clicked += () => {
                StartCoroutine(EnviarRespuesta(opcion.id_opcion));
            };
        }
    }

    IEnumerator EnviarRespuesta(int idOpcion)
    {
        string url = "http://localhost:3000/verificar_respuesta";

        RespuestaUsuario data = new RespuestaUsuario
        {
            id_usuario = idUsuario,
            id_opcion = idOpcion
        };

        string json = JsonUtility.ToJson(data);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Respuesta del servidor: " + request.downloadHandler.text);
            var respuesta = JsonUtility.FromJson<Respuesta>(request.downloadHandler.text);
            resultadoTexto.text = respuesta.resultado == "correcto" ? "✅ Correcto" : "❌ Incorrecto";

            if (respuesta.resultado == "incorrecto")
                StartCoroutine(ActualizarVidas());

            preguntaActual++;
            Invoke(nameof(MostrarPregunta), 1.5f);
        }
        else
        {
            Debug.LogError("❌ Error al verificar respuesta: " + request.error);
            Debug.Log("Código de estado: " + request.responseCode);
            Debug.Log("Contenido del error: " + request.downloadHandler.text);
        }
    }

    IEnumerator ActualizarVidas()
    {
        UnityWebRequest request = UnityWebRequest.Get($"http://localhost:3000/vidas/{idUsuario}");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Vidas respuesta = JsonUtility.FromJson<Vidas>(request.downloadHandler.text);
            for (int i = 0; i < corazones.Length; i++)
            {
                corazones[i].style.display = i < respuesta.vidas ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }
        else
        {
            Debug.LogError("❌ Error al obtener vidas: " + request.error);
        }
    }

    // -------------------------
    // ESTRUCTURAS DE DATOS
    // -------------------------

    [System.Serializable]
    public class PreguntaLista { public Pregunta[] preguntas; }

    [System.Serializable]
    public class Pregunta
    {
        public int id_pregunta;
        public string texto;
        public Opcion[] opciones;
    }

    [System.Serializable]
    public class Opcion
    {
        public int id_opcion;
        public string texto;
    }

    [System.Serializable]
    public class RespuestaUsuario
    {
        public int id_usuario;
        public int id_opcion;
    }

    [System.Serializable]
    public class Respuesta
    {
        public string resultado;
    }

    [System.Serializable]
    public class Vidas
    {
        public int vidas;
    }
}
