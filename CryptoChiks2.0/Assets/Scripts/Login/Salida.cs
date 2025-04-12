using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Salida: MonoBehaviour
{
    [Header("Botones")]
    public Button botonSalida;

    [System.Serializable]
    public struct RespuestaServidor
    {
        public string mensaje;
        public int idUsuario;
    }

    void Start()
    {
        if (botonSalida != null)
            botonSalida.onClick.AddListener(EnviarDatosJSON_Salida);
        else
            Debug.LogError("No se encontró el botón 'botonSalida' en el Inspector.");
    }

    private void EnviarDatosJSON_Salida()
    {
        if (Sesion.idUsuario == -1)
        {
            Debug.LogError("No hay usuario autenticado");
            return;
        }

        StartCoroutine(SubirDatosJSONSalida());
        SceneManager.LoadScene("LoginScene");
    }

    private IEnumerator SubirDatosJSONSalida()
    {
        RespuestaServidor respuestaServidor = new RespuestaServidor
        {
            idUsuario = Sesion.idUsuario,
            mensaje = "Usuario Encontrado"
        };

        string datosJSONsalida = JsonUtility.ToJson(respuestaServidor);
        Debug.Log("Datos de Salida: " + datosJSONsalida);

        UnityWebRequest request_salida = new UnityWebRequest("http://10.48.99.132:3000/unity/recibeJSON_salida", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(datosJSONsalida);

        request_salida.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request_salida.downloadHandler = new DownloadHandlerBuffer();
        request_salida.SetRequestHeader("Content-Type", "application/json");

        yield return request_salida.SendWebRequest();

        if (request_salida.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Salida enviada correctamente: " + request_salida.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error al enviar la salida: " + request_salida.error);
        }

        request_salida.Dispose();
    }
}
