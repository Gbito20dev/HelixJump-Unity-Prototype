using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorCamara : MonoBehaviour
{
    // Patrón Singleton para poder llamarla desde el GameManager
    public static ControladorCamara instancia;

    [Header("Objetivo a seguir")]
    [Tooltip("Arrastra aquí la Pelota")]
    public Transform objetivo;

    [Header("Ajustes de Visión")]
    public Vector3 offset = new Vector3(0, 3f, -8f);
    public Vector3 anguloRotacion = new Vector3(20f, 0, 0);

    [Header("Ajustes de Suavidad")]
    public float velocidadSuavizado = 10f;

    [Header("Efecto Game Over (Shake)")]
    [Tooltip("Duración del temblor en segundos")]
    public float duracionTemblor = 0.3f;
    [Tooltip("Violencia del temblor")]
    public float magnitudTemblor = 0.2f;

    private float alturaMinimaAlcanzada;

    private void Awake()
    {
        if (instancia == null) instancia = this;
    }

    private void Start()
    {
        transform.rotation = Quaternion.Euler(anguloRotacion);
        if (objetivo != null) alturaMinimaAlcanzada = transform.position.y;
    }

    private void LateUpdate()
    {
        // El movimiento normal de la cámara
        if (objetivo != null)
        {
            float posicionYObjetivo = objetivo.position.y + offset.y;
            if (posicionYObjetivo < alturaMinimaAlcanzada)
            {
                alturaMinimaAlcanzada = posicionYObjetivo;
            }

            Vector3 posicionDeseada = new Vector3(offset.x, alturaMinimaAlcanzada, offset.z);

            // Si el tiempo está a 0 (pausa), Time.deltaTime es 0, así que este Lerp no se moverá.
            // Asi tambien no se bugea con el tembleque si pierde
            transform.position = Vector3.Lerp(transform.position, posicionDeseada, velocidadSuavizado * Time.deltaTime);
        }
    }

    // --- LÓGICA DEL TEMBLEQUE DE PERDER ---
    public void ActivarShake()
    {
        StartCoroutine(RutinaTemblor());
    }

    private IEnumerator RutinaTemblor()
    {
        //   Guardamos exactamente dónde estaba la cámara en el momento de morir
        Vector3 posicionOriginal = transform.position;
        float tiempo = 0f;

        //   Bucle que dura lo que le hayas puesto en el Inspector
        while (tiempo < duracionTemblor)
        {
            //   unscaledDeltaTime avanza aunque el juego esté en pausa (Time.timeScale = 0)
            tiempo += Time.unscaledDeltaTime;

            //   Generamos unas coordenadas aleatorias en un radio pequeño
            Vector3 desplazamientoAleatorio = Random.insideUnitSphere * magnitudTemblor;

            //   Ponemos la Z a cero para que la cámara no haga zoom hacia adelante y atrás
            desplazamientoAleatorio.z = 0;

            //   Aplicamos el temblor sumado a la posición en la que estábamos
            transform.position = posicionOriginal + desplazamientoAleatorio;

            yield return null; // Esperamos al siguiente fotograma visual
        }

        //   Al terminar, la dejamos clavada en su posición original para que no quede torcida
        transform.position = posicionOriginal;
    }
}

