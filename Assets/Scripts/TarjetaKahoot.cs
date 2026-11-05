using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TarjetaKahoot : MonoBehaviour
{
    public TMP_Text textoTitulo;
    public TMP_Text textoInformacion;
    public Button botonJugar;

    public void Configurar(KahootData kahoot, Action accionJugar)
    {
        textoTitulo.text = kahoot.titulo;

        int numeroPreguntas = kahoot.preguntas.Count;

        if (kahoot.HaSidoRealizado())
        {
            int mejorPuntuacion = 0;
            bool partidaPerfecta = false;

            foreach (ResultadoData resultado in kahoot.resultados)
            {
                if (resultado.puntuacion > mejorPuntuacion)
                {
                    mejorPuntuacion = resultado.puntuacion;
                }

                if (resultado.preguntasCorrectas == resultado.preguntasTotales)
                {
                    partidaPerfecta = true;
                }
            }

            textoInformacion.text =
                numeroPreguntas + " preguntas\n" +
                "Mejor puntuación: " + mejorPuntuacion + "\n" +
                (partidaPerfecta ? "Partida perfecta conseguida" : "Ya jugado");
        }
        else
        {
            textoInformacion.text =
                numeroPreguntas + " preguntas\n" +
                "Todavía no has jugado";
        }

        botonJugar.onClick.RemoveAllListeners();
        botonJugar.onClick.AddListener(() => accionJugar());
    }
}