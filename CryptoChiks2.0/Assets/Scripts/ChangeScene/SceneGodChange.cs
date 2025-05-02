using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneGodChange : MonoBehaviour
{
    /// <summary>
    /// Cambia a la escena cuyo nombre se pasa como parámetro.
    /// </summary>
    public void CambiarEscena(string nombreEscena)
    {
        if (!string.IsNullOrEmpty(nombreEscena))
        {
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            Debug.LogWarning("El nombre de la escena está vacío o es nulo.");
        }
    }
}