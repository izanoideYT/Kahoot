using System;
using System.Collections.Generic;

[Serializable]
public class KahootData
{
    public string id;
    public string titulo;
    public string fechaCreacion;

    public List<PreguntaData> preguntas = new List<PreguntaData>();
    public List<ResultadoData> resultados = new List<ResultadoData>();

    public KahootData(string titulo)
    {
        id = Guid.NewGuid().ToString();
        this.titulo = titulo;
        fechaCreacion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public bool HaSidoRealizado()
    {
        return resultados.Count > 0;
    }
}