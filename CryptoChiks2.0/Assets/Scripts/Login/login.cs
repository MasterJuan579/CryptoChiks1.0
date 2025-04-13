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
        public int id_sesion;
    }

    void Start()
    {
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

        Debug.Log($"Enviando: {datosJSON}");

        UnityWebRequest request = new UnityWebRequest("https://toz3gahzj3xaytjuup7jkipqai0flfgy.lambda-url.us-east-1.on.aws/", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(datosJSON);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string respuesta = request.downloadHandler.text;
            Debug.Log("Respuesta JSON cruda: " + respuesta);

            bool loginExitoso = false;

            try
            {
                RespuestaServidor respuestaServidor = JsonUtility.FromJson<RespuestaServidor>(respuesta);

                if (respuestaServidor.mensaje == "Login exitoso")
                {
                    Sesion.idUsuario = respuestaServidor.idUsuario;
                    Sesion.id_sesion = respuestaServidor.id_sesion;

                    Debug.Log("Login exitoso. ID Usuario: " + Sesion.idUsuario + ", ID Sesión: " + Sesion.id_sesion);
                    loginExitoso = true;
                }
                else
                {
                    Debug.LogWarning("Login fallido - mensaje del servidor: " + respuestaServidor.mensaje);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error al parsear la respuesta del servidor: " + ex.Message);
                Debug.LogError("Contenido recibido: " + respuesta);
            }

            if (loginExitoso)
            {
                yield return new WaitForSeconds(1f);
                SceneManager.LoadScene("HomePage");
            }

        }
        else
        {
            Debug.LogError("Error de red: " + request.error);
        }

        request.Dispose();
    }
}
