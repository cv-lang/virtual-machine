using Cvl.VirtualMachine.Core;
using Cvl.VirtualMachine.Core.Variables;
using Cvl.VirtualMachine.Core.Variables.Addresses;
using Cvl.VirtualMachine.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cvl.VirtualMachine
{
    public class HardwareContext
    {
        public WirtualnaMaszyna WirtualnaMaszyna { get; internal set; }

        public Stack Stos { get; set; } = new Stack();
        public long NumerIteracji { get; set; }
        private InstructionBase aktualnaInstrukcja;

        public Metoda AktualnaMetoda { get; set; }
        public bool CzyWykonywacInstrukcje { get; private set; } = true;

        public void Execute()
        {
            //NS.Debug.VM = this; // do debugowania 

            while (CzyWykonywacInstrukcje)
            {
                //try
                {
                    //if (NS.Debug.StopIterationNumber == NumerIteracji)
                    //{
                    //    System.Diagnostics.Debugger.Break();
                    //}
                    aktualnaInstrukcja = PobierzAktualnaInstrukcje();
                    aktualnaInstrukcja.Wykonaj();
                    NumerIteracji++;
                }
                //catch (Exception ex)
                //{
                //    CzyWykonywacInstrukcje = false;
                //    //Status = VirtualMachineState.Exception;
                //    //RzuconyWyjatekCalosc = ex.ToString();
                //    //RzuconyWyjatekWiadomosc = ex.Message; ;
                //    //if (Debugger.IsAttached)
                //    //{
                //    //    Debugger.Break();
                //    //}
                //    return;
                //}
            }
        }


        public InstructionBase PobierzAktualnaInstrukcje()
        {
            var ai = AktualnaMetoda.Instrukcje[AktualnaMetoda.NumerWykonywanejInstrukcji];
            ai.HardwareContext = this.WirtualnaMaszyna.HardwareContext;
            return ai;
        }



        public void WczytajLokalneArgumenty(int iloscArgumentow)
        {
            var lista = new object[iloscArgumentow];
            for (int i = iloscArgumentow - 1; i >= 0; i--)
            {
                var o = Stos.Pop();
                lista[i] = o;
            }
            AktualnaMetoda.LocalArguments.Wczytaj(lista);
        }

        public void ZapiszLokalnyArgument(object o, int indeks)
        {
            AktualnaMetoda.LocalArguments.Ustaw(indeks, o);
        }

        public object PobierzLokalnyArgument(int indeks)
        {
            var obiekt = AktualnaMetoda.LocalArguments.Pobierz(indeks);
            var ow = obiekt as ObjectWraperBase;
            if (ow != null)
            {
                return ow.GetValue();
            }
            return obiekt;
        }

        public void ZapiszLokalnaZmienna(object o, int indeks)
        {
            AktualnaMetoda.LocalVariables.Ustaw(indeks, o);
        }

        public object PobierzLokalnaZmienna(int indeks)
        {
            var obiekt = AktualnaMetoda.LocalVariables.Pobierz(indeks);
            var ow = obiekt as ObjectWraperBase;
            if (ow != null)
            {
                return ow.GetValue();
            }
            return obiekt;
        }

        public LocalVariableAddress PobierzAdresZmiennejLokalnej(int indeks)
        {
            var adres = new LocalVariableAddress();
            adres.Indeks = indeks;
            adres.LokalneZmienne = AktualnaMetoda.LocalVariables;

            return adres;
        }

        public ArgumentAddress PobierzAdresArgumentu(int indeks)
        {
            var adres = new ArgumentAddress();
            adres.Indeks = indeks;
            adres.LokalneArgumenty = AktualnaMetoda.LocalArguments;

            return adres;
        }

        public void WykonajNastepnaInstrukcje()
        {
            var am = AktualnaMetoda;
            am.NumerWykonywanejInstrukcji++;
            am.OffsetWykonywanejInstrukcji
                = am.Instrukcje[am.NumerWykonywanejInstrukcji].Instruction.Offset;
        }

        public void WykonajSkok(int nowyOffset)
        {
            var am = AktualnaMetoda;
            var ins = am.Instrukcje.FirstOrDefault(i => i.Instruction.Offset == nowyOffset);
            am.NumerWykonywanejInstrukcji = am.Instrukcje.IndexOf(ins);
        }

        public void PushObject(object o)
        {
            Stos.PushObject(o);
        }

        public void Push(ElementBase o)
        {
            Stos.Push(o);
        }

        /// <summary>
        /// Zwraca obiekt
        /// jeśli jest adres na stosie to zamienia na obiekt
        /// </summary>
        /// <returns></returns>
        public object PopObject()
        {
            var ob = Stos.Pop();
            if (ob is ObjectWraperBase)
            {
                var v = ob as ObjectWraperBase;
                return v.GetValue();
            }

            return ob;
        }

        public object Pop()
        {
            var ob = Stos.Pop();

            return ob;
        }

        public object PobierzElementZeStosu(int numerElementuOdSzczytu)
        {
            return Stos.PobierzElementZeStosu(numerElementuOdSzczytu);
        }
    }
}
