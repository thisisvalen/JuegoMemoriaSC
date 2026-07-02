using System.Collections.Generic;
using UnityEngine;
using TMPro; // 👈 Importante para controlar los textos
using UnityEngine.SceneManagement; // 👈 Importante para reiniciar la escena

public class GameManager : MonoBehaviour
{
    [Header("Configuración del Tablero")]
    [SerializeField] private Transform tableroTransform;
    [SerializeField] private GameObject cartaPrefab;

    [Header("Sprites del Juego")]
    [SerializeField] private Sprite spriteDorso;
    [SerializeField] private Sprite spriteFrente;
    [SerializeField] private List<Sprite> iconosLista;

    [Header("Interfaz de Usuario (UI)")]
    [SerializeField] private TextMeshProUGUI textoTiempo;
    [SerializeField] private TextMeshProUGUI textoParejas;
    [SerializeField] private GameObject panelFinJuego;
    [SerializeField] private TextMeshProUGUI textoResultado;

    [Header("Ajustes de Tiempo")]
    [SerializeField] private float tiempoRestante = 60f; // 60 segundos de juego

    private List<CartaController> cartasEnTablero = new List<CartaController>();
    private CartaController primeraCartaSeleccionada;
    private CartaController segundaCartaSeleccionada;
    
    private int parejasEncontradas = 0;
    private bool seEstaValidando = false;
    private bool juegoTerminado = false;

    void Start()
    {
        // Al empezar, nos aseguramos de ocultar la pantalla de fin de juego
        panelFinJuego.SetActive(false);
        CrearTablero();
        ActualizarUI();
    }

    void Update()
    {
        // Si el juego ya terminó, no reducimos el tiempo
        if (juegoTerminado) return;

        if (tiempoRestante > 0)
        {
            tiempoRestante -= Time.deltaTime;
            ActualizarUI();
        }
        else
        {
            tiempoRestante = 0;
            ActualizarUI();
            TerminarJuego(false); // Derrota por tiempo
        }
    }

    void CrearTablero()
    {
        List<int> idsCartas = new List<int>();
        for (int i = 0; i < iconosLista.Count; i++)
        {
            idsCartas.Add(i);
            idsCartas.Add(i);
        }

        for (int i = 0; i < idsCartas.Count; i++)
        {
            int temp = idsCartas[i];
            int randomIndex = Random.Range(i, idsCartas.Count);
            idsCartas[i] = idsCartas[randomIndex];
            idsCartas[randomIndex] = temp;
        }

        for (int i = 0; i < idsCartas.Count; i++)
        {
            GameObject nuevaCarta = Instantiate(cartaPrefab, tableroTransform);
            CartaController controller = nuevaCarta.GetComponent<CartaController>();
            
            int id = idsCartas[i];
            Sprite icono = iconosLista[id];

            controller.ConfigurarCarta(id, spriteFrente, spriteDorso, icono);
            cartasEnTablero.Add(controller);

            nuevaCarta.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => controller.AlHacerClic());
        }
    }

    public void CartaSeleccionada(CartaController carta)
    {
        if (seEstaValidando || juegoTerminado) return;

        if (primeraCartaSeleccionada == null)
        {
            primeraCartaSeleccionada = carta;
            primeraCartaSeleccionada.Voltear();
        }
        else if (segundaCartaSeleccionada == null && carta != primeraCartaSeleccionada)
        {
            segundaCartaSeleccionada = carta;
            segundaCartaSeleccionada.Voltear();
            
            StartCoroutine(CheckParejaCoroutine());
        }
    }

    private System.Collections.IEnumerator CheckParejaCoroutine()
    {
        seEstaValidando = true;
        yield return new WaitForSeconds(0.8f); // Reducido un poco para mayor fluidez

        if (primeraCartaSeleccionada.IdPareja == segundaCartaSeleccionada.IdPareja)
        {
            parejasEncontradas++;
            ActualizarUI();

            // Validar condición de victoria (6 parejas en total)
            if (parejasEncontradas >= iconosLista.Count)
            {
                TerminarJuego(true); // Victoria
            }
        }
        else
        {
            primeraCartaSeleccionada.MostrarDorso();
            segundaCartaSeleccionada.MostrarDorso();
        }

        primeraCartaSeleccionada = null;
        segundaCartaSeleccionada = null;
        seEstaValidando = false;
    }

    void ActualizarUI()
    {
        // Mathf.CeilToInt hace que el tiempo se muestre en números enteros redondeados hacia arriba
        textoTiempo.text = "Tiempo: " + Mathf.CeilToInt(tiempoRestante).ToString();
        textoParejas.text = "Parejas: " + parejasEncontradas.ToString() + "/" + iconosLista.Count.ToString();
    }

    void TerminarJuego(bool gano)
    {
        juegoTerminado = true;
        panelFinJuego.SetActive(true);

        if (gano)
        {
            textoResultado.text = "¡VICTORIA!";
            textoResultado.color = Color.green;
        }
        else
        {
            textoResultado.text = "FIN DEL TIEMPO";
            textoResultado.color = Color.red;
        }
    }

    // Función pública para el botón de reiniciar de la UI
    public void ReiniciarJuego()
    {
        // Recarga la escena actual desde cero
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}