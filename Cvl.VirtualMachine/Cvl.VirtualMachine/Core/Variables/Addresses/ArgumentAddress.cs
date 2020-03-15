using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Core.Variables.Addresses
{
    public class ArgumentAddress : ObjectAddressWraper
    {
        public int Indeks { get; set; }
        public MethodData LokalneArgumenty { get; set; }

        public override object GetValue()
        {
            if (LokalneArgumenty.Obiekty.ContainsKey(Indeks) == false)
            {
                LokalneArgumenty.Obiekty[Indeks] = null;
            }
            return LokalneArgumenty.Obiekty[Indeks];
        }

        public override void SetNull()
        {
            LokalneArgumenty.Obiekty[Indeks] = null;
        }

        public override string ToString()
        {
            return "Adres arg " + Indeks;
        }
    }
}
