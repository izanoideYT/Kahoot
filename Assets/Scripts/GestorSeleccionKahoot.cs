using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GestorSeleccionKahoot : MonoBehaviour
{
    [Header("Referencias")]
    public Transform contenedorTarjetas;
    public GameObject prefabTarjeta;
    public TMP_Text textoNombreSlot;

    [Header("Escenas")]
    public string escenaPartida;
    public string escenaVolver;

    private void Start()
    {
        Debug.Log("Comprobando SlotActual...");

        if (SlotActual.Instancia == null)
        {
            Debug.LogError("La instancia estática SlotActual.Instancia es NULL.");
            return;
        }

        Debug.Log("Instancia encontrada.");

        if (SlotActual.Instancia.slotSeleccionado == null)
        {
            Debug.LogError("La instancia existe, pero no tiene slot seleccionado.");
            return;
        }

        Debug.Log("Slot seleccionado: " +
            SlotActual.Instancia.slotSeleccionado.nombre);

        ActualizarLista();
    }

    public void ActualizarLista()
    {
        Debug.Log("Kahoots guardados: " +
            SlotActual.Instancia.slotSeleccionado.kahoots.Count);

        if (SlotActual.Instancia == null ||
            SlotActual.Instancia.slotSeleccionado == null)
        {
            Debug.LogError("No hay ningún slot seleccionado.");
            return;
        }

        SlotData slot = SlotActual.Instancia.slotSeleccionado;

        textoNombreSlot.text = slot.nombre;

        foreach (Transform hijo in contenedorTarjetas)
        {
            Destroy(hijo.gameObject);
        }

        foreach (KahootData kahoot in slot.kahoots)
        {
            GameObject nuevaTarjeta = Instantiate(
                prefabTarjeta,
                contenedorTarjetas
            );

            TarjetaKahoot tarjeta =
                nuevaTarjeta.GetComponent<TarjetaKahoot>();

            tarjeta.Configurar(kahoot, () => Jugar(kahoot));
        }
    }

    private void Jugar(KahootData kahoot)
    {
        SlotActual.Instancia.kahootSeleccionado = kahoot;

        SceneManager.LoadScene(escenaPartida);
    }

    public void Volver()
    {
        SceneManager.LoadScene(escenaVolver);
    }
}