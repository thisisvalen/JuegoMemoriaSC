using UnityEngine;
using UnityEngine.UI;

public class CartaController : MonoBehaviour
{
    [Header("Componentes Visuales")]
    [SerializeField] private Image tarjetaImage; // La imagen de la carta en sí
    [SerializeField] private Image iconoImage;   // El hijo que tiene la fruta/carrito

    [Header("Sprites")]
    private Sprite spriteDorso;  // Imagen verde con logo
    private Sprite spriteFrente; // Imagen blanca vacía

    private bool estaVolteada = false;
    private int idPareja; // Identificador para saber qué fruta es (0 para sandía, 1 para banano, etc.)

    // Este método lo llamaremos desde otro script para configurar la carta al iniciar
    public void ConfigurarCarta(int id, Sprite frente, Sprite dorso, Sprite icono)
    {
        idPareja = id;
        spriteFrente = frente;
        spriteDorso = dorso;
        
        iconoImage.sprite = icono;
        
        // Al empezar, mostramos el dorso y ocultamos el icono
        MostrarDorso();
    }

    // Función que se ejecuta cuando el jugador hace clic en la carta
    public void AlHacerClic()
    {
        if (estaVolteada) return;

        // Le avisa directamente al GameManager actual
        Object.FindFirstObjectByType<GameManager>().CartaSeleccionada(this);
    }

    public void Voltear()
    {
        estaVolteada = true;
        tarjetaImage.sprite = spriteFrente;
        iconoImage.gameObject.SetActive(true);
    }

    public void MostrarDorso()
    {
        estaVolteada = false;
        tarjetaImage.sprite = spriteDorso;
        iconoImage.gameObject.SetActive(false); // Ocultamos la fruta/carrito
    }

    // Propiedad pública para que el juego pueda leer el ID de la carta
    public int IdPareja => idPareja;
}