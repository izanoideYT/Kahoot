using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GestorPartida : MonoBehaviour
{
    [Header("Textos")]
    public TMP_Text textoNumeroPregunta;
    public TMP_Text textoTemporizador;
    public TMP_Text textoPregunta;
    public TMP_Text textoResultado;

    [Header("Respuestas")]
    public Button[] botonesRespuestas;
    public TMP_Text[] textosRespuestas;

    [Header("Tiempo")]
    public Slider barraTiempo;

    [Header("Paneles")]
    public GameObject panelPartida;
    public GameObject panelFinal;

    [Header("Resultado final")]
    public TMP_Text textoPuntuacion;
    

    private KahootData kahoot;
    private int preguntaActual = 0;
    private int puntuacion = 0;
    private int preguntasCorrectas = 0;

    private float tiempoRestante;
    private bool respondiendo = false;

    private void Start()
    {
        if (SlotActual.Instancia == null ||
            SlotActual.Instancia.kahootSeleccionado == null)
        {
            Debug.LogError("No hay ningún Kahoot seleccionado.");
            return;
        }

        kahoot = SlotActual.Instancia.kahootSeleccionado;

        if (kahoot.preguntas == null || kahoot.preguntas.Count == 0)
        {
            Debug.LogError("El Kahoot no contiene preguntas.");
            return;
        }

        panelPartida.SetActive(true);
        panelFinal.SetActive(false);

        MostrarPregunta();
    }

    private void Update()
    {
        if (!respondiendo)
            return;

        tiempoRestante -= Time.deltaTime;

        textoTemporizador.text =
            Mathf.CeilToInt(tiempoRestante).ToString();

        barraTiempo.value = tiempoRestante /
            kahoot.preguntas[preguntaActual].duracion;

        if (tiempoRestante <= 0)
        {
            StartCoroutine(FinalizarPregunta(-1));
        }
    }

    private void MostrarPregunta()
    {
        if (preguntaActual >= kahoot.preguntas.Count)
        {
            FinalizarPartida();
            return;
        }

        PreguntaData pregunta = kahoot.preguntas[preguntaActual];

        textoNumeroPregunta.text =
            "Pregunta " + (preguntaActual + 1) +
            " / " + kahoot.preguntas.Count;

        textoPregunta.text = pregunta.enunciado;

        tiempoRestante = pregunta.duracion;

        barraTiempo.maxValue = 1;
        barraTiempo.value = 1;

        textoTemporizador.text = pregunta.duracion.ToString();

        textoResultado.text = "";

        respondiendo = true;

        for (int i = 0; i < botonesRespuestas.Length; i++)
        {
            botonesRespuestas[i].gameObject.SetActive(
                i < pregunta.respuestas.Length
            );

            botonesRespuestas[i].interactable = true;

            if (i < pregunta.respuestas.Length)
            {
                textosRespuestas[i].text = pregunta.respuestas[i];

                int indice = i;

                botonesRespuestas[i].onClick.RemoveAllListeners();

                botonesRespuestas[i].onClick.AddListener(
                    () => Responder(indice)
                );
            }
        }
    }

    public void Responder(int indiceRespuesta)
    {
        if (!respondiendo)
            return;

        StartCoroutine(FinalizarPregunta(indiceRespuesta));
    }

    private IEnumerator FinalizarPregunta(int indiceRespuesta)
    {
        respondiendo = false;

        PreguntaData pregunta = kahoot.preguntas[preguntaActual];

        bool correcta =
            indiceRespuesta == pregunta.indiceRespuestaCorrecta;

        if (correcta)
        {
            preguntasCorrectas++;

            puntuacion += 1000;
            textoResultado.text = "¡Respuesta correcta!";
        }
        else
        {
            if (indiceRespuesta == -1)
                textoResultado.text = "¡Se ha acabado el tiempo!";
            else
                textoResultado.text = "Respuesta incorrecta.";
        }

        for (int i = 0; i < botonesRespuestas.Length; i++)
        {
            botonesRespuestas[i].interactable = false;
        }

        yield return new WaitForSeconds(2f);

        preguntaActual++;

        MostrarPregunta();
    }

    private void FinalizarPartida()
    {
        panelPartida.SetActive(false);
        panelFinal.SetActive(true);

        textoPuntuacion.text =
            "Puntuación: " + puntuacion +
            "\nPreguntas correctas: " + preguntasCorrectas +
            " / " + kahoot.preguntas.Count;

        ResultadoData resultado = new ResultadoData(
            puntuacion,
            preguntasCorrectas,
            kahoot.preguntas.Count
        );

        kahoot.resultados.Add(resultado);

        SistemaGuardado sistema =
            FindFirstObjectByType<SistemaGuardado>();

        if (sistema != null)
        {
            sistema.GuardarSlot(SlotActual.Instancia.slotSeleccionado);
        }
        else
        {
            Debug.LogError("No se ha encontrado SistemaGuardado.");
        }
    }

    public void Volver(string escenaVolver)
    {
        SceneManager.LoadScene(escenaVolver);
    }
}