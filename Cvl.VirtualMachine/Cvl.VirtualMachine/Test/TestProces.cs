using Cvl.VirtualMachine.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Test
{
    public class ProcesTest
    {
        public string JakasPropercja { get; set; }
        private int zmienna = 4;
        public object Wynik { get; set; }

        public void Start()
        {
            Wynik= Funkcja1(1, 2, "3");
        }

        //[Interpret]
        public void Metoda1()
        {
            zmienna += 3;
        }

        [Interpret]
        public int Funkcja1(int p1, int p2, string p3)
        {
            var zmienalokaln1 = p1;
            var zmienalokalna2 = p2;
            var zmiennaLokaln2 = p2.ToString() + p1.ToString() + zmienalokalna2;

            Metoda1();

            var zmienna3 = zmienalokaln1 + zmiennaLokaln2;

            return zmienna + p1 + p2.ToString().Length + zmienna3.Length;
        }
    }
}
