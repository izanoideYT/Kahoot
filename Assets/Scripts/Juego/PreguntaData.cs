using System;

[Serializable]
public class PreguntaData
{
    public string enunciado;
    public string[] respuestas;
    public int indiceRespuestaCorrecta;
    public int duracion;
}