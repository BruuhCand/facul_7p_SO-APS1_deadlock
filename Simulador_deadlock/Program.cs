using System.Reflection.Metadata.Ecma335;
using System.Threading;

namespace Simulador_deadlock
{
    class Program
    {
        static List<Processo> processos = new();
        static Dictionary<string, object> recursos = new();
        static object lockObj = new();
        static CancellationTokenSource cancellationTokenSource = new();

        static void Main()
        {
            recursos["R1"] = new object();
            recursos["R2"] = new object();
            recursos["R3"] = new object();

            processos.Add(new Processo("P1", new[] { "R1", "R2" }));
            processos.Add(new Processo("P2", new[] { "R2", "R3" }));
            processos.Add(new Processo("P3", new[] { "R3", "R1" }));

            foreach (var p in processos)
            {
                var thread = new Thread(() => ExecutarProcesso(p, cancellationTokenSource.Token));
                p.Thread = thread;
                thread.Start();
            }

            while (true)
            {
                Thread.Sleep(5000);
                DetectarETratarDeadlock();
            }
        }

        static void ExecutarProcesso(Processo p, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var recurso in p.RecursosNecessarios)
                {
                    bool cancelar = false;

                    if (CancelamentoRequerido(p, cancellationToken.IsCancellationRequested))
                        return;

                    lock (lockObj)
                    {
                        p.Estado = $"Esperando {recurso}";
                        p.RecursoAtual = recurso;
                        Console.WriteLine($"{p.Nome} está esperando recurso {recurso}");
                    }

                    Thread.Sleep(3000); 

                    
                    lock (recursos[recurso])
                    {

                        if (CancelamentoRequerido(p, cancellationToken.IsCancellationRequested))
                            return;

                        lock (lockObj)
                        {
                            p.Estado = $"Usando {recurso}";
                            Console.WriteLine($"{p.Nome} está usando recurso {recurso}");
                        }

                        Thread.Sleep(5000);


                        if (CancelamentoRequerido(p, cancellationToken.IsCancellationRequested))
                            return;
                    }
                }

                lock (lockObj)
                {
                    p.Estado = "Finalizado";
                    p.Finalizado = true;
                    Console.WriteLine($"{p.Nome} finalizou.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no processo {p.Nome}: {ex.Message}");
            }
        }

        static bool CancelamentoRequerido(Processo p, bool cancelamento)
        {
            if (cancelamento)
            {
                lock (lockObj)
                {
                    p.Estado = "Abortado";
                    Console.WriteLine($"{p.Nome} abortado após usar {p.RecursoAtual}.");
                    return true;
                }
            }

            return false;
        }

        static void DetectarETratarDeadlock()
        {
            lock (lockObj)
            {
                var bloqueados = processos
                    .Where(p => !p.Finalizado && p.Estado.StartsWith("Esperando"))
                    .ToList();

                if (bloqueados.Count >= 3 && IsDeadlock(bloqueados))
                {
                    Console.WriteLine("\nDEADLOCK DETECTADO!");

                    cancellationTokenSource.Cancel(); 

                    foreach (var p in bloqueados)
                    {
                        Console.WriteLine($"Matando processo {p.Nome} que estava em: {p.Estado}");
                    }

                    Console.WriteLine();
                }
            }
        }

        static bool IsDeadlock(List<Processo> bloqueados)
        {
            var recursosBloqueados = bloqueados.Select(p => p.RecursoAtual).Distinct();
            return recursosBloqueados.Count() <= bloqueados.Count;
        }
    }
}
