using Cvl.VirtualMachine.Core;
using Cvl.VirtualMachine.Core.Attributes;
using Cvl.VirtualMachine.Instructions.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using Cvl.VirtualMachine.Core.Variables;
using Cvl.VirtualMachine.Core.Variables.Values;

namespace Cvl.VirtualMachine.Instructions.Calls
{
    /// <summary>
    /// Calls the method indicated by the passed method descriptor.
    /// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.call?view=netcore-3.1
    /// https://www.ecma-international.org/wp-content/uploads/ECMA-335_6th_edition_june_2012.pdf page 422
    /// </summary>
    public class Call : InstructionBase
    {        
        public override void Wykonaj()
        {
            var method = Instruction.Operand as System.Reflection.MethodBase;
            //var methodDef = methodRef.Resolve();
            var parameters = new List<object>();
            object instance = null;
            object instancePop = null;

            var methodParameters = method.GetParameters();
            foreach (var paramDef in methodParameters)
            {
                var parameterValue = MethodContext.PopObject();
                parameters.Add(parameterValue);
            }
            if (method.IsStatic == false)
            {
                instancePop = MethodContext.Pop();
                if (instancePop is ObjectWraperBase wraper)
                {
                    instance = wraper.GetValue();
                }
                else
                {
                    instance = instancePop;
                }
            }

            parameters.Reverse();
                        
            MethodContext.WirtualnaMaszyna.EventCall(method, parameters);

            if (method.Name.Equals("Hibernate") && method.DeclaringType == typeof(VirtualMachine))
            {
                //wywołał metodę do hibernacji wirtualnej maszyny
                if (parameters.Count > 0)
                {
                    var p = (object[])parameters[0];
                    this.MethodContext.WirtualnaMaszyna.HibernateVirtualMachine(p);
                }
                else
                {
                    this.MethodContext.WirtualnaMaszyna.HibernateVirtualMachine(null);
                }

                return;
            }

            if (method.Name.Equals("EndProcess") && method.DeclaringType == typeof(VirtualMachine))
            {
                //wywołał metodę do hibernacji wirtualnej maszyny
                this.MethodContext.WirtualnaMaszyna.EndProcessVirtualMachine();
                return;
            }

            //if (method.IsSetter)
            //{
            //    setter(methodDef, instance, parameters);
            //}
            //else if (methodDef.IsGetter)
            //{
            //    getter(methodRef, instance, parameters);
            //}
            //else
            {
                //Wykonywanie
                if (CzyWykonacCzyInterpretowac(method, instance) == true)
                {
                    //wykonywanie

                    

                    //var methodInfo = type.GetMethod(methodRef);
                    var dopasowaneParametry = new List<object>();

                    int i = 0;
                    foreach (var parameter in parameters)
                    {
                        var methodParam = method.GetParameters()[i];
                        i++;

                        if (methodParam.ParameterType == typeof(bool) && parameter is int)
                        {
                            dopasowaneParametry.Add(Convert.ToBoolean((int)parameter));
                        }
                        else
                        {
                            dopasowaneParametry.Add(parameter);
                        }
                    }

                    object ret = null;
                    try
                    {
                        if (method.IsConstructor == true)
                        {
                            var constructor = method as ConstructorInfo;
                            ret = constructor.Invoke(dopasowaneParametry.ToArray());

                            LogMethodCall(method.Name, dopasowaneParametry, ret);

                            //po wykonaniu odznaczam że był powrót z funkcji (bo już nie będzie instrukcji ret)
                            MethodContext.WirtualnaMaszyna.EventRet(ret);

                            if (instancePop is ObjectWraperBase wraperBase)
                            {
                                wraperBase.SetValue(ret);
                            }
                            else
                            {
                                MethodContext.PushObject(ret);
                            }
                        } 
                        else if (instance == null && method.IsStatic == false)
                        {
                            //wykonywanie metody nie statycznej dla instance==null np. null.Equals(..)

                            var expressionParameters = new List<UnaryExpression>();
                            i = 0;
                            foreach (var item in dopasowaneParametry)
                            {
                                var methodParam = method.GetParameters()[i];
                                i++;
                                expressionParameters
                                    .Add(Expression.Convert(Expression.Constant(item), methodParam.ParameterType));
                            }

                            var call = Expression.Call(Expression.Constant(instance,
                                //typeof(int?)),
                                //method.DeclaringType),
                                MethodContext.ConstrainedType), //ConstrainedType set by Constrained instruction
                                (MethodInfo)method,
                                expressionParameters);

                            MethodContext.ConstrainedType = null;
                            var lambda = Expression.Lambda(call).Compile();
                            ret = lambda.DynamicInvoke();

                            LogMethodCall(method.Name, dopasowaneParametry, ret);

                            //po wykonaniu odznaczam że był powrót z funkcji (bo już nie będzie instrukcji ret)
                            MethodContext.WirtualnaMaszyna.EventRet(ret);
                        } else
                        {
                            //standardowe wykonywanie metod
                            ret = method.Invoke(instance, dopasowaneParametry.ToArray());

                            LogMethodCall(method.Name, dopasowaneParametry, ret);

                            //po wykonaniu odznaczam że był powrót z funkcji (bo już nie będzie instrukcji ret)
                            MethodContext.WirtualnaMaszyna.EventRet(ret);
                        }
                    } catch(Exception exception)
                    {
                        //wyjątek z zewnętrznej funkcji
                        HardwareContext.Status = VirtualMachineState.Exception;
                        var throwedException = exception.InnerException ?? exception;
                        HardwareContext.ExceptionHandling.ExceptionHandlinFromCall(throwedException);
                        //HardwareContext.ThrowedException = exception.InnerException;
                        //Throw.ObslugaRzuconegoWyjatku(MethodContext.WirtualnaMaszyna, exception.InnerException);
                        return;
                    }

                    if (method is MethodInfo methodInfo)
                    {
                        if (methodInfo.ReturnType == typeof(void))
                        {
                            //nie zwracam wyniku
                        }
                        else
                        {
                            MethodContext.PushObject(ret);
                        }
                    }
                    MethodContext.WykonajNastepnaInstrukcje();
                }
                else
                {
                    //interpretowanie

                    //tworzę nową metodę i wrzucam ją na stos wykonania

                    var m = new MethodState(method, MethodContext.WirtualnaMaszyna, instance);
                    m.WczytajInstrukcje();
                    MethodContext = m;
                    var iloscArgumentow = method.GetParameters().Count();

                    if (method.IsStatic == false)
                    {
                        MethodContext.PushObject(instance);
                        iloscArgumentow += 1;
                    }

                    foreach (var parameter in parameters)
                    {
                        MethodContext.PushObject(parameter);
                    }

                    MethodContext.WczytajLokalneArgumenty(iloscArgumentow);
                    

                    //wrzucam na stos wykonania nową metodę
                    HardwareContext.PushAktualnaMetode(MethodContext);
                }
            }


        }


        private void LogMethodCall(string name, List<object> parameters, object ret)
        {
           //var log =MethodContext.Logger.Trace($"Call {name}");
           // foreach (var item in parameters)
           // {
           //     log.AddParameter(item);
           // }

           // log.AddParameter(ret, "return");
        }

        private void setter(MethodDefinition methodDefinition, object instance, List<object> parameters)
        {
            //wykonujemy settera
            var dane = parameters.Last();
            var typ = instance.GetType();

            //var m2 = typ.GetProperty(methodDefinition.Name.Replace("set_", ""));
            //if (m2.PropertyType == typeof(bool))
            //{ //bool czasem jest łączony z int
            //    if (dane is int)
            //    {
            //        var daneInt = (int)dane;
            //        m2.SetValue(instance, (daneInt != 0));
            //    }
            //}
            //else
            //{
            //    m2.SetValue(instance, dane);
            //}


            MethodContext.WykonajNastepnaInstrukcje();
        }

        //private void getter(MethodReference methodReference, object instance, List<object> parameters)
        //{
        //    //wykonuje gettera - 

        //    var typ = methodReference.DeclaringType.GetSystemType();

        //    var argumenty = parameters.ToArray();
        //    var propertyInfo = typ.GetProperty(methodReference.Name.Replace("get_", ""));
        //    object wynik = null;

        //    if (typ.ContainsGenericParameters)
        //    {
        //        var typNulleblowany = instance.GetType();
        //        var typNulleble = typ.MakeGenericType(typNulleblowany);
        //        var pi = typNulleble.GetProperty(methodReference.Name.Replace("get_", ""));
        //        wynik = pi.GetValue(instance, argumenty);
        //    }
        //    else
        //    {
        //        wynik = propertyInfo.GetValue(instance, argumenty);
        //    }

        //    if (wynik == null)
        //    {
        //        PushObject(wynik);
        //    }
        //    else
        //    {
        //        //sprawdzam czy zwracany tym jest Nullable<typ>
        //        if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.Name.Contains("Nullable"))
        //        {
        //            //jest nulable, więc wynik muszę opakować w nulable
        //            var typWyniku = wynik.GetType();
        //            if (typWyniku.IsValueType)
        //            {
        //                var typNullable = typeof(Nullable<>).MakeGenericType(typWyniku);
        //                var wynikNullable = Activator.CreateInstance(typNullable, wynik);
        //                PushObject(wynikNullable);
        //            }
        //            else
        //            {
        //                PushObject(wynik);
        //            }
        //        }
        //        else
        //        {
        //            //zwykły typ, więc go zwracam - takim jakim jest
        //            PushObject(wynik);
        //        }
        //    }

        //    WykonajNastepnaInstrukcje();
        //}



        //public override void Wykonaj()
        //{
        //    var mr = instrukcja.Operand as MethodReference;
        //    if (mr.FullName.Contains("VirtualMachine::Hibernate()"))
        //    {
        //        //wywołał metodę do hibernacji wirtualnej maszyny
        //        WirtualnaMaszyna.HibernujWirtualnaMaszyne();
        //        return;
        //    }
        //    var md = mr.Resolve();


        //    //pobieram argumenty ze stosu i kładę do zmiennych funkcji      
        //    int iloscArgumentow = md.Parameters.Count;
        //    object instancja = null;
        //    if (md.HasThis)
        //    {
        //        iloscArgumentow++;//+1 to zmienna this
        //       //pobieram wlasciwa instancje obiektu ktorego metode wykonamy
        //        instancja = PobierzElementZeStosu(iloscArgumentow - 1);
        //    }



        //    //sprawdzam czy getter lub seter lub czy metoda lub klasa oznaczona atrybutem Interpertuj
        //    if (CzyWykonacCzyInterpretowac(md) == true)
        //    {
        //        //wykonujemy
        //        WykonajMetode(md, instancja);
        //    }
        //    else
        //    {
        //        //interpretujemy
        //        var staraMetoda = WirtualnaMaszyna.AktualnaMetoda;

        //        var m = new Metoda();
        //        m.NazwaTypu = md.DeclaringType.FullName;
        //        m.NazwaMetody = md.Name;
        //        m.AssemblyName = md.DeclaringType.Module.FullyQualifiedName;
        //        m.NumerWykonywanejInstrukcji = 0;

        //        WirtualnaMaszyna.AktualnaMetoda = m;

        //        //pobieram argumenty ze stosu i kładę do zmiennych funkcji
        //        WczytajLokalneArgumenty(iloscArgumentow);

        //        //zapisuję aktualną metodę na stosie
        //        PushObject(staraMetoda);
        //        WykonajNastepnaInstrukcje();
        //    }            
        //}

        //public void WykonajMetode(MethodReference mr, object instancja)
        //{
        //    var md = mr.Resolve();
        //    var gm = mr as Mono.Cecil.GenericInstanceMethod;

        //    //wykonujemy
        //    if (md != null && md.IsGetter == true)
        //    {
        //        Type typ;
        //        //wykonuje gettera - 

        //        //jeśli instancja jest adresem do obiektu
        //        var adres = instancja as ObjectAddressWraper;
        //        if (adres != null)
        //        {
        //            instancja = adres.GetValue();
        //        }

        //        //getter może mieć parametr - jeśli jest operatore [parametry]
        //        int iloscArgumentow2 = mr.Parameters.Count;
        //        var arg = new List<object>();
        //        for (int i = 0; i < iloscArgumentow2; i++)
        //        {
        //            var o = PopObject();
        //            arg.Add(o);
        //        }


        //        //!! z jakiegoś powodu to tu było, ale jak mamy getter z instancją
        //        //to instancję pobieramy w funkcji powyżej i drugie pobranie psuje stos
        //        if (md.HasThis)
        //        {
        //            //Wykonujemy gettera
        //            instancja = PopObject();
        //        }
        //        typ = md.DeclaringType.GetSystemType();


        //        arg.Reverse();
        //        var argumenty = arg.ToArray();
        //        var propertyInfo = typ.GetProperty(md.Name.Replace("get_", ""));
        //        object wynik = null;

        //        if (typ.IsGenericType)
        //        {

        //            var typNulleblowany = instancja.GetType();
        //            var typNulleble = typ.MakeGenericType(typNulleblowany);
        //            var pi = typNulleble.GetProperty(md.Name.Replace("get_", ""));
        //            wynik = pi.GetValue(instancja, argumenty);
        //        }
        //        else
        //        {
        //            wynik = propertyInfo.GetValue(instancja, argumenty);
        //        }

        //        if (wynik == null)
        //        {
        //            PushObject(wynik);
        //        } else
        //        {
        //            //sprawdzam czy zwracany tym jest Nullable<typ>
        //            if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.Name.Contains("Nullable"))
        //            {
        //                //jest nulable, więc wynik muszę opakować w nulable
        //                var typWyniku = wynik.GetType();
        //                if (typWyniku.IsValueType)
        //                {
        //                    var typNullable = typeof(Nullable<>).MakeGenericType(typWyniku);
        //                    var wynikNullable = Activator.CreateInstance(typNullable, wynik);
        //                    PushObject(wynikNullable);
        //                }
        //                else
        //                {
        //                    PushObject(wynik);
        //                }
        //            } else
        //            {
        //                //zwykły typ, więc go zwracam - takim jakim jest
        //                PushObject(wynik);
        //            }
        //        }


        //        WykonajNastepnaInstrukcje();
        //    }
        //    else if (md != null && md.IsSetter)
        //    {
        //        //wykonujemy settera
        //        var dane = PopObject();
        //        instancja = PopObject();
        //        var typ = instancja.GetType();

        //        var m2 = typ.GetProperty(md.Name.Replace("set_", ""));

        //        m2.SetValue(instancja, dane );
        //        WykonajNastepnaInstrukcje();
        //    }
        //    else
        //    {
        //        //wykonujemy metodę 
        //        int iloscArgumentow2 = mr.Parameters.Count;
        //        var arg = new List<object>();
        //        for (int i = 0; i < iloscArgumentow2; i++)
        //        {
        //            var o = PopObject();
        //            arg.Add(o);
        //        }
        //        if (md.HasThis)
        //        {
        //            //metoda ma this (metoda obiektu)
        //            instancja = PopObject();
        //            arg.Reverse();
        //            var argumenty = arg.ToArray();
        //            var zmienioneArgumenty = new List<object>();
        //            var obj = instancja;

        //            //pobieram dostępne motody i sprawdzam najlepsze dopasowanie
        //            var typDeklarujacy = md.DeclaringType.Resolve().GetSystemType();
        //            if(typDeklarujacy == null)
        //            {
        //                typDeklarujacy = obj.GetType();
        //            }
        //            var meth = typDeklarujacy.GetMethods().Where(m=>m.Name == md.Name);

        //            if (meth.Count() >= 1)
        //            {
        //                //mamy więcej metod o danej nazwie - sprawdzamy dopasowanie po parametrach

        //                foreach (var methodInfo in meth)
        //                {
        //                    zmienioneArgumenty.Clear();

        //                    var parametry = methodInfo.GetParameters();
        //                    int i = 0;
        //                    bool czyZgodneParametry = true;
        //                    if(parametry.Count() != argumenty.Count())
        //                    {
        //                        //metoda o dobrej nazwie ale zle ilosci parametrow
        //                        continue;
        //                    }

        //                    foreach (var parameterInfo in parametry)
        //                    {
        //                        var argument = argumenty[i];
        //                        if (argument is int)
        //                        {
        //                            //mamy albo int albo enum
        //                            if (parameterInfo.ParameterType != typeof (int))
        //                            {
        //                                if (parameterInfo.ParameterType == typeof(bool))
        //                                {
        //                                    var j = (int)argument;
        //                                    zmienioneArgumenty.Add(j == 1);
        //                                }
        //                                else if(!parameterInfo.ParameterType.IsEnum)
        //                                {
        //                                    czyZgodneParametry = false;
        //                                    break;
        //                                } else 
        //                                {
        //                                    //zgodny ale enum zmieniam go na enum
        //                                    var e = Enum.ToObject(parameterInfo.ParameterType, (int)argument);
        //                                    zmienioneArgumenty.Add(e);
        //                                }
        //                            }
        //                            else
        //                            {
        //                                //zgodny i int
        //                                zmienioneArgumenty.Add(argument);
        //                            }
        //                        }
        //                        else
        //                        {

        //                            if (argument != null)
        //                            {
        //                                var argTyp = argumenty[i].GetType();
        //                                czyTypySaTakieSame(argTyp, parameterInfo.ParameterType);
        //                                if (czyTypySaTakieSame(argTyp, parameterInfo.ParameterType) == false)
        //                                {
        //                                    czyZgodneParametry = false;
        //                                    break;
        //                                }
        //                                else
        //                                {
        //                                    //zgodny null
        //                                    zmienioneArgumenty.Add(argument);
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (parameterInfo.ParameterType.IsValueType == true)
        //                                {
        //                                    czyZgodneParametry = false;
        //                                    break;
        //                                }
        //                                else
        //                                {
        //                                    zmienioneArgumenty.Add(argument);
        //                                }
        //                            }
        //                        }
        //                        i++;
        //                    }
        //                    //sprawdzanie dopasowania dla tego motody zakończone
        //                    if (czyZgodneParametry == true)
        //                    {

        //                        //mamy dopasowaną metodę
        //                        //zamieniam listę argumentów
        //                        argumenty = zmienioneArgumenty.ToArray();
        //                        try
        //                        {
        //                            if (gm != null && gm.HasGenericArguments == true)
        //                            {
        //                                var typ = instancja.GetType();
        //                                var typParametruGenerycznego = gm.GenericArguments[0].GetSystemType();
        //                                MethodInfo method = typ.GetMethod(mr.Name);
        //                                MethodInfo generic = method.MakeGenericMethod(typParametruGenerycznego);
        //                                var wynik = generic.Invoke(instancja, argumenty);
        //                                PushObject(wynik);
        //                            } else if (mr.ReturnType.FullName != typeof(void).FullName)
        //                            {
        //                                var wynik = methodInfo.Invoke(instancja, argumenty);
        //                                PushObject(wynik);                                        
        //                            } else
        //                            {
        //                                methodInfo.Invoke(instancja, argumenty);
        //                            }

        //                            WykonajNastepnaInstrukcje();
        //                            return;
        //                        }
        //                        catch(Exception ex)
        //                        {

        //                        }
        //                        break;
        //                    }

        //                }
        //            }
        //            //jeśli tu dochodzimy to znaczy że nie udało się dopasować parametrów - lub jest jedna metoda
        //            //uruchamiamy to przez interefejs dynamic

        //            if (gm != null && gm.HasGenericArguments == true)
        //            {
        //                var typ = instancja.GetType();
        //                var typParametruGenerycznego = gm.GenericArguments[0].GetSystemType();
        //                MethodInfo method = typ.GetMethod(mr.Name);
        //                MethodInfo generic = method.MakeGenericMethod(typParametruGenerycznego);
        //                var wynik = generic.Invoke(instancja, argumenty);
        //                PushObject(wynik);
        //            }
        //            else
        //            {
        //                if (mr.ReturnType.FullName != typeof(void).FullName && mr.HasGenericParameters == false)
        //                {
        //                    var wynikDyn = Dynamic.InvokeMember(instancja, mr.Name, argumenty);
        //                    var wynik = (object)wynikDyn;
        //                    PushObject(wynik);
        //                }
        //                else if (mr.ReturnType.FullName != typeof(void).FullName && mr.HasGenericParameters == true)
        //                {
        //                    var typ = instancja.GetType();
        //                    var genParam = md.GenericParameters[0];
        //                    var genD = genParam.Resolve();

        //                    var typParametruGenerycznego = md.GenericParameters[0].GetSystemType();
        //                    MethodInfo method = typ.GetMethod(mr.Name);
        //                    MethodInfo generic = method.MakeGenericMethod(typParametruGenerycznego);
        //                    var wynik = generic.Invoke(instancja, argumenty);
        //                    PushObject(wynik);
        //                }
        //                else
        //                {
        //                    Dynamic.InvokeMemberAction(instancja, mr.Name, argumenty);
        //                }
        //            }
        //            WykonajNastepnaInstrukcje();
        //        } else if( mr.Name.Equals("op_Equality"))
        //        {
        //            dynamic a = arg[0];
        //            dynamic b = arg[1];

        //            dynamic wynik = a == b;
        //            PushObject(wynik);
        //            WykonajNastepnaInstrukcje();
        //        } else
        //        {
        //            //metoda statyczna (bez this)
        //            arg.Reverse();
        //            var staticContext = InvokeContext.CreateStatic;
        //            var typ = md.DeclaringType.GetSystemType();
        //            var wynikDyn = Dynamic.InvokeMember(staticContext(typ), mr.Name, arg.ToArray());

        //            var wynik = (object) wynikDyn;
        //            if (mr.ReturnType.FullName != typeof(void).FullName)
        //            {
        //                PushObject(wynik);
        //            }
        //            WykonajNastepnaInstrukcje();
        //        }


        //        //wykonujemy metodę

        //    }
        //}

        //private bool czyTypySaTakieSame(Type t1, Type t2)
        //{
        //    if (t1.IsGenericType != t2.IsGenericType)
        //    {
        //        return false;
        //    }

        //    if (t1.IsGenericType == false)
        //    {
        //        var t = t1;

        //        while(true)
        //        {
        //            if(t == t2)
        //            {
        //                return true;
        //            }

        //            if(t == typeof(object))
        //            {
        //                return false;
        //            }

        //            t = t.BaseType;
        //        }
        //        return t1 == t2;
        //    }
        //    else
        //    {
        //        return t1.Name == t2.Name;
        //    }


        //    return true;
        //}


        /// <summary>
        /// Metoda określa czy dane metoda ma być wykonana czy interpretowana.
        /// Interpretowana metoda jest rozbijana na assemblera i każda instrukcja jest interpretowana w wirtualnej maszynie
        /// "Wykonywane" motody są wykonywane na procesorze (tak jak wszystkie inne funkcje)
        /// </summary>
        /// <param name="md"></param>
        /// <returns>true - znaczy wykonywać
        ///         false - znaczy interpretować</returns>
        public bool CzyWykonacCzyInterpretowac(MethodBase mr, object instance)
        {
            if(mr.IsAbstract && instance.GetType().IsAbstract)
            {
                return true;
            }

            return MethodContext.WirtualnaMaszyna.CzyWykonacCzyInterpretowac(mr);            
        }
    }
}
