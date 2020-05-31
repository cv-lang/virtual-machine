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
            
            var constructorInfo = md as ConstructorInfo;
            if (constructorInfo != null)
            {
                if (md.IsStatic)
                {
                    var nowyObiekt = constructorInfo.Invoke(null, listaParametrow.ToArray());
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
                        var dopasowaneTypowoParametry = new List<object>();
                        var constructorParametrerTypes = constructorInfo.GetParameters();
                        for (int i = 0; i < listaParametrow.Count; i++)
                        {
                            var parameter = listaParametrow[i];
                            var typParametru = parameter.GetType();
                            var typKonstruktora = constructorParametrerTypes[i].ParameterType;

                            if(parameter is MethodInfo parameterMethodInfo)
                            {

                            }


                            if (typKonstruktora == typeof(IntPtr) && typParametru == typeof(bool))
                            {
                                dopasowaneTypowoParametry.Add(((bool)parameter ? 1:0));
                            }
                            else
                            {
                                dopasowaneTypowoParametry.Add(parameter);
                            }
                        }

                        var nowyObiekt = constructorInfo.Invoke(dopasowaneTypowoParametry.ToArray());
                        HardwareContext.PushObject(nowyObiekt);

                        ////wyszukuje odpowiedniego konstruktora, przykładowo dla Func<JakisObiekt, bool> konstruktor
                        ////zamiast bool musi być przekonwertowany na IntPrt
                        ////var types = new Type[listaParametrow.Count];
                        ////foreach (Object parameter in listaParametrow)
                        ////{
                        ////    var parameterType = parameter.GetType();

                        ////}

                        //var parameterTypes = listaParametrow.Select(x => x.GetType()).ToArray();

                        //var constructor = typ.GetConstructor(BindingFlags.CreateInstance | BindingFlags.Instance |
                        //                   BindingFlags.NonPublic | BindingFlags.Public,
                        //    null, parameterTypes, null);

                        //if (constructor != null)
                        //{
                        //    var nowyObiekt = constructor.Invoke(listaParametrow.ToArray());
                        //    HardwareContext.PushObject(nowyObiekt);
                        //}
                        //else
                        //{
                        //    var nowyObiekt = Activator.CreateInstance(typ, listaParametrow.ToArray());
                        //    HardwareContext.PushObject(nowyObiekt);
                        //}
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
