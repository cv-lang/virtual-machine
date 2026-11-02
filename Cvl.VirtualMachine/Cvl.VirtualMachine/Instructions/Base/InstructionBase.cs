using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Cvl.VirtualMachine.Core.Variables.Addresses;
using Cvl.VirtualMachine.Core;

namespace Cvl.VirtualMachine.Instructions
{
    public class InstructionBase
    {
        internal Instruction Instruction { get; set; }
        public MethodState MethodContext { get; set; }
        public ThreadOfControl HardwareContext { get; internal set; }

        /// <summary>
        /// Czy instrukcja ma zatrzymać wykonywanie - używane przez debuger VW
        /// </summary>
        public bool Breakpoint { get; set; }

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

        /// <summary>
        /// Warunek brtrue/brfalse: null i zero (także bool, char i enum) to fałsz, cała reszta to prawda
        /// </summary>
        protected static bool CzyPrawda(object wartosc)
        {
            if (wartosc == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(wartosc.GetType()))
            {
                case TypeCode.Boolean: return (bool)wartosc;
                case TypeCode.Char: return (char)wartosc != 0;
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return Convert.ToDecimal(wartosc) != 0;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Na stosie IL małe typy (byte, bool, char, enum...) leżą jako int,
        /// a wywołanie przez refleksję wymaga dokładnie typu parametru
        /// </summary>
        public static object DopasujTyp(object wartosc, Type typ)
        {
            if (wartosc == null || wartosc.GetType() == typ)
            {
                return wartosc;
            }

            long liczba;
            switch (wartosc)
            {
                case int v: liczba = v; break;
                case long v: liczba = v; break;
                case uint v: liczba = v; break;
                case ulong v: liczba = unchecked((long)v); break;
                case short v: liczba = v; break;
                case ushort v: liczba = v; break;
                case sbyte v: liczba = v; break;
                case byte v: liczba = v; break;
                case char v: liczba = v; break;
                case bool v: liczba = v ? 1 : 0; break;
                default: return wartosc;
            }

            if (typ.IsEnum)
            {
                return Enum.ToObject(typ, liczba);
            }

            switch (Type.GetTypeCode(typ))
            {
                case TypeCode.Boolean: return liczba != 0;
                case TypeCode.Byte: return unchecked((byte)liczba);
                case TypeCode.SByte: return unchecked((sbyte)liczba);
                case TypeCode.Int16: return unchecked((short)liczba);
                case TypeCode.UInt16: return unchecked((ushort)liczba);
                case TypeCode.Char: return unchecked((char)liczba);
                case TypeCode.Int32: return unchecked((int)liczba);
                case TypeCode.UInt32: return unchecked((uint)liczba);
                case TypeCode.Int64: return liczba;
                case TypeCode.UInt64: return unchecked((ulong)liczba);
                default: return wartosc;
            }
        }

        public int Offset => Instruction.Offset;
        public string Code => Instruction.OpCode.ToString();
        public string Operand => Instruction.Operand?.ToString();

        public override string ToString()
        {
            return $"{Instruction}";
        }
    }
}
