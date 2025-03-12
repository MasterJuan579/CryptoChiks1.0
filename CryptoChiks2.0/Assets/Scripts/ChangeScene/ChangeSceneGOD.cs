using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneGOD : MonoBehaviour
{
    public void ChangeScene(string nombre)
    {
        SceneManager.LoadScene(nombre); // Nombre exacto de la escena
    }

}
