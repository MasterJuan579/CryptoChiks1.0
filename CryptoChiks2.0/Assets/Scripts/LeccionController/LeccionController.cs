using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class LeccionController : MonoBehaviour
{
    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        root.Q<Button>("leccion1").clicked += () => CambiarEscena("HomePage");
        root.Q<Button>("leccion2").clicked += () => CambiarEscena("HomePage");
        root.Q<Button>("leccion3").clicked += () => CambiarEscena("HomePage");
        root.Q<Button>("leccion4").clicked += () => CambiarEscena("HomePage");
        root.Q<Button>("leccion5").clicked += () => CambiarEscena("HomePage");
    }

    private void CambiarEscena(string nombreEscena)
    {
        Debug.Log("Cambiando a: " + nombreEscena);
        SceneManager.LoadScene(nombreEscena);
    }
}
