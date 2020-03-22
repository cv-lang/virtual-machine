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
            var obj= LokalneArgumenty.Obiekty[Indeks];

            if(obj is ObjectWraperBase objectWraper)
            {
                //mamy referencje do obieku opakowującego - pobieramy jego wartość
                return objectWraper.GetValue();
            } else
            {
                //mamy normalny obiekt, zwracamy go
                return obj;
            }
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
