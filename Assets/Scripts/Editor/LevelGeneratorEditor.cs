using UnityEngine;
using UnityEditor; // IMPORTANTE: Esta librería nos permite modificar el Inspector

//  Este script va a modificar cómo se ve el LevelGenerator en el Inspector
[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{
    // Esta función sobreescribe el Inspector por defecto
    public override void OnInspectorGUI()
    {
        //   Le decimos que dibuje todas tus variables públicas normales (los huecos de los prefabs, etc)
        DrawDefaultInspector();

        //   Obtenemos una referencia a tu script LevelGenerator
        LevelGenerator generador = (LevelGenerator)target;

        //   Dejamos un poco de espacio visual para que quede bonito
        GUILayout.Space(15);

        //    CREAMOS EL BOTÓN
        // Le damos un nombre y una altura de 40 píxeles para que destaque
        if (GUILayout.Button(" CONSTRUIR NIVEL", GUILayout.Height(40)))
        {
            // Si alguien pulsa el botón, llamamos a tu función de construir
            generador.ConstruirNivel();
        }

        //   Botón secundario para limpiar
        if (GUILayout.Button(" LIMPIAR NIVEL"))
        {
            generador.LimpiarNivel();
        }
    }
}