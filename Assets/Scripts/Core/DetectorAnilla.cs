using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorAnilla : MonoBehaviour
{
    private Transform pelota;
    private bool yaPuntuada = false;

    [Header("Efecto de Rotura")]
    public float fuerzaExplosion = 15f;
    public float fuerzaGiro = 300f;

    private void Start()
    {
        pelota = GameObject.Find("Pelota").transform;
    }

    // --- FUNCION QUE IDENTIFICA SI ES LA META ---
    // Busca en todas las piezas, por muy escondidas que estén, si alguna tiene el tag Meta
    private bool EsPlataformaMeta()
    {
        Transform[] todasLasPiezas = GetComponentsInChildren<Transform>();
        foreach (Transform pieza in todasLasPiezas)
        {
            if (pieza.CompareTag("Meta"))
            {
                return true; // Hemos encontrado la meta
            }
        }
        return false; // No hay meta aquí, es un piso normal
    }

    private void Update()
    {
        //seguro por si la anilla ya esta puntuada que no se vuelva a puntuar
        if (!yaPuntuada && pelota != null)
        {
            float alturaDeRotura = transform.position.y;

            //si tengo el objeto equipado las anillas se rompen antes de tocarlas, por eso se sube 
            if (ControladorPelota.instancia != null && ControladorPelota.instancia.modoDestructor)
            {
                alturaDeRotura += 1.5f;
            }
            //cuando la altura pasa la altura de rotura
            if (pelota.position.y < alturaDeRotura)
            {
                // ¡AQUÍ ESTÁ LA PROTECCIÓN!
                // Si la pelota ha cruzado la línea, comprobamos si somos la Meta ANTES de rompernos
                if (EsPlataformaMeta())
                {
                    yaPuntuada = true; // Lo bloqueamos para que no lo compruebe más
                    return; // SALIMOS de la función. La anilla no se rompe ni da puntos extra.
                }

                // Si llegamos aquí, es que NO era la meta, así que la rompemos normalmente y sumamos puntos
                yaPuntuada = true;
                GameManager.instancia.SumarPuntos(2);
                StartCoroutine(AnimacionRotura());
            }
        }
    }
    //sistema de anicacion de romperse la anilla cuadno la superas
    private IEnumerator AnimacionRotura()
    {
        // apagamos los colliders lo primero para evitar bugs
        Collider[] colisionadores = GetComponentsInChildren<Collider>();
        foreach (Collider col in colisionadores)
        {
            col.enabled = false;
        }

        float tiempoAnimacion = 0.4f;
        float tiempoActual = 0f;
        //creamos las direccion hacia donde va a ir la plataforma
        Vector3[] escalasIniciales = new Vector3[transform.childCount];
        Vector3[] direccionesSalida = new Vector3[transform.childCount];
        // Precalcula el estado inicial y la trayectoria radial de cada pieza para optimizar el bucle de animación.
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform pieza = transform.GetChild(i);
            escalasIniciales[i] = pieza.localScale;
            // Se utiliza el centro del Renderer (bounds) en lugar del Transform para obtener una dirección física más precisa.
            Renderer renderizador = pieza.GetComponentInChildren<Renderer>();
            Vector3 centroVisual = pieza.position;

            if (renderizador != null)
            {
                centroVisual = renderizador.bounds.center;
            }

            Vector3 direccionAfuera = (centroVisual - transform.position);
            direccionAfuera.y = 0; // Fuerza un desplazamiento inicial estrictamente horizontal.

            // Fallback de seguridad en caso de que el pivote coincida exactamente con el centro del objeto.
            if (direccionAfuera.magnitude < 0.1f) direccionAfuera = pieza.forward;
            // Calcula el vector de trayectoria combinando el empuje horizontal con una ligera caída por gravedad simulada.
            direccionesSalida[i] = (direccionAfuera.normalized + (Vector3.down * 0.5f)).normalized;
        }

        // Bucle de interpolación temporal para la animación.
        while (tiempoActual < tiempoAnimacion)
        {
            tiempoActual += Time.deltaTime;
            float progreso = tiempoActual / tiempoAnimacion;

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform pieza = transform.GetChild(i);

                // Aplica transformaciones de escala, traslación y rotación.
                pieza.localScale = Vector3.Lerp(escalasIniciales[i], Vector3.zero, progreso);
                pieza.Translate(direccionesSalida[i] * fuerzaExplosion * Time.deltaTime, Space.World);

                // Alterna el eje de rotación (X o Z) dependiendo del índice de la pieza para un efecto más orgánico.
                Vector3 ejeGiro = new Vector3(i % 2 == 0 ? 1 : 0, 1, i % 2 != 0 ? 1 : 0);
                pieza.Rotate(ejeGiro * fuerzaGiro * Time.deltaTime, Space.Self);
            }

            yield return null;
        }

        // Libera la memoria eliminando el objeto completo una vez la animación finaliza.
        Destroy(gameObject);
    }
}