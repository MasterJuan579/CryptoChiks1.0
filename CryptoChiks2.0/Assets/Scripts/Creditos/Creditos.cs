using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Creditos : MonoBehaviour
{
    public Button botonContinue;

    void Start()
    {
        if (botonContinue != null)
        {
            botonContinue.onClick.AddListener(IrACreditos);
        }
        else
        {
            Debug.LogError("No se asignó el botón 'Continue' en el inspector.");
        }
    }

    void IrACreditos()
    {
        Debug.Log("🟢 Botón 'Continue' presionado. Cargando escena Créditos...");
        SceneManager.LoadScene("Creditos");
    }
}
