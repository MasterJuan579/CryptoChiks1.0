using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ContinuarProgreso : MonoBehaviour
{
    public Button botonContinue;

    void Start()
    {
        if (botonContinue != null)
        {
            botonContinue.onClick.AddListener(IrAContinuar);
        }
        else
        {
            Debug.LogError("No se asignó el botón 'New Game' en el inspector.");
        }
    }

    void IrAContinuar()
    {
        Debug.Log("🟥 Botón 'Continue' presionado.");
        StartCoroutine(ObtenerProgresoYRedirigir());    
    }

    private IEnumerator ObtenerProgresoYRedirigir()
    {
        string url = "https://tuprogresoapi-get.lambda-url.us-east-1.on.aws/";
        UnityWebRequest request = UnityWebRequest.Get(url + "?id_usuario=" + SesionManager.instancia.idUsuario);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Progreso respuesta = JsonUtility.FromJson<Progreso>(request.downloadHandler.text);
            Debug.Log($"🔄 Cargando curso {respuesta.id_curso}, lección {respuesta.id_leccion}");

            // Opcional: guarda la lección para el controlador del curso
            SesionManager.instancia.idCurso = respuesta.id_curso;
            SesionManager.instancia.idLeccion = respuesta.id_leccion;

            // Carga la escena correspondiente
            SceneManager.LoadScene("Curso" + respuesta.id_curso); // por ejemplo: Curso1, Curso2, etc.
        }
        else
        {
            Debug.LogError("❌ Error al obtener progreso: " + request.error);
        }
    }

    [System.Serializable]
    public class Progreso
    {
        public int id_usuario;
        public int id_curso;
        public int id_leccion;
    }
}
