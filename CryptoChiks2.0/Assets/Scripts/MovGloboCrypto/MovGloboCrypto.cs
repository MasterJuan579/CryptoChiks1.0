using UnityEngine;
using UnityEngine.UIElements;

public class MovGloboCrypto : MonoBehaviour
{

    public float speed = 2f;
    public Transform pointA;
    public Transform pointB;

    private Vector3 target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = pointB.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Mover hacia el objetivo
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Si llego al punto objetivo, cambiar a otro punto
        if (Vector2.Distance(transform.position, target) < 0.1f)
        {
            if (target == pointB.position)
            {
                target = pointA.position;

                 // Hacer flip en X para mirar a la derecha
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else {
                target = pointB.position;

                // Hacer flip en X para mirar a la izquierda
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }
}