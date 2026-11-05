
using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorArchivo : MonoBehaviour
{
    [Header("Sistema de guardado")]
    public SistemaGuardado sistemaGuardado;

    [Header("Escenas")]
    public string escenaJugar;
    public string escenaLeaderboard;
    public string escenaCrearKahoot;
    public string escenaVolver;
    public string escenaDespuesBorrar;

    public void JugarKahoot()
    {
        CargarEscena(escenaJugar);
    }

    public void AbrirLeaderboard()
    {
        CargarEscena(escenaLeaderboard);
    }

    public void BorrarArchivo()
    {
        if (SlotActual.Instancia == null ||
            SlotActual.Instancia.slotSeleccionado == null)
        {
            Debug.LogError("No hay ningún slot seleccionado.");
            return;
        }

        if (sistemaGuardado == null)
        {
            Debug.LogError("Falta asignar SistemaGuardado.");
            return;
        }

        string id = SlotActual.Instancia.slotSeleccionado.id;

        bool eliminado = sistemaGuardado.EliminarSlot(id);

        if (eliminado)
        {
            Debug.Log("Slot y todo su contenido eliminados.");

            SlotActual.Instancia.slotSeleccionado = null;

            CargarEscena(escenaDespuesBorrar);
        }
    }

    public void CrearKahoot()
    {
        CargarEscena(escenaCrearKahoot);
    }

    public void Volver()
    {
        CargarEscena(escenaVolver);
    }

    private void CargarEscena(string nombreEscena)
    {
        if (string.IsNullOrWhiteSpace(nombreEscena))
        {
            Debug.LogWarning("No se ha asignado una escena de destino.");
            return;
        }

        SceneManager.LoadScene(nombreEscena);
    }
}
