using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cvl.VirtualMachine.Core.Variables.Values;

namespace Cvl.VirtualMachine.Core
{

    /// <summary>
    /// Stos wirtualnej maszyny
    /// </summary>
    public class CallStack
    {
        public CallStack()
        {
            stosWewnetrzny = new Stack<MethodState>();
        }

        public Stack<MethodState> PobierzStos()
        {
            return stosWewnetrzny;
        }

        private Stack<MethodState> stosWewnetrzny;

        /// <summary>
        /// Używany tylko do serializacji stosu (zapisuje go jako lista)
        /// </summary>
        public List<MethodState> StosSerializowany
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

        public void Push(MethodState obiekt)
        {
            stosWewnetrzny.Push(obiekt);
        }

        public MethodState Pop()
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
                    str += $"{item};\n ";
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
            var o = Pop();
            return o;
        }

        public MethodState PobierzTopMethodState()
        {
            var tablicaElementowStosu = stosWewnetrzny.ToArray();
            return tablicaElementowStosu[0];
        }
    }
}
