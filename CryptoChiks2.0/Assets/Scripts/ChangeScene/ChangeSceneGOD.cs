using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneGOD : MonoBehaviour
{
    public void ChangeScene(string nombre)
    {
        SceneManager.LoadScene(nombre); // Nombre exacto de la escena
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("HomePage"); // Cambia a la escena de inicio
        }
    }
}
