using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class IniciarNuevoProgreso : MonoBehaviour
{
    [Header("Referencia al botón")]
    public Button botonNewGame;

    void Start()
    {
        if (botonNewGame != null)
        {
            botonNewGame.onClick.AddListener(IniciarNuevoJuego);
        }
        else
        {
            Debug.LogError("No se asignó el botón 'New Game' en el inspector.");
        }
    }

    public void IniciarNuevoJuego()
    {
        Debug.Log("🟥 Botón 'New Game' presionado.");
        StartCoroutine(GuardarNuevoProgreso());
    }

    private IEnumerator GuardarNuevoProgreso()
    {
        if (SesionManager.instancia.idUsuario == -1)
        {
            Debug.LogError("❌ El idUsuario no está definido. Verifica el login.");
            yield break;
        }

        Debug.Log("🆔 id_usuario actual: " + SesionManager.instancia.idUsuario);

        // 1. Elimina el progreso del usuario en el backend
        string url = "https://izybsp2h4gnvlbkol3atx4br5y0debju.lambda-url.us-east-1.on.aws/";
        string urlConParametros = url + "?id_usuario=" + SesionManager.instancia.idUsuario;

        UnityWebRequest request = UnityWebRequest.Delete(urlConParametros);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Progreso anterior eliminado.");

            // 2. Reinicia sesión localmente
            SesionManager.instancia.idCurso = 1;
            SesionManager.instancia.idLeccion = 1;

            // 3. Carga la escena inicial del curso
            SceneManager.LoadScene("Curso1");
        }
        else
        {
            Debug.LogError("❌ Error al eliminar el progreso anterior: " + request.error);
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
