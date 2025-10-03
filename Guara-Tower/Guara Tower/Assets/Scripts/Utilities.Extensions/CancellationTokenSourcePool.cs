using System.Collections.Generic;
using System.Threading;

public static class CancellationTokenSourcePool
{
    private static readonly Stack<CancellationTokenSource> pool = new();

    /// <summary>Retorna um CTS do pool ou cria um novo.</summary>
    public static CancellationTokenSource Rent()
    {
        if (pool.Count > 0)
        {
            var cts = pool.Pop();

            // O token foi cancelado antes? Não dá pra "resetar", tem que descartar e criar novo
            if (cts.IsCancellationRequested)
            {
                cts.Dispose();
                return new CancellationTokenSource();
            }

            return cts;
        }

        return new CancellationTokenSource();
    }

    /// <summary>Devolve um CTS para o pool.</summary>
    public static void Return(this CancellationTokenSource cts)
    {
        if (cts == null) return;

        if (cts.IsCancellationRequested)
        {
            cts.Dispose(); // nunca reusa um token já cancelado
            return;
        }

        pool.Push(cts);
    }
    
    public static void CancellHelper(this CancellationTokenSource cts)
    {
        if (cts == null) return;

        cts.Cancel();
        cts.Dispose();

    }

    /// <summary>Limpa todos os CTS do pool (em caso de reinício ou unload).</summary>
    public static void Clear()
    {
        while (pool.Count > 0)
            pool.Pop().Dispose();
    }
}