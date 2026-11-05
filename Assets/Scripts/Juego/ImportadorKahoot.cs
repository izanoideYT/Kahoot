using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ImportadorKahoot : MonoBehaviour
{
    [Header("Sistema de guardado")]
    public SistemaGuardado sistemaGuardado;

    [Header("Archivo JSON")]
    public string nombreArchivo = "Videojuegos.json";


    private void Start()
    {
        Debug.Log("Importar Iniciado");
        ImportarKahoot();
    }
    public void ImportarKahoot()
    {
        Debug.Log("BOTÓN DE IMPORTAR PULSADO");

        if (SlotActual.Instancia == null ||
            SlotActual.Instancia.slotSeleccionado == null)
        {
            Debug.LogError("No hay ningún slot seleccionado.");
            return;
        }

        if (sistemaGuardado == null)
        {
            Debug.LogError("No se ha asignado SistemaGuardado.");
            return;
        }

        StartCoroutine(LeerJSON());
    }

    private IEnumerator LeerJSON()
    {
        string ruta = Path.Combine(
            Application.streamingAssetsPath,
            "Kahoots/" + nombreArchivo
        );

        using (UnityWebRequest peticion = UnityWebRequest.Get(ruta))
        {
            yield return peticion.SendWebRequest();

            if (peticion.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error al leer el JSON: " + peticion.error);
                yield break;
            }

            string contenido = peticion.downloadHandler.text;

            KahootData kahoot = JsonUtility.FromJson<KahootData>(contenido);

            if (!ValidarKahoot(kahoot))
            {
                Debug.LogError("El JSON no contiene un Kahoot válido.");
                yield break;
            }

            SlotData slot = SlotActual.Instancia.slotSeleccionado;

            if (slot.kahoots == null)
            {
                slot.kahoots = new System.Collections.Generic.List<KahootData>();
            }

            foreach (KahootData kahootExistente in slot.kahoots)
            {
                if (kahootExistente.id == kahoot.id)
                {
                    Debug.LogWarning("Este Kahoot ya está importado en el slot.");
                    yield break;
                }
            }

            slot.kahoots.Add(kahoot);

            sistemaGuardado.GuardarSlot(slot);
            FindFirstObjectByType<GestorSeleccionKahoot>().ActualizarLista();

            Debug.Log("Kahoot importado correctamente: " + kahoot.titulo);
        }
    }

    private bool ValidarKahoot(KahootData kahoot)
    {
        if (kahoot == null ||
            string.IsNullOrWhiteSpace(kahoot.id) ||
            string.IsNullOrWhiteSpace(kahoot.titulo) ||
            kahoot.preguntas == null ||
            kahoot.preguntas.Count == 0)
        {
            return false;
        }

        foreach (PreguntaData pregunta in kahoot.preguntas)
        {
            if (pregunta == null ||
                string.IsNullOrWhiteSpace(pregunta.enunciado) ||
                pregunta.respuestas == null ||
                pregunta.respuestas.Length < 2 ||
                pregunta.indiceRespuestaCorrecta < 0 ||
                pregunta.indiceRespuestaCorrecta >= pregunta.respuestas.Length ||
                pregunta.duracion <= 0)
            {
                return false;
            }
        }

        return true;
    }
}