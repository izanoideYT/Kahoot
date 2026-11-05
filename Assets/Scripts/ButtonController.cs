using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{

    public void Escena(string escena)
    {
        SceneManager.LoadScene(escena);

    }

    public void salir()
    {
        Application.Quit();
        Debug.Log("Salir..");
    }
}
