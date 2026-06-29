using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorGiro : MonoBehaviour
{
    [Header("Configuración de Control")]
    [Tooltip("Velocidad a la que gira el nivel")]
    public float sensibilidadRotacion = 50f;

    private Vector3 posicionRatonAnterior;

    private void Update()
    {
        //   Primer frame del click
        if (Input.GetMouseButtonDown(0))
        {
            posicionRatonAnterior = Input.mousePosition;
        }
        //  Mientras se mantiene el arrastre
        else if (Input.GetMouseButton(0))
        {
            Vector3 deltaRaton = Input.mousePosition - posicionRatonAnterior;

            // Al poner el negativo, si arrastras el ratón a la derecha, 
            // el objeto gira en su eje Y hacia la izquierda (acompañando al dedo).
            float rotacionY = -deltaRaton.x * sensibilidadRotacion * Time.deltaTime;

            // Giramos el contenedor del nivel
            transform.Rotate(0, rotacionY, 0);

            // Actualizamos para el siguiente frame
            posicionRatonAnterior = Input.mousePosition;
        }
    }
}
