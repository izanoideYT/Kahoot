
using UnityEngine;

public class SlotActual : MonoBehaviour
{
    public static SlotActual Instancia;

    public SlotData slotSeleccionado;
    public KahootData kahootSeleccionado;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;
        DontDestroyOnLoad(gameObject);
    }
}
