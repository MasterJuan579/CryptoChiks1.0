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
        string url = "https://oewpzv2scmv3ot75p4c7t66gem0tyywb.lambda-url.us-east-1.on.aws/";
        UnityWebRequest request = UnityWebRequest.Get(url + "?id_usuario=" + SesionManager.instancia.idUsuario + "&id_curso=" + idCurso);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ProgresoCurso progreso = JsonUtility.FromJson<ProgresoCurso>(request.downloadHandler.text);
            int leccionCompletada = progreso.id_leccion ?? 0; // Si es null, lo tratamos como 0
            ActualizarUI(leccionCompletada);

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

            // Si no ha completado ninguna lección
            if (ultimaLeccionCompletada == 0 && i == 1)
            {
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
            else if (i <= ultimaLeccionCompletada)
            {
                boton.style.backgroundColor = new StyleColor(Color.green);
                boton.SetEnabled(true);

                int leccion = i;
                boton.clicked += () =>
                {
                    SesionManager.instancia.idLeccion = leccion;
                    SesionManager.instancia.idCurso = idCurso;
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Leccion");
                };
            }
            else if (i == ultimaLeccionCompletada + 1)
            {
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
        public int? id_leccion;
    }
}
