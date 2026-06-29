using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{



    [Header("Datos del nivel a generar")]
    public LevelData nivelActual; // aqui se arrastra el nivel Actual

    [Header("Contenedor Base")]
    [Tooltip("El objeto vacío que contendrá todo el nivel para no ensuciar la escena")]
    public Transform levelContainer;




    [Header("Referencia a prefabas 3D")]
    public GameObject prefab_mitad_seguro;
    public GameObject prefab_cuarto_seguro;
    public GameObject prefab_mitad_malo;// estos dos dañan al jugador si tocan
    public GameObject prefab_cuarto_malo;
    public GameObject meta;
    [Header("Items y Modificadores")]
    [Tooltip("Arrastra aquí tu Prefab del Item Destructor")]
    public GameObject prefabDestructor;

    [Tooltip("Probabilidad de que aparezca en un piso (0.1 = 10%)")]
    [Range(0f, 1f)]
    public float probabilidadDestructor = 0.1f;
    [Tooltip("Altura a la que flota el item sobre la anilla")]
    public float alturaSpawnItem = 1.5f;
    [Tooltip("Distancia hacia afuera desde el palo central")]
    public float distanciaAfueraItem = 2.5f;


    [Header("Referencias de la Escena")]
    public Transform pilarCentral; // El palo central

    // punto visual para decidir dónde empieza el nivel
    [Tooltip("Crea un objeto vacío, ponlo en la punta del pilar y arrástralo aquí")]
    public Transform puntoDeInicioArriba;


    // Esta será la función que llamaremos desde nuestro botón para crear nivel en el Editor, sin tener que darle al Play
    [ContextMenu("Construir Nivel Ahora")]
    public void ConstruirNivel()
    {
        //comprobacion de que hay parametros asignados
        if (nivelActual == null)
        {
            Debug.LogError("No has asignado un LevelData al Generador");
            return;
        }
        if (levelContainer == null)
        {
            Debug.LogError("No has asignado un levelcontainer al Generador");
            return;
        }

        //  Borrar cualquier nivel anterior

        LimpiarNivel();


        //  Uempezamos en y = 0  y vamos bajando

        float posicionYactual = puntoDeInicioArriba.position.y;

        // recorremos cada anilla del nivel
        for (int i = 0; i < nivelActual.anillas.Count; i++)
        {
            AnillaData anilla = nivelActual.anillas[i];

            //creamos un objeto vacio para agrupar las piezas la anilla
            GameObject anillaParent = new GameObject("Anilla_" + i);
            anillaParent.AddComponent<DetectorAnilla>();
            anillaParent.transform.parent = levelContainer;

            //colocamos la anilla en la altura que le toca
            anillaParent.transform.localPosition = new Vector3(0, posicionYactual, 0);

            //aplicamos la rotacion base a para que no coincida con el de arriba
            anillaParent.transform.localRotation = Quaternion.Euler(0, anilla.baseRotationY, 0);

            //4 recorremos y creamos las piezas de esta anilla

            foreach(PiezaData pieza in anilla.pieces)
            {
                GameObject prefabAInstanciar = ObtenerPrefabCorrecto(pieza);

                if( prefabAInstanciar != null )
                {
                    //instaciamos el prefab y lo hacemos hijo de anilla parent
                    GameObject nuevaPieza = Instantiate(prefabAInstanciar, anillaParent.transform);

                    //aseguramos qu eesta en el centro de su padre
                    nuevaPieza.transform.localPosition = Vector3.zero;

                    //le damos rotacion (0.90.180.270)
                    nuevaPieza.transform.localRotation = Quaternion.Euler(0, pieza.localRotationY, 0);

                }
            }
            // ---  LÓGICA DEL ITEM DESTRUCTOR ---
            // Tiramos los dados para ver si toca crear el item en este piso (y que no sea en la meta)
            if (prefabDestructor != null && Random.value <= probabilidadDestructor && i < nivelActual.anillas.Count - 1)
            {
                // 1. Usamos tu variable para la altura (Y)
                Vector3 posicionItem = anillaParent.transform.position + new Vector3(0, alturaSpawnItem, 0);

                // 2. Usamos tu variable para sacarlo hacia afuera del pilar (Z local / Forward)
                posicionItem += anillaParent.transform.forward * distanciaAfueraItem;

                GameObject item = Instantiate(prefabDestructor, posicionItem, Quaternion.identity);

                // Lo hacemos hijo de la anilla. Así gira con ella.
                item.transform.parent = anillaParent.transform;
            }
            //   Restamos la distacia para que el siguiente piso este mas abajo
            posicionYactual -= nivelActual.distanciaEntreAnillas;
        }


       

        Debug.Log("<color=green>¡Nivel Construido con Éxito!</color>");
    }

    //FUNCION DE LIMPIAR NIVEL PERO NO DE BORRAR EL LEVEL DATA
    [ContextMenu("Limpiar Nivel")]
    public void LimpiarNivel()
    {
        for (int i = levelContainer.childCount - 1; i >= 0; i--)
        {
            // Si estamos jugando usamos Destroy normal, si estamos en el Editor usamos DestroyImmediate
            if (Application.isPlaying)
            {
                Destroy(levelContainer.GetChild(i).gameObject);
            }
            else
            {
                DestroyImmediate(levelContainer.GetChild(i).gameObject);
            }
        }
    }

    // --- FUNCIÓN HELPER   ---
    // Solo decide qué prefab usar.
    private GameObject ObtenerPrefabCorrecto(PiezaData pieza)
    {
        if (pieza.tipoFormaPieza == PieceShape.MedioCirculo_180)
        {
            switch (pieza.tipoPieza)
            {
                case PieceType.Segura: return prefab_mitad_seguro;
                case PieceType.Peligro: return prefab_mitad_malo;
                case PieceType.Meta: return meta; // <-- Añadido aquí directamente
            }
        }
        else if (pieza.tipoFormaPieza == PieceShape.CuartoCirculo_90)
        {
            switch (pieza.tipoPieza)
            {
                case PieceType.Segura: return prefab_cuarto_seguro;
                case PieceType.Peligro: return prefab_cuarto_malo;
                case PieceType.Meta: return meta; // <-- Añadido aquí también por si acaso
            }
        }

        return null;
    }
}

