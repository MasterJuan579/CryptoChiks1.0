using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;

public class Curso1Controller : MonoBehaviour
{
    private int totalLecciones = 5; // Número de lecciones de este curso
    private int idCurso = 1;        // ID del curso que representa esta escena

    void Start()
    {
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
            if (progreso.id_leccion <= 0) {
                Debug.LogWarning("⚠️ El progreso recibido tiene una lección inválida.");
            }

            Debug.Log("Despues de recibir. id_leccion = "+ progreso.id_leccion);

            Debug.Log($"📥 Progreso recibido: id_usuario={progreso.id_usuario}, id_curso={progreso.id_curso}, id_leccion={progreso.id_leccion}");
            Debug.Log($"📊 Lección completada más alta: {leccionCompletada}");

            ActualizarUI(leccionCompletada);
        }
        else
        {
            Debug.LogError("❌ Error al cargar progreso: " + request.error);
        }
    }

    // 👉 Método separado para evitar múltiples lambdas anónimas
    private void IrALeccion(int leccion)
    {
        Debug.Log($"➡️ Cargando Lección {leccion}");
        SesionManager.instancia.idLeccion = leccion;
        SesionManager.instancia.idCurso = idCurso;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Leccion");
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

            int leccion = i; // Necesario para capturar el índice

            // Limpia listeners previos antes de asignar uno nuevo (opcional pero recomendable si haces múltiples cargas)
            boton.clicked -= () => IrALeccion(leccion); // ⚠️ Esto no funciona con lambdas directamente, así que preferimos asegurar que no se acumulen

            // Estilo visual y estado de los botones
            if (ultimaLeccionCompletada == 0 && i == 1)
            {
                Debug.Log($"🔵 Lección {i} desbloqueada como primera.");
                boton.style.backgroundColor = new StyleColor(Color.blue);
                boton.SetEnabled(true);
            }
            else if (i <= ultimaLeccionCompletada)
            {
                Debug.Log($"🟢 Lección {i} marcada como completada.");
                boton.style.backgroundColor = new StyleColor(Color.green);
                boton.SetEnabled(true);
            }
            else if (i == ultimaLeccionCompletada + 1)
            {
                Debug.Log($"🔵 Lección {i} desbloqueada como siguiente disponible.");
                boton.style.backgroundColor = new StyleColor(Color.blue);
                boton.SetEnabled(true);
            }
            else
            {
                Debug.Log($"⚫ Lección {i} bloqueada.");
                boton.style.backgroundColor = new StyleColor(Color.gray);
                boton.SetEnabled(false);
            }

            // Solo asignamos listener si está habilitado
            if (boton.enabledSelf)
            {
                boton.clicked += () => IrALeccion(leccion);
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
