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

        Progreso progreso = new Progreso
        {
            id_usuario = SesionManager.instancia.idUsuario,
            id_curso = 1,
            id_leccion = 1
        };

        string jsonData = JsonUtility.ToJson(progreso);
        Debug.Log("📤 JSON enviado: " + jsonData);

        UnityWebRequest request = new UnityWebRequest("https://hxylz66dvpeg52x2sqxubqziwm0knymz.lambda-url.us-east-1.on.aws/", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Progreso guardado correctamente: " + request.downloadHandler.text);

            SesionManager.instancia.idCurso = 1;
            SesionManager.instancia.idLeccion = 1;
            SceneManager.LoadScene("LeccionPreguntas");
        }
        else
        {
            Debug.LogError("❌ Error al guardar nuevo juego: " + request.responseCode);
            Debug.LogError("➡️ Contenido del error: " + request.downloadHandler.text);
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
