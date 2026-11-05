
using System;
using System.Collections.Generic;

[Serializable]
public class SlotData
{
    public string id;
    public string nombre;
    public string fechaCreacion;

    public List<KahootData> kahoots = new List<KahootData>();

    public SlotData(string nombre)
    {
        id = Guid.NewGuid().ToString();
        this.nombre = nombre;
        fechaCreacion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
