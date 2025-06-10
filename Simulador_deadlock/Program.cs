using Simulador_deadlock;
using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace SimuladorDeadlock
{
    class Program
    {
        static List<Processo> processos = new();
       // static Dictionary<string, object> recursos = new();
        static object sincronizador = new();
        //static bool cancelador = false;
        static List<Recurso> recursos = new();
        static List<Processo> deadlock = new();
        static bool deadLockDetectado = false;
        static void Main()
        {

            recursos.Add(new Recurso("R1"));
            recursos.Add(new Recurso("R2"));
            recursos.Add(new Recurso("R3"));
            recursos.Add(new Recurso("R4"));
            recursos.Add(new Recurso("R5"));
            recursos.Add(new Recurso("R6"));



            processos.Add(new Processo("P1", new[] { "R6", "R2" }));
            processos.Add(new Processo("P2", new[] { "R3", "R3" }));
            processos.Add(new Processo("P3", new[] { "R1", "R2" }));
            processos.Add(new Processo("P4", new[] { "R2", "R4" }));
            processos.Add(new Processo("P5", new[] { "R4", "R1" }));

            foreach (var processo in processos)
            {
                var thread = new Thread(() => ExecutarProcesso(processo));
                processo.Thread = thread;
                thread.Start();
                Thread.Sleep(100);
            }

            
            while (true)
            {
                Thread.Sleep(2000);
                VerificarDeadlock();
            }
        }

        static void ExecutarProcesso(Processo processo)
        {
            try
            {
                foreach (var r in processo.RecursosNecessarios)
                {
                    if (VerificarCancelamento(processo, processo.cancelar)) return;

                    lock (sincronizador)
                    {
                        processo.Estado = $"Esperando pelo recurso {r}";
                        processo.RecursoEsperado = r;
                        Console.WriteLine($"{processo.Nome} está aguardando o recurso {r}...");
                    }

                    Thread.Sleep(3000);
                    var recursoUsado = recursos.Find(rec => rec.Nome.Equals(processo.RecursoAtual));
                    if (recursoUsado != null) 
                    {
                        recursoUsado.EmUso = false;
                    }
                    var recurso = recursos.Find(rec => rec.Nome.Equals(r));
                    lock (recurso.Lock)
                    {
                        if (VerificarCancelamento(processo, processo.cancelar)) return;

                        lock (sincronizador)
                        {
                            processo.RecursoEsperado = null;
                            recurso.EmUso = true;
                            processo.Estado = $"Utilizando o recurso {r}";
                            processo.RecursoAtual = r;
                            Console.WriteLine($"{processo.Nome} está utilizando o recurso {r}");
                        }

                        Thread.Sleep(10000);
                        
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
            if (foiCancelado && deadlock.Contains(processo))
            {
                lock (sincronizador)
                {
                   
                    processo.Estado = "Abortado";
                    Console.WriteLine($"{processo.Nome} foi interrompido");
                    //processo.Thread.Abort();
                    deadlock.Remove(processo);
                     return true;
                    

                }
            }


            return false;
        }

        static void VerificarDeadlock()
        {
            if (!deadLockDetectado)
            {
                lock (sincronizador)
                {
                    var recursosUsados = recursos.Where(x => x.EmUso == true).ToList();

                    var emEspera = processos
                        .Where(p => !p.Finalizado && p.Estado.StartsWith("Esperando"))
                        .ToList();

                    var processosFiltrados = ExisteCicloDeEspera(ExisteNaLista(emEspera), emEspera);


                    if (processosFiltrados.Any())
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nDeadlock detectado!");
                        Console.ResetColor();

                        //cancelador.Cancel();
                        deadLockDetectado = true;


                        deadlock.Clear();
                        foreach (var i in processosFiltrados)
                        {
                            i.cancelar = true;
                            deadlock.Add(i);
                            Console.WriteLine($"Cancelando {i.Nome}, pois estava em: {i.Estado}");
                        }


                        Console.WriteLine("A execução será encerrada para os processos em conflito.\n");
                    }
                }
            }
          
        }

        static List<String> ExisteNaLista(List<Processo> processosTravados)
        {
            var recursosAguardados = processosTravados.Select(p => p.RecursoEsperado).Distinct().ToList();
            var usando = processos.Select(p => p.RecursoAtual ).Distinct().ToList();

            return recursosAguardados.Intersect(usando).ToList(); 
        }

        static List<Processo> ExisteCicloDeEspera(List<String> recursosAguardando, List<Processo> processos)
        {
            //verifica quais desses recursosaguardando possui na lista dos processos que estão em espera
            var processoEspera = processos
            .Where(p => recursosAguardando.Contains(p.RecursoEsperado) && p.RecursoAtual != p.RecursoEsperado && recursosAguardando.Contains(p.RecursoAtual))
            .ToList();

            return processoEspera;
        }
    }
}
