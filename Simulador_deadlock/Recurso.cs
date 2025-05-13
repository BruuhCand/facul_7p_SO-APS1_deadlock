using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulador_deadlock
{
    public class Recurso
    {
        public string Nome { get; set; }
        public object Lock { get; } = new object();
        public bool EmUso { get; set; }

        public Recurso(string nome)
        {
            Nome = nome;
            EmUso = false;
        }
    }
}
