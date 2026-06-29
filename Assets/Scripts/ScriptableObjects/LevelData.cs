using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Definimos los tipos de piezas 
public enum PieceShape
{
    MedioCirculo_180,
    CuartoCirculo_90
}

// Definimos si la pieza rebota, te mata, o si es la base final
public enum PieceType
{
    Segura,
    Peligro,
    Meta
}


// Etiqueta para crear assets desde el menu con clicj derecho
[CreateAssetMenu(fileName = "Level_", menuName = "HelixJump/Nuevo Nivel", order = 1)]
public class LevelData : ScriptableObject
{
    [Header("Configuracion del Nivel")]
    [Tooltip("Color de fondo ")]
    public Color colordefondo = Color.cyan;

    [Tooltip("El material o color  del cilindro ")]
    public Color pilarColor = Color.white;
    [Tooltip("Distancia vertucal (eje y) que habra en entreun piso y el siguiente")]
    public float distanciaEntreAnillas = 3f;

    [Header("Estructura de la torre")]
    [Tooltip("Lista de anillas, inicio arriba - final abajo")]
    public List<AnillaData> anillas = new List<AnillaData> ();

    //  OnValidate se ejecuta SOLA en el mismo milisegundo 
    // en el que modificas cualquier valor de este archivo en el Inspector.
    private void OnValidate()
    {
        // Le decimos al compilador que esto SOLO debe funcionar en el Editor, 
        // para que no de errores cuando exportes el .zip del juego final.
#if UNITY_EDITOR

        // Usamos delayCall porque a Unity no le gusta destruir y crear 
        // cientos de objetos en el mismo frame que actualiza la interfaz. 
        // Le pedimos que espere un respiro (1 frame).
        UnityEditor.EditorApplication.delayCall += () =>
        {
            // Buscamos directamente el generador en la escena (¡sin eventos!)
            LevelGenerator generador = FindObjectOfType<LevelGenerator>();

            // Si lo encuentra, le damos al botón por código
            if (generador != null)
            {
                generador.ConstruirNivel();
            }
        };
#endif
    }
}

[System.Serializable]
public class AnillaData
{
    [Tooltip("Gira anillas para que los huecos de la torre no estén alineados")]
    [Range(0f, 360f)]
    public float baseRotationY = 0f;

    [Tooltip("Añade las piezas (Mitades o Cuartos) para construir este piso")]
    public List<PiezaData> pieces = new List<PiezaData>();

   
}
[System.Serializable]
public class PiezaData
{
    public PieceShape tipoFormaPieza;
    public PieceType tipoPieza;

    [Tooltip("Ángulo local para encajar la pieza. Usa múltiplos de 90 (0, 90, 180, 270)")]
    public float localRotationY = 0f;
}
