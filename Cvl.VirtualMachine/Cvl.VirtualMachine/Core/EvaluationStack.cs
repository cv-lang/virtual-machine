using Cvl.VirtualMachine.Core.Variables.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cvl.VirtualMachine.Core
{
    /// <summary>
    /// Stos wirtualnej maszyny
    /// </summary>
    public class EvaluationStack
    {
        public EvaluationStack()
        {
            stosWewnetrzny = new Stack<ElementBase>();
        }

        public Stack<ElementBase> PobierzStos()
        {
            return stosWewnetrzny;
        }

        private Stack<ElementBase> stosWewnetrzny;

        /// <summary>
        /// Używany tylko do serializacji stosu (zapisuje go jako lista)
        /// </summary>
        public List<ElementBase> StosSerializowany
        {
            get
            {
                return stosWewnetrzny.ToList();
            }
            set
            {
                var l = value.ToList();
                l.Reverse();
                foreach (var item in l)
                {
                    stosWewnetrzny.Push(item);
                }
            }
        }

        public void PushObject(object obiekt)
        {
            var w = new ObjectWraper(obiekt);
            Push(w);
        }

        public void Push(ElementBase obiekt)
        {
            stosWewnetrzny.Push(obiekt);
        }

        public object Pop()
        {
            return stosWewnetrzny.Pop();
        }

        public bool IsEmpty()
        {
            return stosWewnetrzny.Count == 0;
        }

        public override string ToString()
        {
            var str = stosWewnetrzny.Count + ": ";

            foreach (var item in stosWewnetrzny.ToList())
            {
                if (item != null)
                {
                    str += item.ToString() + "; \n";
                }
                else
                {
                    str += "null;\n";
                }
            }

            return str;
        }

        public MethodState PobierzNastepnaMetodeZeStosu()
        {
            while (stosWewnetrzny.Count > 0)
            {
                var o = Pop();
                if(o is ObjectWraper wo)
                {
                    o = wo.Warosc;
                }

                if (o is MethodState)
                {
                    return o as MethodState;
                }
            }

            return null;
        }

        public object PobierzElementZeStosu(int numerElementuOdSzczytu)
        {
            var tablicaElementowStosu = stosWewnetrzny.ToArray();
            return tablicaElementowStosu[numerElementuOdSzczytu];
        }
    }
}
