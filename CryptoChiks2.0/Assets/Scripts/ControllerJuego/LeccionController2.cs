using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class LeccionController2 : MonoBehaviour
{
    private List<RespuestaPregunta> respuestasUsuario = new List<RespuestaPregunta>();
    private Label preguntaLabel;

    private Button[] botones = new Button[3];
    private Label explicacionLabel;
    private Button botonContinuar;
    private List<Pregunta> preguntas;
    private int preguntaActual = 0;
    private bool seRespondio = false;

    private int vidas = 3;
    private Label textoVidas;

    private int monedas;
    private Label textoMonedas;

    private VisualElement panelSinVidas;
    private Button botonReiniciar;
    private Button botonComprarVida;

    private VisualElement panelRecompensa;
    private Label textoRecompensa;
    private Button botonRecompensa;

    private VisualElement panelPausa;
    private Button botonMenuPausa;
    private Button botonSalirMenu;
    private Button botonTienda;
    private Button botonCerrarPausa;
    private Slider sliderVolumen;

    private int totalLeccionesCurso;


    void Start()
    {
        Debug.Log("Start ejecutado");
        var root = GetComponent<UIDocument>().rootVisualElement;

        preguntaLabel = root.Q<Label>("PreguntaTexto");
        botones[0] = root.Q<Button>("Opcion1");
        botones[1] = root.Q<Button>("Opcion2");
        botones[2] = root.Q<Button>("Opcion3");
        explicacionLabel = root.Q<Label>("ResultadoTexto");
        botonContinuar = root.Q<Button>("Continuar");

        panelSinVidas = root.Q<VisualElement>("PanelSinVidas");
        botonReiniciar = root.Q<Button>("BotonReiniciar");
        botonComprarVida = root.Q<Button>("BotonComprarVidas");

        panelRecompensa = root.Q<VisualElement>("PanelRecompensa");
        textoRecompensa = root.Q<Label>("Recompensa");
        botonRecompensa = root.Q<Button>("BotonRecompensa");

        textoVidas = root.Q<Label>("TextoVidas");
        textoMonedas = root.Q<Label>("Monedas");

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
            Time.timeScale = 0f;
            HabilitarBotonesPregunta(false); 
        };

        // Cerrar menú de pausa
        botonCerrarPausa.clicked += () =>
        {
            panelPausa.style.display = DisplayStyle.None;
            Time.timeScale = 1f;
            HabilitarBotonesPregunta(true); 
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


        // Acción de repetir lección
        botonReiniciar.clicked += () =>
        {
            panelSinVidas.style.display = DisplayStyle.None;
            StartCoroutine(ReiniciarVidasYRepetirLeccion());
        };

        // Acción de comprar vida
        botonComprarVida.clicked += () =>
        {
            if (monedas >= 100)
            {
                monedas -= 100;
                vidas = 1;
                ActualizarCorazones();
                ActualizarMonedas();

                StartCoroutine(GuardarMonedas());
                StartCoroutine(GuardarVidas());

                panelSinVidas.style.display = DisplayStyle.None;
            }
            else
            {
                Debug.Log("❌ No tienes suficientes monedas.");
            }
        };

        botonRecompensa.clicked += () =>
        {
            StartCoroutine(FinalizarLeccionYVolver());
        };


        botonContinuar.clicked += ContinuarPregunta;
        botonContinuar.visible = false;

        Debug.Log("CargarVidas iniciando");
        StartCoroutine(CargarVidas());
        Debug.Log("CargarMonedas iniciando");
        StartCoroutine(CargarMonedas());
        Debug.Log("CargarPreguntas iniciando");
        StartCoroutine(CargarPreguntas());
        Debug.Log("Todo bien se supone");
    }

    [System.Serializable]
    public class TotalLeccionesResponse
    {
        public int total;
    }

    int ObtenerUltimaLeccionDelCurso(int idCurso)
    {
        return idCurso * 4; // Asumiendo 4 lecciones por curso
    }

    private IEnumerator ActualizarCursoCompletado()
    {
        string url = "https://c5sixrvlkejnsmeukhsa5tghqq0dwchi.lambda-url.us-east-1.on.aws/";

        UsuarioCurso curso = new UsuarioCurso
        {
            id_usuario = SesionManager.instancia.idUsuario,
            id_curso = SesionManager.instancia.idCurso,
            completado = true
        };

        string json = JsonUtility.ToJson(curso);
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Curso marcado como completado correctamente.");
        }
        else
        {
            Debug.LogError("❌ Error al actualizar curso completado: " + request.error);
        }
    }

    private IEnumerator FinalizarLeccionYVolver()
    {
        yield return StartCoroutine(ActualizarProgreso());

        yield return new WaitForSeconds(0.2f);

        int idCurso = SesionManager.instancia.idCurso;

        if (SesionManager.instancia.idLeccion == ObtenerUltimaLeccionDelCurso(idCurso))
        {
            Debug.Log("🎉 Lección final del curso alcanzada. Actualizando...");

            yield return StartCoroutine(ActualizarCursoCompletado());

            if (SesionManager.instancia.idCurso < 3) // suponiendo que hay 3 cursos
            {
                SesionManager.instancia.idCurso++;
                
                SesionManager.instancia.idLeccion = (SesionManager.instancia.idCurso - 1) * 4 + 1;
                Debug.Log("✅ Progreso inicializado para el nuevo curso.");
            }

            // Inicializar progreso del nuevo curso
            Progreso nuevoProgreso = new Progreso
            {
                id_usuario = SesionManager.instancia.idUsuario,
                id_curso = SesionManager.instancia.idCurso,
                id_leccion = ObtenerUltimaLeccionDelCurso(SesionManager.instancia.idCurso - 1) // desbloquea la primera lección del nuevo curso
            };

            string urlProgreso = "https://hxylz66dvpeg52x2sqxubqziwm0knymz.lambda-url.us-east-1.on.aws/";
            string jsonNuevo = JsonUtility.ToJson(nuevoProgreso);
            UnityWebRequest requestNuevo = new UnityWebRequest(urlProgreso, "POST");
            byte[] body = System.Text.Encoding.UTF8.GetBytes(jsonNuevo);
            requestNuevo.uploadHandler = new UploadHandlerRaw(body);
            requestNuevo.downloadHandler = new DownloadHandlerBuffer();
            requestNuevo.SetRequestHeader("Content-Type", "application/json");

            yield return requestNuevo.SendWebRequest();

            if (requestNuevo.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ Progreso inicializado para el nuevo curso.");
            }
            else
            {
                Debug.LogError("❌ Error al inicializar progreso del nuevo curso: " + requestNuevo.error);
            }

        }

        SceneManager.LoadScene("Curso" + SesionManager.instancia.idCurso, LoadSceneMode.Single);
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
            botonComprarVida.SetEnabled(false);
        }
        else
        {
            textoMonedas.style.color = new StyleColor(Color.white);
            botonComprarVida.SetEnabled(true);
        }
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
    void HabilitarBotonesPregunta(bool habilitar)
    {
    foreach (var boton in botones)
    {
        boton.SetEnabled(habilitar);
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

    private IEnumerator ReiniciarVidasYRepetirLeccion()
    {
        // Reiniciar las vidas internamente
        vidas = 3;

        // Guardar las nuevas vidas en el backend y actualizar el UI
        yield return StartCoroutine(GuardarVidas());
        ActualizarCorazones();  // Esto actualiza el texto del UI

        // Reinicia el estado del juego
        preguntaActual = 0;
        respuestasUsuario.Clear();

        // Recargar preguntas y reiniciar el flujo
        yield return StartCoroutine(CargarPreguntas());
    }




    private IEnumerator CargarPreguntas()
    {
        string url = "https://z52ydajbqtvzbldkd5rfpg7t3e0oxitr.lambda-url.us-east-1.on.aws/";
        string json = JsonUtility.ToJson(new LeccionRequest { id_leccion = SesionManager.instancia.idLeccion });
        Debug.Log("Json que se va a enviar: " + json);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text;
            Debug.Log("📥 JSON recibido del backend:\n" + response);

            PreguntaLista lista = JsonUtility.FromJson<PreguntaLista>(response);

            if (lista == null || lista.preguntas == null || lista.preguntas.Length == 0)
            {
                Debug.LogError("❌ No se encontraron preguntas para la lección.");
                preguntaLabel.text = "No hay preguntas disponibles.";
                yield break;
            }

            preguntas = new List<Pregunta>(lista.preguntas);
            MostrarPregunta();
        }
        else
        {
            preguntaLabel.text = "Error loading questions";
        }
    }

    void MostrarPregunta()
    {
        seRespondio = false;
        explicacionLabel.text = "";
        botonContinuar.visible = false;

        Debug.Log($"📌 MOSTRANDO pregunta #{preguntaActual}");

        if (preguntaActual >= preguntas.Count)
        {
            Debug.Log("🏁 Se completaron todas las preguntas.");
            preguntaLabel.text = "Lesson complete!";

            StartCoroutine(EnviarExamenRespuestas());
            StartCoroutine(ActualizarProgreso());

            MostrarRecompensa();

            return;
        }

        var pregunta = preguntas[preguntaActual];
        Debug.Log($"🟢 Pregunta ID: {pregunta.id_pregunta}, Texto: {pregunta.texto}, Explicación: {pregunta.explicacion}");

        preguntaLabel.text = pregunta.texto;
        foreach (var btn in botones)
        {
            btn.SetEnabled(true);
            btn.visible = true; // por si también quieres asegurarte que se vean
        }


        for (int i = 0; i < botones.Length; i++)
        {
            var boton = botones[i];
            int index = i; // Captura del índice

            boton.text = pregunta.opciones[i].texto;
            boton.Clear(); // Limpia listeners anteriores

            boton.clicked += () =>
            {
                if (!seRespondio)
                {
                    // Accede directamente a la pregunta actual usando preguntaActual
                    var preguntaActualReal = preguntas[preguntaActual];
                    var opcionSeleccionada = preguntaActualReal.opciones[index];

                    var respuesta = new RespuestaPregunta
                    {
                        id_pregunta = preguntaActualReal.id_pregunta,
                        opcion_elegida = opcionSeleccionada.texto,
                        es_correcta = opcionSeleccionada.es_correcta
                    };

                    respuestasUsuario.Add(respuesta);

                    Debug.Log($"📥 Agregada respuesta - ID Pregunta: {respuesta.id_pregunta}, Opción: {respuesta.opcion_elegida}, Correcta: {respuesta.es_correcta}");

                    MostrarExplicacion(preguntaActualReal.explicacion, opcionSeleccionada.es_correcta == 1);
                    seRespondio = true;
                    botonContinuar.visible = true;

                    if (opcionSeleccionada.es_correcta == 0)
                    {
                        vidas--;
                        StartCoroutine(GuardarVidas());
                        ActualizarCorazones();
                        if (vidas <= 0)
                        {
                            Debug.Log("❌ Sin vidas restantes, mostrando panel de opciones");
                            panelSinVidas.style.display = DisplayStyle.Flex;

                            // Opcional: desactivar botones de opciones para evitar más clics
                            foreach (var btn in botones)
                                btn.SetEnabled(false);
                        }
                    }
                }
            };
        }

    }

    void MostrarRecompensa()
    {
        foreach (var btn in botones)
        {
            btn.visible = false;
        }

        panelRecompensa.style.display = DisplayStyle.Flex;
        botonRecompensa.visible = true;

        int recompensa = CalcularRecompensa();

        textoRecompensa.text = $"x{recompensa}";
        monedas += recompensa;

        ActualizarMonedas();
        StartCoroutine(GuardarMonedas());
    }

    int CalcularPuntaje()
    {
        int total = respuestasUsuario.Count;
        int correctas = 0;

        foreach (var respuesta in respuestasUsuario)
        {
            if (respuesta.es_correcta == 1)
                correctas++;
        }

        if (total == 0) return 0; // Prevención división por cero

        float porcentaje = ((float)correctas / total) * 100f;
        return Mathf.RoundToInt(porcentaje);
    }

    int CalcularRecompensa()
    {
        int correctas = 0;

        foreach (var respuesta in respuestasUsuario)
        {
            if(respuesta.es_correcta == 1)
                correctas++;
        }
        int recompensa = correctas * 12;
        return recompensa;
    }


    void ContinuarPregunta()
    {
        preguntaActual++;
        MostrarPregunta();
    }

    private IEnumerator EnviarExamenRespuestas()
    {
        string url = "https://u44smfummcee7cvnyes7vmuqjy0jmtrl.lambda-url.us-east-1.on.aws/"; // Cambia esta URL por la tuya real
        int puntajeFinal = CalcularPuntaje();

        EnvioExamen examen = new EnvioExamen
        {
            id_usuario = SesionManager.instancia.idUsuario,
            id_leccion = SesionManager.instancia.idLeccion,
            puntaje = puntajeFinal,
            respuestas = respuestasUsuario
        };

        string json = JsonUtility.ToJson(examen);
        Debug.Log("📤 JSON del examen: " + json);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonRespuesta = request.downloadHandler.text;
            Debug.Log("✅ Respuesta recibida: " + jsonRespuesta);

            RespuestaExamen respuesta = JsonUtility.FromJson<RespuestaExamen>(jsonRespuesta);
            Debug.Log($"🎯 Tu puntaje fue: {respuesta.puntaje}");
        }
        else
        {
            Debug.LogError("❌ Error al enviar el examen: " + request.downloadHandler.text);
        }
    }

    private IEnumerator ActualizarProgreso()
    {
        string urlGet = "https://oewpzv2scmv3ot75p4c7t66gem0tyywb.lambda-url.us-east-1.on.aws/";
        UnityWebRequest requestGet = UnityWebRequest.Get(urlGet + "?id_usuario=" + SesionManager.instancia.idUsuario + "&id_curso=" + SesionManager.instancia.idCurso);

        Debug.Log("🔄 Consultando progreso actual...");
        yield return requestGet.SendWebRequest();

        if (requestGet.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("📥 Progreso actual recibido: " + requestGet.downloadHandler.text);
            Progreso progresoActual = JsonUtility.FromJson<Progreso>(requestGet.downloadHandler.text);

            int leccionActual = SesionManager.instancia.idLeccion;
            Debug.Log($"ℹ️ Lección actual: {leccionActual}, Última guardada: {progresoActual.id_leccion}");

            if (leccionActual > progresoActual.id_leccion)
            {
                Debug.Log("✅ Lección más avanzada, se actualizará el progreso.");

                string urlPost = "https://hxylz66dvpeg52x2sqxubqziwm0knymz.lambda-url.us-east-1.on.aws/";
                Progreso nuevoProgreso = new Progreso
                {
                    id_usuario = SesionManager.instancia.idUsuario,
                    id_curso = SesionManager.instancia.idCurso,
                    id_leccion = leccionActual
                };

                string json = JsonUtility.ToJson(nuevoProgreso);
                Debug.Log("📤 JSON de nuevo progreso: " + json);

                UnityWebRequest requestPost = new UnityWebRequest(urlPost, "POST");
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
                requestPost.uploadHandler = new UploadHandlerRaw(bodyRaw);
                requestPost.downloadHandler = new DownloadHandlerBuffer();
                requestPost.SetRequestHeader("Content-Type", "application/json");

                yield return requestPost.SendWebRequest();

                if (requestPost.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("✅ Progreso actualizado correctamente.");
                }
                else
                {
                    Debug.LogError("❌ Error al actualizar el progreso: " + requestPost.error);
                }
            }
            else
            {
                Debug.Log("🔒 La lección ya estaba registrada o no es mayor. No se actualiza.");
            }
        }
        else
        {
            Debug.LogError("❌ Error al consultar progreso actual: " + requestGet.error);
        }
    }

    

    void MostrarExplicacion(string explicacion, bool correcta)
    {
        explicacionLabel.text = correcta ? $"Correct!\n{explicacion}" : $"Incorrect.\n{explicacion}";
    }

    [System.Serializable]
    public class LeccionRequest
    {
        public int id_leccion;
    }

    [System.Serializable]
    public class PreguntaLista { public Pregunta[] preguntas; }

    [System.Serializable]
    public class Pregunta
    {
        public int id_pregunta;
        public string texto;
        public string explicacion;
        public Opcion[] opciones;
    }

    [System.Serializable]
    public class Opcion
    {
        public int id_opcion;
        public string texto;
        public int es_correcta;
    }

    [System.Serializable]
    public class Progreso
    {
        public int id_usuario;
        public int id_curso;
        public int id_leccion;
    }

    [System.Serializable]
    public class Vidas
    {
        public int id_usuario;
        public int vidas;
    }

    public class Monedas
    {
        public int id_usuario;
        public int monedas;
    }
    
    [System.Serializable]
    public class RespuestaPregunta
    {
        public int id_pregunta;
        public string opcion_elegida;
        public int es_correcta;
    }

    [System.Serializable]
    public class EnvioExamen
    {
        public int id_usuario;
        public int id_leccion;
        public int puntaje;
        public List<RespuestaPregunta> respuestas;
    }
    
    [System.Serializable]
    public class RespuestaExamen
    {
        public string mensaje;
        public int puntaje;
    }

    [System.Serializable]
    public class UsuarioCurso
    {
        public int id_usuario;
        public int id_curso;
        public bool completado;
    }
}