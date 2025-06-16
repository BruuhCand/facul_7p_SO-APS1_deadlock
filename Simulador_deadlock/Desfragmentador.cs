using System;
using System.Threading;

namespace Simulador_deadlock
{
    public class Desfragmentador
    {
        private double[] V = new double[1_000_000]; // cria um vetor com 1 milhão de posições
        private readonly object lockObj = new object();
        private bool desfragmentando = true;

        public void Iniciar()
        {
            // 1 inicia o vetor com dados fragmentados (aleatórios)
            InicializarVetorFragmentado();

            // 2 Inicia threads para desfragmentação paralela
            int numThreads = Environment.ProcessorCount;
            Console.WriteLine($"Iniciando desfragmentação com {numThreads} threads...");

            for (int i = 0; i < numThreads; i++)
            {
                new Thread(() =>
                {
                    Random rand = new Random();
                    while (desfragmentando)
                    {
                        // Simula desfragmentação movendo dados para posições contíguas
                        for (int j = 0; j < 100_000; j++)
                        {
                            int indexOrigem = rand.Next(0, V.Length);
                            if (V[indexOrigem] != 0)
                            {
                                lock (lockObj) // Garante thread-safety
                                {
                                    // Encontra próxima posição vazia no início do vetor
                                    int indexDestino = Array.IndexOf(V, 0);
                                    if (indexDestino != -1)
                                    {
                                        V[indexDestino] = Math.Sqrt(V[indexOrigem]) * Math.Sin(V[indexOrigem]); // Cálculo CPU-bound
                                        V[indexOrigem] = 0;
                                    }
                                }
                            }
                        }
                    }
                })
                { IsBackground = true }.Start();
            }

            //  caso trave o PC vai parar de rodar após 30 segundos
            new Timer(_ => desfragmentando = false, null, 30000, Timeout.Infinite);
        }

        private void InicializarVetorFragmentado()
        {
            Random rand = new Random();
            for (int i = 0; i < 100_000; i++) // vai preencher só 10% do vetor (fragmentado)
            {
                V[rand.Next(0, V.Length)] = rand.NextDouble() * 1000;
            }
        }
    }
}