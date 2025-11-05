using System.Collections;
using UnityEngine;

/// <summary>
/// Controla el giro de un carrete de tragaperras.
/// Gira hasta un índice exacto, ajustado en casillas.
/// Pensado para 16 items (22.5º por casilla) pero configurable.
/// </summary>
public class ReelSpinner : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    // Eje de rotación del cilindro (normalmente Z)
    public Axis rotateAxis = Axis.Z;

    // Número de casillas (símbolos) que tiene el cilindro
    public int items = 16;

    // Ángulo que indica dónde está el 0 visual real del cilindro en Unity
    public float baseOffsetDeg = 176f;

    // Desplazamiento lógico de casillas para alinear visualmente el símbolo correcto
    public int frontOffsetSlots = 0;

    // Si el cilindro gira al revés marcamos esto en true
    public bool invertDirection = false;

    // Cuántas vueltas completas da antes de frenar (solo estético)
    public float spinsBeforeStop = 3f;

    // Duración total del giro
    public float spinDuration = 1.2f;

    // Curva de aceleración / frenado
    public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    float SlotAngle => 360f / Mathf.Max(1, items); // grados que ocupa una casilla
    Coroutine current;

    // Llama a esto para que gire hacia ese índice final
    public void SpinToIndex(int index)
    {
        index = Mod(index, items);
        if (current != null) StopCoroutine(current);
        current = StartCoroutine(SpinToIndexCo(index));
    }

    // Coloca directamente en el índice sin animación (para test)
    public void SetInstantToIndex(int index)
    {
        float target = IndexToAngle(index);
        SetLocalAngleDeg(Normalize360(target));
    }

    // Devuelve el índice aproximado actual en el que está el cilindro
    public int GetApproxIndex()
    {
        float ang = Normalize360(GetLocalAngleDeg() - baseOffsetDeg);

        if (invertDirection)
            ang = Normalize360(-ang);

        int idx = Mathf.RoundToInt(ang / SlotAngle) - frontOffsetSlots;

        return Mod(idx, items);
    }

    IEnumerator SpinToIndexCo(int index)
    {
        float start = GetLocalAngleDeg();
        float target = IndexToAngle(index);

        // calcula cuántos grados debe avanzar para llegar al target hacia adelante
        float totalDelta = spinsBeforeStop * 360f + ShortestPositiveArc(start, target);
        float end = start + totalDelta;

        float t = 0f;
        float dur = Mathf.Max(0.05f, spinDuration);

        while (t < 1f)
        {
            t += Time.deltaTime / dur;
            float k = ease.Evaluate(Mathf.Clamp01(t));
            float ang = Mathf.LerpUnclamped(start, end, k);
            SetLocalAngleDeg(ang);
            yield return null;
        }

        // Asegura que acaba exactamente en la casilla (sin error flotante)
        SetLocalAngleDeg(Normalize360(target));
        current = null;
    }

    // Convierte índice → ángulo final exacto
    float IndexToAngle(int index)
    {
        int logical = invertDirection ? -index : index;
        float deg = baseOffsetDeg + (logical + frontOffsetSlots) * SlotAngle;
        return Normalize360(deg);
    }

    // ==============================================================
    // Utilidades internas
    // ==============================================================

    float GetLocalAngleDeg()
    {
        var e = transform.localEulerAngles;
        return rotateAxis switch
        {
            Axis.X => e.x,
            Axis.Y => e.y,
            _ => e.z
        };
    }

    void SetLocalAngleDeg(float deg)
    {
        var e = transform.localEulerAngles;
        switch (rotateAxis)
        {
            case Axis.X: e.x = deg; break;
            case Axis.Y: e.y = deg; break;
            default: e.z = deg; break;
        }
        transform.localEulerAngles = e;
    }

    static float Normalize360(float a) => Mathf.Repeat(a, 360f);

    static float Normalize180(float a)
    {
        a = Mathf.Repeat(a + 180f, 360f) - 180f;
        return a;
    }

    // devuelve el avance mínimo positivo para ir desde from hacia to sin retroceder
    static float ShortestPositiveArc(float fromDeg, float toDeg)
    {
        float delta = Normalize180(toDeg - fromDeg);
        if (delta < 0) delta += 360f;
        return delta;
    }

    // módulo positivo
    static int Mod(int a, int m) => (a % m + m) % m;
}
