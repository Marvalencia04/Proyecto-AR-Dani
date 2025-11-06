using UnityEngine;

/// <summary>
/// Evalúa combinaciones ganadoras en la tragaperras.
/// Se conecta con el script TestSpin para recibir el resultado generado.
/// </summary>
public class SlotPrizeManager : MonoBehaviour
{
    [Header("Referencia al controlador principal")]
    public Giro slotMachine; // arrastra aquí tu objeto con TestSpin

    [Header("Premios configurables")]
    [Tooltip("Premio por 3 símbolos iguales.")]
    public int premioTresIguales = 100;

    [Tooltip("Premio especial si los tres son 7.")]
    public int premioTriple7 = 500;

    [Tooltip("Mensaje si no hay premio.")]
    public string mensajeSinPremio = "Sigue intentando...";

    [Header("Referencia al gestor de sonidos (opcional)")]
    public SlotSoundManager soundManager;

    private void Start()
    {
        // Puedes suscribirte al evento o simplemente llamar desde TestSpin
        if (slotMachine == null)
            Debug.LogWarning("SlotPrizeManager: no hay referencia al TestSpin.");
    }

    /// <summary>
    /// Evalúa el resultado recibido desde TestSpin.
    /// </summary>
    public void EvaluarResultado(int[] resultado)
    {
        if (resultado == null || resultado.Length != 3)
        {
            Debug.LogError("Resultado inválido recibido por SlotPrizeManager.");
            return;
        }

        int a = resultado[0];
        int b = resultado[1];
        int c = resultado[2];

        // CASO 1: triple 7
        if (a == 0 && b == 0 && c == 0)
        {
            Debug.Log($"🎉 ¡Triple 7! Premio: {premioTriple7}");
            OnWin(premioTriple7, "¡Triple 7!");
            return;
        }

        // CASO 2: tres iguales
        if (a == b && b == c)
        {
            Debug.Log($"🎉 Tres iguales ({a}) → Premio: {premioTresIguales}");
            OnWin(premioTresIguales, "Tres iguales");
            return;
        }


        // CASO 3: sin premio
        Debug.Log(mensajeSinPremio);
        OnLose();
    }

    // 🔔 Método que puedes sobreescribir o conectar a UI/SFX
    protected virtual void OnWin(int cantidad, string tipo)
    {
        if (soundManager != null)
            soundManager.OnWin();
    }

    protected virtual void OnLose()
    {
        if (soundManager != null)
            soundManager.OnLose();
    }
}
