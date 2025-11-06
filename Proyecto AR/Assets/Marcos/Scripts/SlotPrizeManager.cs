using UnityEngine;
using System.Collections;

public class SlotPrizeManager : MonoBehaviour
{
    [Header("Referencia al controlador principal")]
    public Giro slotMachine;

    [Header("Premios configurables")]
    public int premioTresIguales = 100;
    public int premioTriple7 = 500;
    public string mensajeSinPremio = "Sigue intentando...";

    [Header("Gestión de sonidos (opcional)")]
    public SlotSoundManager soundManager;

    [Header("Efectos visuales (opcional)")]
    public ParticleSystem confettiFX;
    public ParticleSystem coinsFX;

    [Tooltip("Duración antes de comenzar a apagar los efectos.")]
    public float stopEffectsAfter = 5f;

    private Coroutine activeCoroutine;

    private void Start()
    {
        if (slotMachine == null)
            Debug.LogWarning("SlotPrizeManager: no hay referencia al objeto Giro.");
    }

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

        if (a == 0 && b == 0 && c == 0)
        {
            Debug.Log($"🎉 ¡Triple 7! Premio: {premioTriple7}");
            OnWin(premioTriple7, "¡Triple 7!", true);
            return;
        }

        if (a == b && b == c)
        {
            Debug.Log($"🎉 Tres iguales ({a}) → Premio: {premioTresIguales}");
            OnWin(premioTresIguales, "Tres iguales", false);
            return;
        }

        Debug.Log(mensajeSinPremio);
        OnLose();
    }

    protected virtual void OnWin(int cantidad, string tipo, bool isTriple7)
    {
        if (soundManager != null)
            soundManager.OnWin();

        // Detener cualquier fade-out anterior si existía
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        // ✅ Aseguramos que las emisiones estén habilitadas antes de reproducir
        if (confettiFX != null)
        {
            var em = confettiFX.emission;
            em.enabled = true;
            confettiFX.Play(true);
            Debug.Log("Playing confetti effect for win.");
        }

        if (isTriple7 && coinsFX != null)
        {
            var em = coinsFX.emission;
            em.enabled = true;
            coinsFX.Play(true);
            Debug.Log("Playing coins effect for triple 7 win.");
        }

        if (stopEffectsAfter > 0)
            activeCoroutine = StartCoroutine(FadeOutEffects());
    }

    protected virtual void OnLose()
    {
        if (soundManager != null)
            soundManager.OnLose();
    }

    private IEnumerator FadeOutEffects()
    {
        yield return new WaitForSeconds(stopEffectsAfter);

        // 🔹 Parar la emisión suavemente, dejar que las partículas mueran
        if (confettiFX != null)
        {
            var em = confettiFX.emission;
            em.enabled = false;
            Debug.Log("Stopping confetti emission.");
        }

        if (coinsFX != null)
        {
            var em = coinsFX.emission;
            em.enabled = false;
            Debug.Log("Stopping coins emission.");
        }

        // 🔹 Espera a que las partículas que están en pantalla desaparezcan naturalmente
        yield return new WaitForSeconds(2f);

        // 🔹 Limpiar partículas viejas
        if (confettiFX != null)
            confettiFX.Clear();

        if (coinsFX != null)
            coinsFX.Clear();

        activeCoroutine = null;
    }
}
