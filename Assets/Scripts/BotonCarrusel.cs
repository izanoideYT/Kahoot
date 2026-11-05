
using UnityEngine;
using UnityEngine.Events;

public class BotonCarrusel : MonoBehaviour
{
    [Header("Acción al confirmar")]
    public UnityEvent accion;

    [Header("Efecto Hover")]
    public Color colorHover = Color.cyan;

    [Header("Escalado Hover")]
    public Transform visual;
    public float multiplicadorHover = 1.1f;
    public float velocidadEscala = 8f;

    private Renderer[] renderizadores;
    private Color[] coloresOriginales;
    private string[] propiedadesColor;

    private Vector3 escalaOriginal;
    private Vector3 escalaObjetivo;

    private bool estaEnHover = false;

    void Awake()
    {
        renderizadores = GetComponentsInChildren<Renderer>();

        coloresOriginales = new Color[renderizadores.Length];
        propiedadesColor = new string[renderizadores.Length];

        for (int i = 0; i < renderizadores.Length; i++)
        {
            Material material = renderizadores[i].sharedMaterial;

            if (material.HasProperty("_BaseColor"))
            {
                propiedadesColor[i] = "_BaseColor";
            }
            else if (material.HasProperty("_Color"))
            {
                propiedadesColor[i] = "_Color";
            }

            if (!string.IsNullOrEmpty(propiedadesColor[i]))
            {
                coloresOriginales[i] =
                    material.GetColor(propiedadesColor[i]);
            }
        }

        if (visual != null)
        {
            escalaOriginal = visual.localScale;
            escalaObjetivo = escalaOriginal;
        }
    }

    void Update()
    {
        if (visual == null)
            return;

        float multiplicador = estaEnHover ? multiplicadorHover : 1f;

        escalaObjetivo = escalaOriginal * multiplicador;

        visual.localScale = Vector3.Lerp(
            visual.localScale,
            escalaObjetivo,
            Time.deltaTime * velocidadEscala
        );
    }

    public void Configurar(UnityEvent nuevaAccion)
    {
        accion = nuevaAccion;
    }

    public void EjecutarAccion()
    {
        Debug.Log("Botón confirmado: " + gameObject.name);

        if (accion != null)
        {
            accion.Invoke();
        }
    }

    public void CambiarHover(bool activar)
    {
        if (estaEnHover == activar)
            return;

        estaEnHover = activar;

        for (int i = 0; i < renderizadores.Length; i++)
        {
            if (string.IsNullOrEmpty(propiedadesColor[i]))
                continue;

            MaterialPropertyBlock bloque =
                new MaterialPropertyBlock();

            renderizadores[i].GetPropertyBlock(bloque);

            bloque.SetColor(
                propiedadesColor[i],
                activar ? colorHover : coloresOriginales[i]
            );

            renderizadores[i].SetPropertyBlock(bloque);
        }
    }
}
