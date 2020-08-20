using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Cvl.VirtualMachine.Core.Variables.Addresses;

namespace Cvl.VirtualMachine.Instructions
{
    public class InstructionBase
    {
        internal Instruction Instruction { get; set; }
        public HardwareContext MethodContext { get; set; }
        

        public virtual void Wykonaj()
        {
        }

        public void Inicialize(Instruction instruction)
        {
            Instruction = instruction;
        }

        #region Stack method

        protected object PopObject()
        {
            return MethodContext.PopObject();
        }

        protected object Pop()
        {
            return MethodContext.Pop();
        }

        protected void Push(Core.ElementBase element)
        {
             MethodContext.Push(element);
        }

        protected void PushObject(object obj)
        {
            MethodContext.PushObject(obj);
        }

        protected void WczytajLokalneArgumenty(int v)
        {
            MethodContext.WczytajLokalneArgumenty(v);
        }

        protected object PobierzLokalnyArgument(int v)
        {
            return MethodContext.PobierzLokalnyArgument(v);
        }

        protected void WykonajSkok(int instructionOffset)
        {
            MethodContext.WykonajSkok(instructionOffset);
        }

        protected void WykonajNastepnaInstrukcje()
        {
            MethodContext.WykonajNastepnaInstrukcje();
        }

        protected ArgumentAddress PobierzAdresArgumentu(int index)
        {
            return MethodContext.PobierzAdresArgumentu(index);
        }

        protected object PobierzLokalnaZmienna(int localIndex)
        {
            return MethodContext.PobierzLokalnaZmienna(localIndex);
        }


        protected LocalVariableAddress PobierzAdresZmiennejLokalnej(int index)
        {
            return MethodContext.PobierzAdresZmiennejLokalnej(index);
        }

        protected void ZapiszLokalnyArgument(object o, int index)
        {
            MethodContext.ZapiszLokalnyArgument(o,index);
        }

        protected void ZapiszLokalnaZmienna(object o, int localIndex)
        {
            MethodContext.ZapiszLokalnaZmienna(o, localIndex);
        }

        protected void EventCall(MethodBase method, List<object> parameters)
        {
            MethodContext.WirtualnaMaszyna.EventCall(method, parameters);
        }

        protected void EventRet(object ret = null)
        {
            MethodContext.WirtualnaMaszyna.EventRet(ret);
        }

        protected void EventThrowException(Exception rzuconyWyjatek)
        {
            MethodContext.WirtualnaMaszyna.EventThrowException(rzuconyWyjatek);
        }
        #endregion

        public override string ToString()
        {
            return $"{Instruction}";
        }
    }
}
