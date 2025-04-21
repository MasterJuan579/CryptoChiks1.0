using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class GestorLecciones : MonoBehaviour
{
    public VisualTreeAsset uxml;
    public StyleSheet uss;

    private int idUsuario = SesionManager.instancia.idUsuario;
    private Dictionary<int, Button> botonesLecciones = new Dictionary<int, Button>();
    private List<int> leccionesCompletadas = new List<int>();

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.Clear();
        root.styleSheets.Add(uss);
        root.Add(uxml.CloneTree());

        for (int i = 1; i <= 5; i++)
        {
            var boton = root.Q<Button>("leccion" + i);
            botonesLecciones[i] = boton;
            boton.SetEnabled(false);
            boton.style.backgroundColor = Color.gray;
        }

        StartCoroutine(ConsultarProgreso());
    }

    IEnumerator ConsultarProgreso()
    {
        string url = "https://x7ku27ou2wgijtxvfmii4so5mq0bdtic.lambda-url.us-east-1.on.aws/";  // ⚠️ Modifica con tu URL real
        WWWForm form = new WWWForm();
        form.AddField("id_usuario", idUsuario);

        UnityWebRequest request = UnityWebRequest.Post(url, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            ProgresoRespuesta data = JsonUtility.FromJson<ProgresoRespuesta>(json);

            foreach (int leccionId in data.lecciones)
            {
                leccionesCompletadas.Add(leccionId);
            }

            ActualizarBotones();
        }
        else
        {
            Debug.LogError("Error al obtener progreso: " + request.error);
        }
    }

    void ActualizarBotones()
    {
        for (int i = 1; i <= 5; i++)
        {
            Button boton = botonesLecciones[i];

            if (leccionesCompletadas.Contains(i))
            {
                boton.SetEnabled(true);
                boton.style.backgroundColor = new Color(0.2f, 0.8f, 0.2f); // Verde
            }
            else if (i == 1 || leccionesCompletadas.Contains(i - 1))
            {
                boton.SetEnabled(true);
                boton.style.backgroundColor = new Color(0.56f, 0, 0.29f); // Morado original
            }
            else
            {
                boton.SetEnabled(false);
                boton.style.backgroundColor = Color.gray;
            }
        }
    }

    [System.Serializable]
    public class ProgresoRespuesta
    {
        public List<int> lecciones;
    }
}
