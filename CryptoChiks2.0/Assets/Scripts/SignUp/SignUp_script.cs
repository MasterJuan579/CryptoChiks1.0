using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class SignUpCanvas : MonoBehaviour
{
    [Header("Inputs")]
    public TMP_InputField inputFirstName;
    public TMP_InputField inputLastName;
    public TMP_InputField inputEmail;
    public TMP_InputField inputPassword;

    [Header("Fecha de nacimiento")]
    public TMP_Dropdown dropdownDia;
    public TMP_Dropdown dropdownMes;
    public TMP_Dropdown dropdownAnio;

    [Header("Botones")]
    public Button botonContinuar;

    [System.Serializable]
    public struct UsuarioNuevo
    {
        public string first_name;
        public string last_name;
        public string email;
        public string password_hash;
        public string fecha_nacimiento;
    }

    public struct RespuestaServidor
    {
        public string mensaje;
        public int idUsuario;
    }

    void Start()
    {
        // Llenar dropdowns con opciones
        dropdownDia.ClearOptions();
        dropdownDia.AddOptions(Enumerable.Range(1, 31).Select(d => d.ToString("D2")).ToList());

        List<string> meses = new List<string> {
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
        };
        dropdownMes.ClearOptions();
        dropdownMes.AddOptions(meses);

        dropdownAnio.ClearOptions();
        dropdownAnio.AddOptions(Enumerable.Range(1920, DateTime.Now.Year - 1920 + 1).Reverse().Select(a => a.ToString()).ToList());
        botonContinuar.onClick.AddListener(EnviarRegistro);
    }

    public void EnviarRegistro()
    {
        StartCoroutine(EnviarDatosRegistro());
    }

    private IEnumerator EnviarDatosRegistro()
    {
        string dia = dropdownDia.options[dropdownDia.value].text;
        int mesNumero = dropdownMes.value + 1;
        string mes = mesNumero.ToString("D2");
        string anio = dropdownAnio.options[dropdownAnio.value].text;

        string fechaNacimiento = $"{anio}-{mes}-{dia}";


        UsuarioNuevo usuario = new UsuarioNuevo
        {
            first_name = inputFirstName.text,
            last_name = inputLastName.text,
            email = inputEmail.text,
            password_hash = inputPassword.text,
            fecha_nacimiento = fechaNacimiento,
        };

        string datosJSON = JsonUtility.ToJson(usuario);
        Debug.Log("Datos enviados: " + datosJSON);

        UnityWebRequest request = new UnityWebRequest("https://eplu7fzz3pp5bh5toqxvg6tqxa0bqhtw.lambda-url.us-east-1.on.aws/", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(datosJSON);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string respuesta = request.downloadHandler.text;
            Debug.Log("Respuesta del servidor: " + respuesta);

            RespuestaServidor respuestaServidor = JsonUtility.FromJson<RespuestaServidor>(respuesta);

            if (respuestaServidor.mensaje == "Registro exitoso")
            {
                Debug.Log("Registro exitoso, cambiando a LoginScene");
                SceneManager.LoadScene("LoginScene");
            }
            else
            {
                Debug.LogWarning("El servidor respondió con: " + respuestaServidor.mensaje);
                // Aquí puedes mostrar un mensaje de error en pantalla si quieres
            }
        }
        else
        {
            Debug.LogError("Error al registrar: " + request.error);
        }


        request.Dispose();
    }
}
