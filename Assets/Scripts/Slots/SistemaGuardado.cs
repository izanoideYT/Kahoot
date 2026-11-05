
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SistemaGuardado : MonoBehaviour
{
    private string rutaSlots;

    private void Awake()
    {
        rutaSlots = Path.Combine(
            Application.persistentDataPath,
            "Saves",
            "Slots"
        );

        if (!Directory.Exists(rutaSlots))
        {
            Directory.CreateDirectory(rutaSlots);
        }
    }

    // Crear un slot nuevo
    public SlotData CrearSlot(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            Debug.LogWarning("El nombre del slot no puede estar vacío.");
            return null;
        }

        SlotData nuevoSlot = new SlotData(nombre);

        GuardarSlot(nuevoSlot);

        return nuevoSlot;
    }

    // Guardar los datos de un slot
    public void GuardarSlot(SlotData slot)
    {
        if (slot == null || string.IsNullOrEmpty(slot.id))
        {
            Debug.LogError("No se puede guardar un slot inválido.");
            return;
        }

        try
        {
            string rutaArchivo = Path.Combine(
                rutaSlots,
                slot.id + ".json"
            );

            string json = JsonUtility.ToJson(slot, true);

            File.WriteAllText(rutaArchivo, json);

            Debug.Log("Slot guardado: " + slot.nombre);
        }
        catch (Exception e)
        {
            Debug.LogError("Error al guardar el slot: " + e.Message);
        }
    }

    // Cargar todos los slots existentes
    public List<SlotData> CargarSlots()
    {
        List<SlotData> slots = new List<SlotData>();

        try
        {
            string[] archivos = Directory.GetFiles(rutaSlots, "*.json");

            foreach (string archivo in archivos)
            {
                try
                {
                    string json = File.ReadAllText(archivo);

                    SlotData slot = JsonUtility.FromJson<SlotData>(json);

                    if (slot != null && !string.IsNullOrEmpty(slot.id))
                    {
                        slots.Add(slot);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(
                        "Error al cargar " + Path.GetFileName(archivo) +
                        ": " + e.Message
                    );
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error al leer los slots: " + e.Message);
        }

        return slots;
    }

    // Buscar un slot por su identificador
    public SlotData ObtenerSlot(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        string rutaArchivo = Path.Combine(rutaSlots, id + ".json");

        if (!File.Exists(rutaArchivo))
            return null;

        try
        {
            string json = File.ReadAllText(rutaArchivo);
            return JsonUtility.FromJson<SlotData>(json);
        }
        catch (Exception e)
        {
            Debug.LogError("Error al obtener el slot: " + e.Message);
            return null;
        }
    }

    // Eliminar un slot y todos sus datos
    public bool EliminarSlot(string id)
    {
        if (string.IsNullOrEmpty(id))
            return false;

        string rutaArchivo = Path.Combine(rutaSlots, id + ".json");

        if (!File.Exists(rutaArchivo))
        {
            Debug.LogWarning("El slot que intentas eliminar no existe.");
            return false;
        }

        try
        {
            File.Delete(rutaArchivo);

            Debug.Log("Slot eliminado: " + id);

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Error al eliminar el slot: " + e.Message);
            return false;
        }
    }
}
