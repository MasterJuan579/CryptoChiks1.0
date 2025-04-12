using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class login : MonoBehaviour
{
    [Header("Inputs")]
    public TMP_InputField inputUsuario;
    public TMP_InputField inputContraseña;


    [Header("Botones")]
    public Button botonEnviar;

    [System.Serializable]
    public struct DatosUsuario
    {
        public string email;
        public string password;
    }

    [System.Serializable]
    public struct RespuestaServidor
    {
        public string mensaje;
        public int idUsuario;
    }

    void Start()
    {
        // Asignación de eventos
        botonEnviar.onClick.AddListener(EnviarDatosJSONentrada);
    }

    private void EnviarDatosJSONentrada()
    {
        StartCoroutine(SubirDatosJSON_Entrada());
    }
    private IEnumerator SubirDatosJSON_Entrada()
    {
        DatosUsuario datos;
        datos.email = inputUsuario.text;
        datos.password = inputContraseña.text;

        string datosJSON = JsonUtility.ToJson(datos);

        Debug.Log($"Usuario: {datos.email}, Contraseña: {datos.password}");
        Debug.Log($"JSON enviado: {datosJSON}");

        UnityWebRequest request = new UnityWebRequest("https://gawrllxwezdn5cu4lfixiwyzaq0xtftd.lambda-url.us-east-1.on.aws/", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(datosJSON);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string respuesta = request.downloadHandler.text;
            Debug.Log("Respuesta JSON cruda: " + respuesta);

            RespuestaServidor respuestaServidor = JsonUtility.FromJson<RespuestaServidor>(respuesta);

            if (respuestaServidor.mensaje == "Login exitoso")
            {
                Sesion.idUsuario = respuestaServidor.idUsuario;
                Debug.Log("Login exitoso - ID usuario guardado: " + Sesion.idUsuario);
                StartCoroutine(Verificacion(respuesta));
            }
            else
            {
                Debug.Log("Login Fallido");
            }
        }
        else
        {
            Debug.LogError("Error de conexión: " + request.responseCode);
        }

        request.Dispose();
    }

    private IEnumerator Verificacion(string jsonRespuesta)
    {
        RespuestaServidor respuesta = JsonUtility.FromJson<RespuestaServidor>(jsonRespuesta);
        if (respuesta.mensaje == "Login exitoso")
        {
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("HomePage");
        }
    }
}
