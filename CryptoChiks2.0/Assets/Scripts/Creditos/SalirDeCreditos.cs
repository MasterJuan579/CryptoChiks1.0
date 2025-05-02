using UnityEngine;
using UnityEngine.SceneManagement;

public class SalirDeCreditos : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("🔙 Tecla presionada. Regresando a la escena HomePage...");
            SceneManager.LoadScene("HomePage");
        }
    }
}
