
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class GestorSlots : MonoBehaviour
{
    [Header("Referencias principales")]
    public Carrusel3D carrusel;
    public SistemaGuardado sistemaGuardado;

    [Header("Imágenes")]
    public Sprite imagenSlot;
    public Sprite imagenCrearSlot;

    [Header("Panel de creación")]
    public GameObject panelCrearSlot;
    public TMP_InputField inputNombre;
    public Button botonCrear;
    public Button botonCancelar;

    [Header("Eventos")]
    public UnityEvent alSeleccionarSlot;

    [HideInInspector]
    public SlotData slotSeleccionado;

    private void Start()
    {
        // Comprobar referencias
        if (carrusel == null || sistemaGuardado == null ||
            panelCrearSlot == null || inputNombre == null ||
            botonCrear == null || botonCancelar == null)
        {
            Debug.LogError("Faltan referencias en GestorSlots.");
            enabled = false;
            return;
        }

        panelCrearSlot.SetActive(false);

        botonCrear.onClick.AddListener(CrearSlot);
        botonCancelar.onClick.AddListener(CerrarPanel);

        CargarSlots();
    }

    // Cargar slots y generar sus tarjetas
    public void CargarSlots()
    {
        List<SlotData> slots = sistemaGuardado.CargarSlots();

        List<OpcionCarrusel> opciones = new List<OpcionCarrusel>();

        foreach (SlotData slot in slots)
        {
            OpcionCarrusel opcion = new OpcionCarrusel();

            opcion.nombre = slot.nombre;
            opcion.imagen = imagenSlot;
            opcion.esSlotVacio = false;

            opcion.accion = new UnityEvent();

            string idSlot = slot.id;

            opcion.accion.AddListener(() =>
            {
                SeleccionarSlot(idSlot);
            });

            opciones.Add(opcion);
        }

        // Añadir tarjeta para crear un slot
        OpcionCarrusel opcionCrear = new OpcionCarrusel();

        opcionCrear.nombre = "Crear nuevo slot";
        opcionCrear.imagen = imagenCrearSlot;
        opcionCrear.esSlotVacio = true;
        opcionCrear.accion = new UnityEvent();

        opcionCrear.accion.AddListener(AbrirPanel);

        opciones.Add(opcionCrear);

        carrusel.GenerarCarrusel(opciones.ToArray());
    }

    // Seleccionar un slot existente
    public void SeleccionarSlot(string id)
    {
        SlotData slot = sistemaGuardado.ObtenerSlot(id);

        if (slot == null)
        {
            Debug.LogError("No se ha podido cargar el slot.");
            return;
        }

        if (SlotActual.Instancia == null)
        {
            Debug.LogError("No existe ninguna instancia de SlotActual.");
            return;
        }

        SlotActual.Instancia.slotSeleccionado = slot;
        slotSeleccionado = slot;

        Debug.Log("Slot guardado en memoria: " +
                  SlotActual.Instancia.slotSeleccionado.nombre);

        UnityEngine.SceneManagement.SceneManager.LoadScene("Archivo");
    }

    // Mostrar el panel
    public void AbrirPanel()
    {
        inputNombre.text = "";

        panelCrearSlot.SetActive(true);

        inputNombre.ActivateInputField();
    }

    // Crear un nuevo slot
    public void CrearSlot()
    {
        string nombre = inputNombre.text.Trim();

        if (string.IsNullOrWhiteSpace(nombre))
        {
            Debug.LogWarning("Debes escribir un nombre.");
            inputNombre.ActivateInputField();
            return;
        }

        SlotData nuevoSlot = sistemaGuardado.CrearSlot(nombre);

        if (nuevoSlot == null)
        {
            Debug.LogError("No se ha podido crear el slot.");
            return;
        }

        Debug.Log("Slot creado: " + nuevoSlot.nombre);

        panelCrearSlot.SetActive(false);

        CargarSlots();
    }

    // Cerrar el panel
    public void CerrarPanel()
    {
        panelCrearSlot.SetActive(false);
    }

    public void EliminarSlotSeleccionado()
    {
        if (slotSeleccionado == null)
        {
            Debug.LogWarning("No hay ningún slot seleccionado.");
            return;
        }

        string idSlot = slotSeleccionado.id;

        bool eliminado = sistemaGuardado.EliminarSlot(idSlot);

        if (eliminado)
        {
            Debug.Log("Slot eliminado correctamente: " + slotSeleccionado.nombre);

            slotSeleccionado = null;

            CargarSlots();
        }
        else
        {
            Debug.LogError("No se ha podido eliminar el slot.");
        }
    }
}
