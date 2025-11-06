using System.Collections;
using UnityEngine;

public class Giro2 : MonoBehaviour
{
    // Asigna tus 3 cilindros con ReelSpinner
    public ReelSpinner r1; // Cylinder.001
    public ReelSpinner r2; // Cylinder.002
    public ReelSpinner r3; // Cylinder.003

    [Header("Timing")]
    public float delayBetweenReels = 0.15f;

    // ORDEN del reel (16 casillas)
    // Mapeo: 0=7, 1=Campana, 2=Cereza, 3=BAR
    public int[] reelOrder = new int[]
    {
        1,2,3, 1,0,2,3, 1,2,3, 1,2,3, 1,2,3
    };

    [Header("Calibración (shift por reel)")]
    public int shiftR1 = 0;
    public int shiftR2 = 0;
    public int shiftR3 = 0;

    [Header("Configuración de símbolos y probabilidades")]
    [Tooltip("Cantidad total de símbolos posibles (0..N-1). Ej: 4 = {0,1,2,3}")]
    public int totalSymbols = 4;

    [Tooltip("Pesos de aparición para cada símbolo (la suma puede ser cualquier número)")]
    public int[] symbolWeights = new int[4] { 1, 3, 3, 1 };
    // Ejemplo: [7, Campana, Cereza, BAR] => 7 raro, campana y cereza comunes, BAR raro.
    
    public SlotPrizeManager prizeManager; // arrástralo desde el inspector
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int[] generated = GenerateWeightedArray();
            PlayRequested(generated);
        }
        //Depuración: forzar resultados específicos
        if (Input.GetKeyDown(KeyCode.Alpha1)) // Fuerza triple 0 (7)
            ForceResult(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) // Fuerza triple 1 (Campana)
            ForceResult(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) // Fuerza triple 2 (Cereza)
            ForceResult(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) // Fuerza triple 3 (BAR)
            ForceResult(3);
    }
    // -----------------------------------------------------------
    // 🔹 Método auxiliar que fuerza un triple símbolo específico
    // -----------------------------------------------------------
    void ForceResult(int symbol)
    {
        int[] forced = new int[3] { symbol, symbol, symbol };
        Debug.Log($"🎯 Forzando resultado: [{symbol},{symbol},{symbol}]");
        PlayRequested(forced);
    }
    // Genera un array aleatorio de 3 símbolos según los pesos
    public int[] GenerateWeightedArray()
    {
        int[] arr = new int[3];
        for (int i = 0; i < arr.Length; i++)
            arr[i] = WeightedRandom(symbolWeights);

        Debug.Log("Resultado generado: [" + string.Join(", ", arr) + "]");
        return arr;
    }

    // Retorna un índice aleatorio basado en los pesos de probabilidad
    int WeightedRandom(int[] weights)
    {
        int total = 0;
        foreach (var w in weights) total += Mathf.Max(0, w); // evita negativos
        if (total == 0) return 0; // fallback

        int r = Random.Range(0, total);
        for (int i = 0; i < weights.Length; i++)
        {
            if (r < weights[i])
                return i;
            r -= weights[i];
        }
        return 0;
    }

    public void PlayRequested(int[] requestSymbols)
    {
        if (requestSymbols == null || requestSymbols.Length != 3)
        {
            Debug.LogError("PlayRequested necesita un array de 3 símbolos [0..3].");
            return;
        }
        StartCoroutine(SpinAllSequential(requestSymbols));
    }

    IEnumerator SpinAllSequential(int[] req)
    {
        // Reel 1
        int idx1 = FindNextIndexForSymbol(r1, req[0], shiftR1);
        r1.SpinToIndex(idx1);
        yield return new WaitForSeconds(delayBetweenReels);

        // Reel 2
        int idx2 = FindNextIndexForSymbol(r2, req[1], shiftR2);
        r2.SpinToIndex(idx2);
        yield return new WaitForSeconds(delayBetweenReels);

        // Reel 3
        int idx3 = FindNextIndexForSymbol(r3, req[2], shiftR3);
        r3.SpinToIndex(idx3);

        // Esperar hasta que los 3 rodillos terminen
        float timeout = Mathf.Max(r1.spinDuration, r2.spinDuration, r3.spinDuration) + 2f;
        float elapsed = 0f;


        if (elapsed >= timeout)
            Debug.LogWarning("Timeout esperando rodillos.");

        // ✅ Ahora los rodillos terminaron
        if (prizeManager != null)
        {
            Debug.Log("Evaluando resultado final: [" + string.Join(", ", req) + "]");
            prizeManager.EvaluarResultado(req);
        }
        else
        {
            Debug.LogWarning("No hay PrizeManager asignado, no se evalúa el resultado.");
        }
    }


    int FindNextIndexForSymbol(ReelSpinner reel, int symbol, int shift)
    {
        int items = Mathf.Max(1, reel.items);
        int from = reel.GetApproxIndex();
        shift = ((shift % items) + items) % items;

        for (int step = 1; step <= items; step++)
        {
            int i = (from + step) % items;
            if (reelOrder[(i + shift) % items] == symbol)
                return i;
        }

        for (int i = 0; i < items; i++)
            if (reelOrder[(i + shift) % items] == symbol)
                return i;

        return from;
    }
}
