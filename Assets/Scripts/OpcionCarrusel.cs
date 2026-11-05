
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class OpcionCarrusel
{
    public string nombre;
    public Sprite imagen;
    public UnityEvent accion;

    public bool esSlotVacio;
}
    