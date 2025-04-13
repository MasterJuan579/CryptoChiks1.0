using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System.Collections;

public class InicioSesion : MonoBehaviour
{
    private TextField inputNombre;
    private Label estado;
    private Button boton;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        inputNombre = root.Q<TextField>("NombreInput");
        estado = root.Q<Label>("Estado");
        boton = root.Q<Button>("BotonComenzar");

        boton.clicked += () =>
        {
            string nombre = inputNombre.text.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                estado.text = "⚠️ Escribe tu nombre.";
            }
            else
            {
                StartCoroutine(ObtenerIdUsuario(nombre));
            }
        };
    }

    IEnumerator ObtenerIdUsuario(string nombre)
    {
        string url = $"http://localhost:3000/usuario?nombre={UnityWebRequest.EscapeURL(nombre)}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            UsuarioRespuesta respuesta = JsonUtility.FromJson<UsuarioRespuesta>(request.downloadHandler.text);
            SesionUsuario.id_usuario = respuesta.id_usuario;
            SceneManager.LoadScene("Pregunta1");
        }
        else
        {
            if (request.responseCode == 404)
            {
                estado.text = "❌ Usuario no encontrado.";
                Debug.LogWarning("Usuario no encontrado en la base de datos");
            }
            else
            {
                estado.text = "❌ Error de conexión.";
                Debug.LogError(request.downloadHandler.text);
            }
        }
    }

    [System.Serializable]
    public class UsuarioRespuesta
    {
        public int id_usuario;
    }
}
