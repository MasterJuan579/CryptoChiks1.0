using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void LoadForgotPasswordScene()
    {
        SceneManager.LoadScene("ForgotPasswordScene"); // Nombre exacto de la escena
    }
}
