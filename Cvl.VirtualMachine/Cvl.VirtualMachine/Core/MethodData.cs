using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Core
{
    /// <summary>
    /// Dane metody albo parametry metody albo argumenty
    /// </summary>
    public class MethodData
    {
        public MethodData()
        {
            Obiekty = new Dictionary<int, object>();
        }

        public void Ustaw(int index, object obiekt)
        {
            Obiekty[index] = obiekt;
        }

        public object Pobierz(int index)
        {
            if (Obiekty.ContainsKey(index) == false)
            {
                Obiekty[index] = null;
            }
            return Obiekty[index];
        }

        public Dictionary<int, object> Obiekty { get; set; }
        //public object[] Obiekty { get; set; }

        public override string ToString()
        {
            var str = Obiekty.Count + ": ";
            foreach (var item in Obiekty.Keys)
            {
                string wartosc = "";
                if (Obiekty.ContainsKey(item) != false)
                {
                    wartosc = Obiekty[item].ToString();
                }

                str += item.ToString() + "=" + wartosc + ";\n";
            }
            return str;
        }

        public void Wczytaj(object[] lista)
        {
            Obiekty.Clear();
            int i = 0;
            foreach (var item in lista)
            {
                Obiekty[i] = item;
                i++;
            }
        }
    }
}
