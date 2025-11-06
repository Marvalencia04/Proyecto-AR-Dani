using UnityEngine;
using TMPro;

/// <summary>
/// Controla las monedas del jugador, su gasto y la UI.
/// </summary>
public class SlotCurrencyManager : MonoBehaviour
{
    [Header("Configuración de monedas")]
    [Tooltip("Cantidad inicial de monedas del jugador.")]
    public int monedasIniciales = 50;

    [Tooltip("Cantidad máxima de monedas que puede tener el jugador.")]
    public int monedasMaximas = 50;

    [Tooltip("Costo en monedas por cada tirada (Play).")]
    public int costePorJugada = 5;

    [Header("Referencias UI")]
    [Tooltip("Texto que muestra las monedas actuales y máximas (formato XX/YY).")]
    public TextMeshProUGUI monedasTexto;

    private int monedasActuales;

    private void Start()
    {
        monedasActuales = monedasIniciales;
        ActualizarUI();
    }

    /// <summary>
    /// Intenta restar el costo por jugar. Devuelve true si hay saldo suficiente.
    /// </summary>
    public bool RestarCostoJugada()
    {
        if (monedasActuales >= costePorJugada)
        {
            monedasActuales -= costePorJugada;
            ActualizarUI();
            return true;
        }

        Debug.LogWarning("❌ No hay suficientes monedas para jugar.");
        return false;
    }

    /// <summary>
    /// Añade una cantidad de monedas al ganar.
    /// </summary>
    public void AñadirPremio(int cantidad)
    {
        monedasActuales = Mathf.Min(monedasActuales + cantidad, monedasMaximas);
        ActualizarUI();
    }

    /// <summary>
    /// Actualiza el texto del contador.
    /// </summary>
    private void ActualizarUI()
    {
        if (monedasTexto != null)
            monedasTexto.text = $"{monedasActuales} / {monedasMaximas}";
    }

    /// <summary>
    /// Devuelve la cantidad actual de monedas (para otros scripts).
    /// </summary>
    public int ObtenerMonedas() => monedasActuales;
}
