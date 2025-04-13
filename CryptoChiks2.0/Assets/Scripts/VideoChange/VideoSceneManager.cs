using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoSceneManager : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Asigna tu componente de VideoPlayer desde el inspector
    public string loginSceneName = "LoginScene";

    private bool hasSwitchedScene = false;

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }

    void Update()
    {
        if (!hasSwitchedScene && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape)))
        {
            SwitchToLoginScene();
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        SwitchToLoginScene();
    }

    void SwitchToLoginScene()
    {
        hasSwitchedScene = true;
        SceneManager.LoadScene(loginSceneName);
    }
}