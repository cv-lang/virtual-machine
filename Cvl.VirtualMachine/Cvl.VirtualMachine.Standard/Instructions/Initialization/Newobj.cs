using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Initialization
{
    /// <summary>
    /// Creates a new object or a new instance of a value type, pushing an object reference (type O) onto the evaluation stack.
    /// https://msdn.microsoft.com/pl-pl/library/system.reflection.emit.opcodes.newobj(v=vs.110).aspx
    /// </summary>
    public class Newobj : InstructionBase
    {
        public override void Wykonaj()
        {
            var md = Instruction.Operand as System.Reflection.MethodBase;
            var typMono = md.DeclaringType;
            var typ = md.DeclaringType;//typMono.GetSystemType();
            var iloscParametrow = md.GetParameters().Length;
            var listaParametrow = new List<Object>();
            for (int i = 0; i < iloscParametrow; i++)
            {
                listaParametrow.Add(HardwareContext.PopObject());
            }

            listaParametrow.Reverse();

            //Obsługa akcji z dwoma parametrami
            if (typ.Name.Contains("Action"))
            {
                var p_1 = listaParametrow[1];
                var p_0 = listaParametrow[0];

                var genericArgument = typMono.GetGenericArguments()[0];
                var gaSystem = genericArgument;

                var metoda = p_1 as MethodInfo;
                var nazwaMetody = metoda.Name;

                var actionT = typeof(Action<>).MakeGenericType(gaSystem);
                var action = Delegate.CreateDelegate(actionT, p_0, nazwaMetody);
                HardwareContext.PushObject(action);
                HardwareContext.WykonajNastepnaInstrukcje();
            }
            else
            {
                var constructor = md as ConstructorInfo;
                if (constructor != null)
                {
                    if (md.IsStatic)
                    {
                        var nowyObiekt = constructor.Invoke(null, listaParametrow.ToArray());
                        //Activator.CreateInstance(typ, listaParametrow.ToArray());
                        HardwareContext.PushObject(nowyObiekt);
                    }
                    else
                    {
                        if (listaParametrow.Any() == false)
                        {
                            //tworze po prostu dany obiekt - danego typu
                            var nowyObiekt = Activator.CreateInstance(typ, null);
                            HardwareContext.PushObject(nowyObiekt);
                        }
                        else
                        {
                            var nowyObiekt = Activator.CreateInstance(typ, listaParametrow.ToArray());
                            HardwareContext.PushObject(nowyObiekt);
                        }
                    }
                }
                else
                {
                    var nowyObiekt = md.Invoke(null, listaParametrow.ToArray());
                    //Activator.CreateInstance(typ, listaParametrow.ToArray());
                    HardwareContext.PushObject(nowyObiekt);
                }
                HardwareContext.WykonajNastepnaInstrukcje();
            }
        }
    }
}
