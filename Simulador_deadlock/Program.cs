using Simulador_deadlock;
using System.Threading;

namespace SimuladorDeadlock
{
    class Program
    {
        static List<Processo> processos = new();
        static Dictionary<string, object> recursos = new();
        static object sincronizador = new();
        static CancellationTokenSource cancelador = new();

        static void Main()
        {
            
            recursos["R1"] = new object();
            recursos["R2"] = new object();
            recursos["R3"] = new object();

            
            processos.Add(new Processo("P1", new[] { "R1", "R2" }));
            processos.Add(new Processo("P2", new[] { "R2", "R3" }));
            processos.Add(new Processo("P3", new[] { "R3", "R1" }));

            
            foreach (var processo in processos)
            {
                var thread = new Thread(() => ExecutarProcesso(processo, cancelador.Token));
                processo.Thread = thread;
                thread.Start();
            }

            
            while (true)
            {
                Thread.Sleep(5000);
                VerificarDeadlock();
            }
        }

        static void ExecutarProcesso(Processo processo, CancellationToken token)
        {
            try
            {
                foreach (var recurso in processo.RecursosNecessarios)
                {
                    if (VerificarCancelamento(processo, token.IsCancellationRequested)) return;

                    lock (sincronizador)
                    {
                        processo.Estado = $"Esperando pelo recurso {recurso}";
                        processo.RecursoAtual = recurso;
                        Console.WriteLine($"{processo.Nome} está aguardando o recurso {recurso}...");
                    }

                    Thread.Sleep(3000); 

                    
                    lock (recursos[recurso])
                    {
                        if (VerificarCancelamento(processo, token.IsCancellationRequested)) return;

                        lock (sincronizador)
                        {
                            processo.Estado = $"Utilizando o recurso {recurso}";
                            Console.WriteLine($"{processo.Nome} está utilizando o recurso {recurso}");
                        }

                        Thread.Sleep(5000);
                    }
                }

                lock (sincronizador)
                {
                    processo.Estado = "Finalizado";
                    processo.Finalizado = true;
                    Console.WriteLine($"{processo.Nome} finalizou.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro durante a execução de {processo.Nome}: {ex.Message}");
            }
        }

        static bool VerificarCancelamento(Processo processo, bool foiCancelado)
        {
            if (foiCancelado)
            {
                lock (sincronizador)
                {
                    processo.Estado = "Abortado";
                    Console.WriteLine($"{processo.Nome} foi interrompido");
                    return true;
                }
            }

            return false;
        }

        static void VerificarDeadlock()
        {
            lock (sincronizador)
            {
                var emEspera = processos
                    .Where(p => !p.Finalizado && p.Estado.StartsWith("Esperando"))
                    .ToList();

                if (emEspera.Count >= 3 && ExisteCicloDeEspera(emEspera))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nDeadlock detectado!");
                    Console.ResetColor();

                    cancelador.Cancel();

                    foreach (var p in emEspera)
                    {
                        Console.WriteLine($"Cancelando {p.Nome}, pois estava em: {p.Estado}");
                    }

                    Console.WriteLine("A execução será encerrada para os processos em conflito.\n");
                }
            }
        }

        static bool ExisteCicloDeEspera(List<Processo> processosTravados)
        {
            var recursosAguardados = processosTravados.Select(p => p.RecursoAtual).Distinct();
            return recursosAguardados.Count() <= processosTravados.Count;
        }
    }
}
