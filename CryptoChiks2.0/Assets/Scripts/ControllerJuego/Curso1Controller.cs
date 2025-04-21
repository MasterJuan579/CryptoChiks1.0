using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;

public class Curso1Controller : MonoBehaviour
{
    private int totalLecciones = 5; // Número de lecciones de este curso
    private int idCurso = 1; // ID del curso que representa esta escena

    void Start()
    {
        StartCoroutine(CargarProgresoYActualizarBotones());
    }

    private IEnumerator CargarProgresoYActualizarBotones()
    {
        string url = "https://tuprogresoapi-get.lambda-url.us-east-1.on.aws/";
        UnityWebRequest request = UnityWebRequest.Get(url + "?id_usuario=" + SesionManager.instancia.idUsuario + "&id_curso=" + idCurso);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ProgresoCurso progreso = JsonUtility.FromJson<ProgresoCurso>(request.downloadHandler.text);
            ActualizarUI(progreso.id_leccion); // última lección completada
        }
        else
        {
            Debug.LogError("❌ Error al cargar progreso: " + request.error);
        }
    }

    private void ActualizarUI(int ultimaLeccionCompletada)
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        for (int i = 1; i <= totalLecciones; i++)
        {
            Button boton = root.Q<Button>("Leccion" + i);

            if (boton == null) continue;

            if (i <= ultimaLeccionCompletada)
            {
                // Lecciones completadas
                boton.style.backgroundColor = new StyleColor(Color.green);
                boton.SetEnabled(true);

                int leccion = i; // Captura el índice
                boton.clicked += () =>
                {
                    SesionManager.instancia.idLeccion = leccion;
                    SesionManager.instancia.idCurso = idCurso;
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Leccion");
                };
            }
            else if (i == ultimaLeccionCompletada + 1)
            {
                // Lección siguiente (disponible)
                boton.style.backgroundColor = new StyleColor(Color.blue);
                boton.SetEnabled(true);

                int leccion = i;
                boton.clicked += () =>
                {
                    SesionManager.instancia.idLeccion = leccion;
                    SesionManager.instancia.idCurso = idCurso;
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Leccion");
                };
            }
            else
            {
                // Lecciones bloqueadas
                boton.style.backgroundColor = new StyleColor(Color.gray);
                boton.SetEnabled(false);
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
