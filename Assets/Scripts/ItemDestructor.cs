using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDestructor : MonoBehaviour
{

    [Header("Efectos")]
    public GameObject particulasRecogida; 

    // si algo lo atraviesa lo detecta
    private void OnTriggerEnter(Collider other)
    {
        // Comprobamos si el objeto que nos ha tocado tiene el script de la pelota
        if (other.GetComponent<ControladorPelota>() != null)
        {
            // Activamos el poder
            ControladorPelota.instancia.ActivarModoDestructor();

            // sistema de particulas para tener un feedback de que lo has recogido
            if (particulasRecogida != null)
            {
                Instantiate(particulasRecogida, transform.position, Quaternion.identity);
            }

            // Destruimos el objeto 
            Destroy(gameObject);
        }
    }
}
