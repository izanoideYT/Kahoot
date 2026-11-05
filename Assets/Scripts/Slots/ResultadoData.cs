
using System;

[Serializable]
public class ResultadoData
{
    public int puntuacion;
    public int preguntasCorrectas;
    public int preguntasTotales;
    public string fechaPartida;

    public ResultadoData(int puntuacion, int correctas, int totales)
    {
        this.puntuacion = puntuacion;
        preguntasCorrectas = correctas;
        preguntasTotales = totales;
        fechaPartida = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
