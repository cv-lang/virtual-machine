using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions
{
    public class InstructionBase
    {
        internal Instruction Instruction { get; set; }
        public WirtualnaMaszyna WirtualnaMaszyna { get; internal set; }

        public virtual void Wykonaj()
        {
        }

        public void Inicialize(Instruction instruction)
        {
            Instruction = instruction;
        }

        protected virtual void InstructionInicialize()
        {

        }

        protected void WykonajNastepnaInstrukcje()
        {
            throw new NotImplementedException();
        }

        protected void PushObject(object o)
        {
            throw new NotImplementedException();
        }

        protected object PobierzLokalnyArgument(object indeks)
        {
            throw new NotImplementedException();
        }
        protected void Push(object o)
        {
            throw new NotImplementedException();
        }

        protected object PobierzAdresArgumentu(object index)
        {
            throw new NotImplementedException();
        }

        protected void ZapiszLokalnaZmienna(object o, int localIndex)
        {
            throw new NotImplementedException();
        }

        protected void ZapiszLokalnyArgument(object o, int index)
        {
            throw new NotImplementedException();
        }

        protected object PopObject()
        {
            throw new NotImplementedException();
        }

        protected object PobierzAdresZmiennejLokalnej(int localIndex)
        {
            throw new NotImplementedException();
        }

        protected object PobierzLokalnaZmienna(int localIndex)
        {
            throw new NotImplementedException();
        }

        protected void WczytajLokalneArgumenty(int v)
        {
            throw new NotImplementedException();
        }
    }
}
