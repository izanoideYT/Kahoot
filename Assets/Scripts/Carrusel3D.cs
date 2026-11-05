
using UnityEngine;
using UnityEngine.InputSystem;

public class Carrusel3D : MonoBehaviour
{
    [Header("Generación de tarjetas")]
    public GameObject tarjetaPrefab;
    public OpcionCarrusel[] opciones;

    [Header("Configuración de Tarjetas")]
    public float radio = 5f;
    public float angularSpacing = 25f;
    public float velocidadSuave = 5f;

    [Header("Efectos Visuales")]
    public float escalaCentro = 1.5f;
    public float escalaFondo = 0.8f;

    [Header("Interacción")]
    public Camera camaraPrincipal;
    public float toleranciaMovimiento = 0.1f;

    private Transform[] tarjetas;
    private Transform contenedorGenerado;

    private float anguloObjetivo = 0f;
    private float anguloActual = 0f;

    private int indiceSeleccionado = 0;

    private BotonCarrusel tarjetaHoverActual;

    private bool inicializado = false;

    void Start()
    {
        if (camaraPrincipal == null)
            camaraPrincipal = Camera.main;

        if (!inicializado)
            CrearTarjetas(opciones);
    }

    // Permite recibir opciones desde otros scripts
    public void GenerarCarrusel(OpcionCarrusel[] nuevasOpciones)
    {
        CrearTarjetas(nuevasOpciones);
    }

    void CrearTarjetas(OpcionCarrusel[] nuevasOpciones)
    {
        // Eliminar tarjetas anteriores
        if (tarjetaHoverActual != null)
        {
            tarjetaHoverActual.CambiarHover(false);
            tarjetaHoverActual = null;
        }

        if (contenedorGenerado != null)
            Destroy(contenedorGenerado.gameObject);

        opciones = nuevasOpciones;

        if (tarjetaPrefab == null)
        {
            Debug.LogError("No has asignado el Prefab de las tarjetas.");
            tarjetas = new Transform[0];
            return;
        }

        if (opciones == null || opciones.Length == 0)
        {
            Debug.LogWarning("No hay opciones configuradas.");
            tarjetas = new Transform[0];
            return;
        }

        GameObject contenedor = new GameObject("TarjetasGeneradas");

        contenedorGenerado = contenedor.transform;
        contenedorGenerado.SetParent(transform, false);

        tarjetas = new Transform[opciones.Length];

        for (int i = 0; i < opciones.Length; i++)
        {
            GameObject nuevaTarjeta = Instantiate(
                tarjetaPrefab,
                contenedorGenerado
            );

            nuevaTarjeta.name = opciones[i].nombre;

            tarjetas[i] = nuevaTarjeta.transform;

            AplicarImagen(nuevaTarjeta, opciones[i].imagen);

            BotonCarrusel boton =
                nuevaTarjeta.GetComponent<BotonCarrusel>();

            if (boton == null)
                boton = nuevaTarjeta.AddComponent<BotonCarrusel>();

            boton.Configurar(opciones[i].accion);
        }

        // Reiniciar el carrusel
        indiceSeleccionado = tarjetas.Length / 2;
        anguloActual = -indiceSeleccionado * angularSpacing;
        anguloObjetivo = anguloActual;

        inicializado = true;

        Debug.Log("Se han creado " + tarjetas.Length + " tarjetas.");
    }

    void AplicarImagen(GameObject tarjeta, Sprite imagen)
    {
        if (imagen == null)
            return;

        Renderer renderizado =
            tarjeta.GetComponentInChildren<Renderer>();

        if (renderizado == null)
        {
            Debug.LogWarning(
                "No se encontró Renderer en " + tarjeta.name
            );
            return;
        }

        Material material = renderizado.material;

        if (material.HasProperty("_BaseMap"))
            material.SetTexture("_BaseMap", imagen.texture);
        else if (material.HasProperty("_MainTex"))
            material.SetTexture("_MainTex", imagen.texture);
    }

    void Update()
    {
        if (tarjetas == null || tarjetas.Length == 0)
            return;

        // Navegación con teclado
        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame ||
                Keyboard.current.aKey.wasPressedThisFrame)
            {
                CambiarOpcion(-1);
            }

            if (Keyboard.current.rightArrowKey.wasPressedThisFrame ||
                Keyboard.current.dKey.wasPressedThisFrame)
            {
                CambiarOpcion(1);
            }
        }

        // Movimiento del carrusel
        anguloObjetivo = -indiceSeleccionado * angularSpacing;

        anguloActual = Mathf.Lerp(
            anguloActual,
            anguloObjetivo,
            Time.deltaTime * velocidadSuave
        );

        // Distribución circular
        for (int i = 0; i < tarjetas.Length; i++)
        {
            float anguloTarjeta =
                (i * angularSpacing) + anguloActual;

            float radianes = anguloTarjeta * Mathf.Deg2Rad;

            float x = Mathf.Sin(radianes) * radio;
            float z = Mathf.Cos(radianes) * radio - radio;

            tarjetas[i].localPosition =
                new Vector3(x, 0f, z);

            tarjetas[i].localRotation =
                Quaternion.Euler(0f, anguloTarjeta, 0f);

            float distanciaAlCentro =
                Mathf.Abs(Mathf.DeltaAngle(anguloTarjeta, 0f));

            float factorCercania =
                1f - Mathf.Clamp01(
                    distanciaAlCentro / (angularSpacing * 2)
                );

            float escala =
                Mathf.Lerp(escalaFondo, escalaCentro, factorCercania);

            tarjetas[i].localScale =
                new Vector3(escala, escala, 1f);
        }

        DetectarHover();

        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            DetectarClic();
        }
    }

    void DetectarHover()
    {
        if (camaraPrincipal == null || Mouse.current == null)
            return;

        Vector2 posicionRaton =
            Mouse.current.position.ReadValue();

        Ray rayo =
            camaraPrincipal.ScreenPointToRay(posicionRaton);

        BotonCarrusel nuevoHover = null;

        if (Physics.Raycast(rayo, out RaycastHit impacto))
        {
            for (int i = 0; i < tarjetas.Length; i++)
            {
                bool esTarjeta =
                    impacto.transform == tarjetas[i] ||
                    impacto.transform.IsChildOf(tarjetas[i]);

                if (esTarjeta)
                {
                    nuevoHover =
                        tarjetas[i].GetComponent<BotonCarrusel>();

                    break;
                }
            }
        }

        if (nuevoHover != tarjetaHoverActual)
        {
            if (tarjetaHoverActual != null)
                tarjetaHoverActual.CambiarHover(false);

            tarjetaHoverActual = nuevoHover;

            if (tarjetaHoverActual != null)
                tarjetaHoverActual.CambiarHover(true);
        }
    }

    public void CambiarOpcion(int direccion)
    {
        if (tarjetas == null || tarjetas.Length == 0)
            return;

        indiceSeleccionado = Mathf.Clamp(
            indiceSeleccionado + direccion,
            0,
            tarjetas.Length - 1
        );

        Debug.Log(
            "Mostrando tarjeta: " +
            tarjetas[indiceSeleccionado].name
        );
    }

    void DetectarClic()
    {
        if (camaraPrincipal == null)
            return;

        if (Mathf.Abs(anguloObjetivo - anguloActual) >
            toleranciaMovimiento)
        {
            return;
        }

        Vector2 posicionRaton =
            Mouse.current.position.ReadValue();

        Ray rayo =
            camaraPrincipal.ScreenPointToRay(posicionRaton);

        if (Physics.Raycast(rayo, out RaycastHit impacto))
        {
            for (int i = 0; i < tarjetas.Length; i++)
            {
                bool esTarjeta =
                    impacto.transform == tarjetas[i] ||
                    impacto.transform.IsChildOf(tarjetas[i]);

                if (!esTarjeta)
                    continue;

                if (i != indiceSeleccionado)
                    CambiarOpcion(i - indiceSeleccionado);
                else
                    ConfirmarTarjeta(i);

                break;
            }
        }
    }

    void ConfirmarTarjeta(int indice)
    {
        BotonCarrusel boton =
            tarjetas[indice].GetComponent<BotonCarrusel>();

        if (boton != null)
            boton.EjecutarAccion();
    }
}
