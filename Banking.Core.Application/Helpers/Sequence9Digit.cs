using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Core.Application.Helpers
{
    public class Sequence9Digit
    {
        public string Secuencia(List<string> sequences)
        {
            Random random = new Random();
            int secuencia;
            do
            {
                secuencia = random.Next(100000000, 1000000000); 
            } while (sequences.Contains(secuencia.ToString()));

            return secuencia.ToString();
        }
    }
}
