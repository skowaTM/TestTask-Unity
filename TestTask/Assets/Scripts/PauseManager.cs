using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public bool IsPaused { get; private set; } = false;


    public void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;
    }

    public void UnPause()
    {
        IsPaused = false;
        Time.timeScale = 1f;
    }
}
