using UnityEngine;
using UnityEngine.UIElements;

public class AnimacionUIController : MonoBehaviour
{
    public RenderTexture animacionTexture; // Asigna en el Inspector

    private VisualElement animacionElement;
    private Texture2D frameTexture;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Crear el VisualElement que contendrá la animación
        animacionElement = new VisualElement();
        animacionElement.style.width = 200;
        animacionElement.style.height = 200;
        animacionElement.style.backgroundSize = new BackgroundSize(Length.Percent(100), Length.Percent(100));


        // Agregamos al contenedor principal del UI Toolkit
        root.Q<VisualElement>("contenedor-principal").Add(animacionElement);

        // Crear una textura vacía para pintar los frames
        frameTexture = new Texture2D(animacionTexture.width, animacionTexture.height, TextureFormat.RGBA32, false);
    }

    private void Update()
    {
        if (animacionTexture == null || frameTexture == null)
            return;

        // Copiar el contenido de la RenderTexture al Texture2D
        RenderTexture.active = animacionTexture;
        frameTexture.ReadPixels(new Rect(0, 0, animacionTexture.width, animacionTexture.height), 0, 0);
        frameTexture.Apply();
        RenderTexture.active = null;

        // Mostrar el frame en el VisualElement
        animacionElement.style.backgroundImage = new StyleBackground(frameTexture);
    }
}
