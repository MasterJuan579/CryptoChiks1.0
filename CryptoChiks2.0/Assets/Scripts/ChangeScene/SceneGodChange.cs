using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SceneGodChange : MonoBehaviour
{
    /// <summary>
    /// Cambia a la escena cuyo nombre se pasa como parámetro.
    /// </summary>
    public void CambiarEscena(string nombreEscena)
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        Button boton = root.Q<Button>("Button");
        if (boton != null)
        {
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            Debug.LogWarning("El nombre de la escena está vacío o es nulo.");
        }
    }
}
