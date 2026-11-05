using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class IntroControllerCanvas : MonoBehaviour
{
    // Este es el método único que usarán ambos botones
    public void CambiarEscena(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }

    public void Salir()
    {
        Debug.Log("Salir...");
        Application.Quit();
    }
}

