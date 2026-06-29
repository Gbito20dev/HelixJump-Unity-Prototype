using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    // El patrón Singleton: permite acceso global a este script
    public static GameManager instancia;

    [Header("Datos de la Partida")]
    public int puntuacionActual = 0;
    public int multiplicadorActual = 1;

    [Header("Interfaz (UI)")]
    [Tooltip("Arrastra aquí tu panel negro de Game Over")]
    public GameObject panelGameOver;
    [Tooltip("Arrastra aquí tu panel negro de Pausa")]
    public GameObject panelPausa;
    [Tooltip("Arrastra aquí tu panel negro de Win")]
    public GameObject panelWin;
    [Tooltip("Arrastra aquí tu texto de puntuación de la pantalla")]
    public TextMeshProUGUI textoPuntuacion; // Referencia al texto en el Canvas
    public TextMeshProUGUI textoPuntuacion2; // Referencia al texto en el Canvas 

    // --- NUEVO: SISTEMA DE SONIDO ---
    [Header("Efectos de Sonido")]
    [Tooltip("Arrastra aquí el propio componente AudioSource del GameManager")]
    public AudioSource audioFuentePuntuacion;
    public AudioClip sonidoPasarAnilla;
    public AudioClip clipVictoria;

    [Header("Gestión de Niveles")]
    [Tooltip("Arrastra aquí TODOS tus ScriptableObjects de niveles en orden")]
    public LevelData[] listaDeNiveles;
    public LevelGenerator generadorNiveles;
    public TextMeshProUGUI textoIndicadorNivel; // El texto que dirá "NIVEL 1"

    private int indiceNivelActual = 0; // 0 = Nivel 1, 1 = Nivel 2, etc.
    // Controles de estado para evitar bugs
    public bool juegoTerminado = false;
    private bool juegoPausado = false;


    private void Awake()
    {
        // Configuramos el Singleton. Si no hay ninguno, me convierto en el jefe. 
        // Si ya hay otro, me destruyo para evitar duplicados.
        if (instancia == null)
        {
            instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

        // Por seguridad, ocultamos los paneles si se te quedaron encendidos en el editor
        if (panelGameOver != null) panelGameOver.SetActive(false);
        if (panelPausa != null) panelPausa.SetActive(false); 

        // Nos aseguramos de que el tiempo corre normal al empezar
        Time.timeScale = 1f;

        // Resetamos el combo y puntos al iniciar la escena
        multiplicadorActual = 1;
        puntuacionActual = 0;

        // Por seguridad, ocultamos el panel si se te quedó encendido en el editor
        if (panelGameOver != null) panelGameOver.SetActive(false);

        // Actualizamos el texto visualmente a "0" al arrancar
        ActualizarTextoPuntuacion();

        CargarNivelCorrespondiente();
    }
    private void CargarNivelCorrespondiente()
    {
        // 1. Leemos la memoria del móvil/PC. Si no hay nada guardado, devuelve 0 (Nivel 1).
        indiceNivelActual = PlayerPrefs.GetInt("NivelGuardado", 0);

        // 2. Seguridad: Si el jugador ha superado todos los niveles que has creado, lo devolvemos al primero (bucle)
        if (indiceNivelActual >= listaDeNiveles.Length)
        {
            indiceNivelActual = 0;
            PlayerPrefs.SetInt("NivelGuardado", indiceNivelActual);
        }

        // 3. Actualizamos el texto de la UI (Le sumamos 1 porque el índice 0 es el Nivel 1)
        if (textoIndicadorNivel != null)
        {
            textoIndicadorNivel.text = "NIVEL " + (indiceNivelActual + 1).ToString();
        }

        // 4. Le pasamos el LevelData correcto al generador y le decimos que construya
        if (generadorNiveles != null && listaDeNiveles.Length > 0)
        {
            generadorNiveles.nivelActual = listaDeNiveles[indiceNivelActual];
            generadorNiveles.ConstruirNivel();
        }
    }

    // --- LÓGICA DE PUNTUACIÓN ---
    public void SumarPuntos(int cantidad)
    {
        // Si el juego ya ha terminado (por ganar o morir), ignoramos los puntos
        if (juegoTerminado) return;
        // Multiplicamos los puntos base (2) por la racha actual
        int puntosASumar = cantidad * multiplicadorActual;
        puntuacionActual += puntosASumar;

        // ---  (Tono más agudo) ---
        if (audioFuentePuntuacion != null && sonidoPasarAnilla != null)
        {
            // Subimos el tono 0.15 por cada multiplicador. 
            // Usamos Mathf.Min para ponerle un tope de 3.0f, así no sonará estridente y roto.
            audioFuentePuntuacion.pitch = Mathf.Min(1f + (multiplicadorActual * 0.15f), 3f);

            // Reproducimos el sonido de la anilla
            audioFuentePuntuacion.PlayOneShot(sonidoPasarAnilla);
        }

        multiplicadorActual++;
        Debug.Log("<color=yellow>¡Puntos sumados! Total: </color>" + puntuacionActual);
        // Llamamos a la función que cambia el texto visual
        ActualizarTextoPuntuacion();
    }

    // Funcion para actualizar el texto in gmae pasando de numeo a texto
    private void ActualizarTextoPuntuacion()
    {
        if (textoPuntuacion != null)
        {
            textoPuntuacion.text = puntuacionActual.ToString(); // Convierte el número a texto
            textoPuntuacion2.text = puntuacionActual.ToString(); // Convierte el número a texto
        }
    }

    // Funcion para que la pelota resetee la racha al tocar el suelo
    public void ResetearMultiplicador()
    {
        multiplicadorActual = 1;
    }

    // --- LÓGICA DE DERROTA ---
    public void GameOver()
    {
        if (juegoTerminado) return;

        juegoTerminado = true;
        Debug.Log("<color=red>LÓGICA DE GAME OVER ACTIVADA</color>");

        // 1. Congelamos el tiempo (Bloquea el giro y la caída de la pelota)
        Time.timeScale = 0f;

        // le damos un tembleque a la camara para sentir que hemos perdido mejor
        ControladorCamara.instancia.ActivarShake();

        // 2. llamamos a la funcion que muestra el panel de derrota
        MostrarPanelDerrota();
    }

    // FUNCIÓN Para mostar el panel derrota 
    private void MostrarPanelDerrota()
    {
        if (panelGameOver != null)
        {
            // Encendemos el objeto visualmente
            panelGameOver.SetActive(true);

        }
        else
        {
            Debug.LogWarning("¡Aviso! El GameManager intentó mostrar el Game Over, pero no tiene un panel asignado.");
        }
    }


    // --- LÓGICA DE VICTORIA ---
    public void NivelCompletado()
    {
        if (juegoTerminado) return;

        juegoTerminado = true;
        Debug.Log("<color=green>¡NIVEL COMPLETADO!</color>");

        if (audioFuentePuntuacion != null && clipVictoria != null)
        {
            // Reseteamos el pitch a 1 por si venía agudo de una racha de anillas
            audioFuentePuntuacion.pitch = 1f;
            //Lanzamos el sonido de victoria
            audioFuentePuntuacion.PlayOneShot(clipVictoria);
        }
        Time.timeScale = 0f;
        // llamamos a la funcion de mostrar el panel
        MostrarPanelVictoria();
    }
    //funcion para mostar el panel de victoria
    private void MostrarPanelVictoria()
    {
        if (panelGameOver != null)
        {
            // Encendemos el objeto visualmente
            panelWin.SetActive(true);

        }
        else
        {
            Debug.LogWarning("¡Aviso! El GameManager intentó mostrar el Game Over, pero no tiene un panel asignado.");
        }
    }

    // --- LÓGICA DE PAUSA --- 
    public void TogglePausa()
    {
        if (juegoTerminado) return; // No puedes pausar si ya estás muerto o has ganado

        juegoPausado = !juegoPausado; // Invertimos el estado

        if (juegoPausado)
        {
            Time.timeScale = 0f; // Congelamos el tiempo
            Debug.Log("Juego Pausado");

            // Encendemos el panel de pausa
            if (panelPausa != null) panelPausa.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f; // El tiempo vuelve a fluir
            Debug.Log("Juego Reanudado");

            // Apagamos el panel de pausa
            if (panelPausa != null) panelPausa.SetActive(false);
        }
    }

    // funcion para activar el panel de pausa
    private void MostrarPanelPausa()
    {
        if (panelGameOver != null)
        {
            // Encendemos el objeto visualmente
            panelPausa.SetActive(true);

      
        }
        else
        {
            Debug.LogWarning("¡Aviso! El GameManager intentó mostrar el Game Over, pero no tiene un panel asignado.");
        }
    }

    // --- LÓGICA DE REINICIO ---
    // Esta es la función que conectaremos al botón de "Reintentar"
    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // --- LOGICA DE SIGUIENTE NIVEL ---
    public void SiguienteNivel()
    {
        // Sumamos 1 al índice
        indiceNivelActual++;

        // Lo guardamos en la memoria del dispositivo
        PlayerPrefs.SetInt("NivelGuardado", indiceNivelActual);

        // Recargamos la escena. Al cargar, el Start() leerá el nuevo número y construirá el nuevo nivel
        ReiniciarNivel();
    }

    //--- LOGICA DE NIVEL ANTERIOR--
    public void NivelAnterior()
    {
        // Restamos 1, pero evitamos que baje de 0 para que no dé error
        indiceNivelActual--;
        if (indiceNivelActual < 0) indiceNivelActual = 0;

        PlayerPrefs.SetInt("NivelGuardado", indiceNivelActual);
        ReiniciarNivel();
    }
}
