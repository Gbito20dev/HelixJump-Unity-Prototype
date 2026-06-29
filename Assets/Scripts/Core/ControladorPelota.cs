using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
public class ControladorPelota : MonoBehaviour
{
    // --- NUEVO: Singleton y Estado ---
    public static ControladorPelota instancia;
    public bool modoDestructor = false;

    [Header("Ajustes de fisicas")]
    public float fuerzaRebote = 7f;

    [Header("Efectos Visuales")]
    public GameObject particulasRebotePrefab;
    [Tooltip("Arrastra aquí el material normal de la pelota")]
    public Material materialNormal;
    [Tooltip("Arrastra aquí el material de fuego/destructor")]
    public Material materialDestructor;

    [Tooltip("Color de la estela normal")]
    public Color colorEstelaNormal = Color.white;
    [Tooltip("Color de la estela al coger el item")]
    public Color colorEstelaDestructor = Color.red;

    // ---SISTEMA DE SONIDO ---
    [Header("Efectos de Sonido")]
    public AudioClip clipRebote;
    public AudioClip clipMuerte;
    public AudioClip clipPowerUp;

    private Rigidbody rb;
    private TrailRenderer estela; // ---Referencia a la estela ---
    private MeshRenderer meshRenderer; // ---Referencia al renderizador 3D ---
    private AudioSource audioPelota; // Referencia a nuestro altavoz

    // ---  Awake para el Singleton ---
    private void Awake()
    {
        if (instancia == null) instancia = this;
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        estela = GetComponent<TrailRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
        // Guardamos la referencia al altavoz de la pelota
        audioPelota = GetComponent<AudioSource>();
    }


    // ---  La corrutina del OBJETO  ---
    public void ActivarModoDestructor()
    {
        // Si coges otro item mientras ya estás en modo destructor, reiniciamos el tiempo
        StopAllCoroutines();
        StartCoroutine(RutinaDestructor());
    }
    //RUTINA PARA CUANDO COGES EL OBJETO
    private IEnumerator RutinaDestructor()
    {
        modoDestructor = true;
        Debug.Log("<color=cyan>¡MODO DESTRUCTOR ACTIVADO!</color>");

        // Se asigna directamente al componente
        if (meshRenderer != null)
        {
            meshRenderer.material = materialDestructor;
        }

        // Cambiamos el color de inicio y fin
        if (estela != null)
        {
            estela.startColor = colorEstelaDestructor;
            estela.endColor = colorEstelaDestructor;
        }

        yield return new WaitForSeconds(1f); // Esperamos 3 segundos

        
        //  DEVOLVER A LA NORMALIDAD
        modoDestructor = false;
        // aQUI SE APAGA LA ESTELA DE LA PELOTA Y SE PONE LA NORMAL
        if (meshRenderer != null)
        {
            meshRenderer.material = materialNormal;
        }

        if (estela != null)
        {
            estela.startColor = colorEstelaNormal;
            estela.endColor = colorEstelaNormal;
        }

        
    }
    private void OnCollisionEnter(Collision collision)
    {
        // --- EL SEGURO ---
        if (GameManager.instancia.juegoTerminado || rb.isKinematic) return;

        // --- DETECCIÓN DE TRAMPAS Y META ---
        if (collision.gameObject.CompareTag("PlataformaMala"))
        {
            // Si tocamos el rojo por casualidad pero somos destructores, lo ignoramos y rebotamos
            if (modoDestructor)
            {
                rb.velocity = new Vector3(0, fuerzaRebote, 0);
                // Si somos destructores, sonamos como un rebote normal
                if (clipRebote != null) audioPelota.PlayOneShot(clipRebote);
                return;
            }
            // --- SONIDO DE MUERTE ---
            if (clipMuerte != null)
            {
                audioPelota.pitch = 1f; // Reseteamos el pitch a normal por si acaso
                audioPelota.PlayOneShot(clipMuerte);
            }

            GameManager.instancia.GameOver();
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            return;
        }

        if (collision.gameObject.CompareTag("Meta"))
        {
            
            GameManager.instancia.NivelCompletado();
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            return;
        }

        // --- LÓGICA DE REBOTE ---
        ContactPoint contacto = collision.GetContact(0);

        // BAJAMOS EL UMBRAL A 0.2f: Si roza el borde del agujero, también rebotará
        if (contacto.normal.y > 0.2f)
        {
            GameManager.instancia.ResetearMultiplicador();

            if (clipRebote != null)
            {
                // Variamos el tono aleatoriamente muy poco para que suene dinámico y no sea repetitivo
                audioPelota.pitch = Random.Range(0.9f, 1.1f);
                audioPelota.PlayOneShot(clipRebote);
            }
            if (particulasRebotePrefab != null)
            {
                Vector3 puntoLigeramenteElevado = contacto.point + (contacto.normal * 0.05f);
                GameObject efecto = Instantiate(particulasRebotePrefab, puntoLigeramenteElevado, Quaternion.LookRotation(contacto.normal));
                Destroy(efecto, 1f);
            }

            // Forzamos X y Z a cero para que la pelota no se desvíe hacia los lados tras tocar un borde
            rb.velocity = new Vector3(0, fuerzaRebote, 0);
        }
    }

    // Si por cualquier bug físico la pelota se queda a velocidad 0 rozando el suelo, la obligamos a saltar
    private void OnCollisionStay(Collision collision)
    {
        if (GameManager.instancia.juegoTerminado || rb.isKinematic) return;

        ContactPoint contacto = collision.GetContact(0);

        // Si está tocando el suelo y su velocidad vertical ha caído casi a 0...
        if (contacto.normal.y > 0.2f && Mathf.Abs(rb.velocity.y) < 0.1f)
        {
            rb.velocity = new Vector3(0, fuerzaRebote, 0);
        }
    }

    // ---RECOGER EL ITEM ---
    private void OnTriggerEnter(Collider other)
    {
        // Comprobamos si lo que acabamos de atravesar tiene la etiqueta del modificador
        if (other.CompareTag("ItemDestructor"))
        {
            // --- SONIDO AL COGER EL POWER UP ---
            if (clipPowerUp != null)
            {
                audioPelota.pitch = 1f; // Pitch normal
                audioPelota.PlayOneShot(clipPowerUp);
            }
            // Activamos la corrutina que ya teníamos preparada
            ActivarModoDestructor();

            // Destruimos el objeto de la escena para que no se pueda coger dos veces
            Destroy(other.gameObject);
        }
    }
}