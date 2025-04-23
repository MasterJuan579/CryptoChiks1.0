using UnityEngine;

public class SesionManager : MonoBehaviour
{
    public static SesionManager instancia;
    public int idUsuario = -1;
    public int id_sesion = -1;
    public int idCurso = -1;
    public int idLeccion = -1;

    void Awake()
    {
        Debug.Log("🟢 SesionManager instanciado");
        
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

