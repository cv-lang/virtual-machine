using Cvl.VirtualMachine.Instructions.Base;
using Cvl.VirtualMachine.Instructions.Calls;
using Cvl.VirtualMachine.Instructions.Special;
using Cvl.VirtualMachine.Instructions.Storage;
using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cvl.VirtualMachine.Instructions
{
    public interface IInstructionFactory
    {
        InstructionBase CreateInstruction(Instruction instrukcja);
        VirtualMachine WirtualnaMaszyna { get; set; }
    }

    public class InstructionFactory : IInstructionFactory
    {
        public VirtualMachine WirtualnaMaszyna { get; set; }

        public virtual InstructionBase CreateInstruction(Instruction instrukcja)
        {
            return null;
        }

        public T CreateInstruction<T>(Instruction instruction, Action<T> a = null)
            where T : InstructionBase, new()
        {
            var intst = new T();
            intst.HardwareContext = WirtualnaMaszyna.HardwareContext;
            intst.Inicialize(instruction);
            if(a != null)
            {
                a.Invoke(intst);
            }
            return intst;
        }

        public T CreateIndexedInstruction<T>(Instruction instruction, int? index = null)
            where T : IndexedInstruction, new()
        {
            var intst = new T();
            intst.Inicialize(instruction, index);
            return intst;
        }
    }

    public  class InstructionsFactory
    {
        public InstructionBase UtworzInstrukcje(Instruction instrukcja, VirtualMachine wirtualnaMaszyna)
        {
            if(instructionFactories == null)
            {
                RegisterInstructionFactory();
            }

            foreach (var facotry in instructionFactories)
            {
                facotry.WirtualnaMaszyna = wirtualnaMaszyna;
                var inst = facotry.CreateInstruction(instrukcja);
                if(inst != null)
                {
                    return inst;
                }
            }

            throw new Exception("Brak instrukcji " + instrukcja.OpCode.Name + " " + instrukcja.ToString());
            //try
            //{


            //    switch (instrukcja.OpCode.Name)
            //    {




            //        //case "add":
            //        //    return new Add(instrukcja);


            //        //case "br.s":
            //        //    return new Br_S(instrukcja);
            //        //case "sub":
            //        //    return new Sub(instrukcja);
            //        //case "ldstr":
            //        //    return new Ldstr(instrukcja);
            //        //case "newobj":
            //        //    return new Newobj(instrukcja);
            //        //case "throw":
            //        //    return new Throw(instrukcja);
            //        //case "leave.s":
            //        //    return new Leave_S(instrukcja);
            //        //case "endfinally":
            //        //    return new Endfinally(instrukcja);
            //        //case "pop":
            //        //    return new Pop(instrukcja);
            //        //case "ldc.i4.m1":
            //        //    return new Ldc(-1, instrukcja);
            //        //case "ldc.i4.0":
            //        //    return new Ldc(0, instrukcja);
            //        //case "ldc.i4.1":
            //        //    return new Ldc(1, instrukcja);
            //        //case "ldc.i4.2":
            //        //    return new Ldc(2, instrukcja);
            //        //case "ldc.i4.3":
            //        //    return new Ldc(3, instrukcja);
            //        //case "ldc.i4.4":
            //        //    return new Ldc(4, instrukcja);
            //        //case "ldc.i4.5":
            //        //    return new Ldc(5, instrukcja);
            //        //case "ldc.i4.6":
            //        //    return new Ldc(6, instrukcja);
            //        //case "ldc.i4.s":
            //        //    return new Ldc((sbyte)instrukcja.Operand, instrukcja);
            //        //case "ldc.i4":
            //        //    return new Ldc((Int32)instrukcja.Operand, instrukcja);
            //        //case "ldc.r8":
            //        //    return new Ldc(instrukcja.Operand, instrukcja);
            //        //case "ldnull":
            //        //    return new Ldc(null, instrukcja);

            //        //case "cgt":
            //        //    return new Cgt(instrukcja);
            //        //case "clt":
            //        //    return new Clt(instrukcja);
            //        //case "ceq":
            //        //    return new Ceq(instrukcja);
            //        //case "brfalse":
            //        //    return new Brfalse(instrukcja);
            //        //case "brfalse.s":
            //        //    return new Brfalse(instrukcja);
            //        //case "brtrue.s":
            //        //    return new Brtrue(instrukcja);
            //        //case "constrained.":
            //        //case "constrained":
            //        //    return new Constrained(instrukcja);
            //        //case "castclass":
            //        //    return new Castclass(instrukcja);
            //        //case "isinst":
            //        //    return new Isinst(instrukcja);
            //        //case "ldtoken":
            //        //    return new Ldtoken(instrukcja);
            //        //case "newarr":
            //        //    return new Newarr(instrukcja);
            //        //case "dup":
            //        //    return new Dup(instrukcja);
            //        //case "stelem.ref":
            //        //    return new Stelem_Ref(instrukcja);
            //        //case "ldfld":
            //        //    return new Ldfld(instrukcja);
            //        //case "stfld":
            //        //    return new Stfld(instrukcja);
            //        //case "cgt.un":
            //        //    return new Cgt_Un(instrukcja);
            //        //case "conv.r8":
            //        //    return new Conv_R8(instrukcja);
            //        //case "unbox.any":
            //        //    return new Unbox_Any(instrukcja);
            //        //case "br":
            //        //    return new Br(instrukcja);
            //        //case "initobj":
            //        //    return new Initobj(instrukcja);
            //        //case "box":
            //        //    return new Box(instrukcja);
            //        //case "unbox":
            //        //    return new Unbox(instrukcja);
            //        //case "ldsfld":
            //        //    return new Ldsfld(instrukcja);
            //        //case "ldftn":
            //        //    return new Ldftn(instrukcja);
            //        //case "stsfld":
            //        //    return new Stsfld(instrukcja);
            //        //case "mul":
            //        //    return new Mul(instrukcja);
            //        //case "div":
            //        //    return new Div(instrukcja);
            //        //case "beq.s":
            //        //    return new Beq(instrukcja);
            //    }

            //    throw new Exception("Brak instrukcji " + instrukcja.OpCode.Name + " " + instrukcja.ToString());
            //}
            //catch (Exception ex)
            //{

            //    throw ex;
            //}
        }


        private List<IInstructionFactory> instructionFactories = null;

        public void RegisterInstructionFactory()
        {
            instructionFactories = new List<IInstructionFactory>();
            foreach (Type mytype in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                .Where(mytype => mytype.GetInterfaces().Contains(typeof(IInstructionFactory))))
            {
                instructionFactories.Add((IInstructionFactory)Activator.CreateInstance(mytype));
            }
        }

        
    }
}
