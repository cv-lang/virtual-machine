//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Text;

//namespace Cvl.VirtualMachine.Core.Extensions
//{
//    public static class MonoTypeExtension
//    {
//        #region Mono.Cecil => System.Type

//        public static Type GetSystemType(this TypeReference typeReference)
//        {
//            var genericInstanceType = typeReference as GenericInstanceType;
//            var nazwa = typeReference.Name;
//            var przestrzenNazw = typeReference.Namespace;
//            var pelnaNazwa = typeReference.FullName.Replace("/", "+");
//            var isNested = typeReference.IsNested;

//            if (genericInstanceType == null)
//            {
//                //mamy zwykły typ - sprawdzam tylko nazwę

//                var assembly = typeReference.Scope.GetSystemAssembly();
//                var typyWewnetrzne = assembly.GetTypes().Where(t => t.IsNested == isNested);
//                return typyWewnetrzne.Single(t => t.FullName == pelnaNazwa);
//            }
//            else
//            {
//                // mamy generika

//                var assembly = genericInstanceType.Scope.GetSystemAssembly();
//                //typ bazowy
//                var typy = assembly.GetTypes().Where(t => t.ContainsGenericParameters && t.IsNested == isNested);

//                Type typBazowy;
//                if (isNested)
//                {
//                    var pelnaNazwaWew = pelnaNazwa.Split('<')[0]; //Generyk wewnętrzny ma po < typ parametru generycznego >
//                    typBazowy = typy.Single(t => t.FullName == pelnaNazwaWew);
//                }
//                else
//                {
//                    typBazowy = typy.Single(t => t.Name == nazwa && t.Namespace == przestrzenNazw);
//                }


//                var listaTypowParametrowGenerycznych = new List<Type>();
//                foreach (var genericArgument in genericInstanceType.GenericArguments)
//                {
//                    var typ = genericArgument.GetSystemType();
//                    listaTypowParametrowGenerycznych.Add(typ);
//                }

//                var typSystemowy = typBazowy.MakeGenericType(listaTypowParametrowGenerycznych.ToArray());
//                return typSystemowy;
//            }
//        }

//        public static Assembly GetSystemAssembly(this IMetadataScope scope)
//        {
//            var assemblyName = scope as AssemblyNameReference;
//            if (assemblyName != null)
//            {
//                return Assembly.Load(assemblyName.FullName);
//            }
//            else
//            {
//                var moduleDefinition = scope as ModuleDefinition;
//                if (moduleDefinition != null)
//                {
//                    return Assembly.LoadFile(moduleDefinition.FullyQualifiedName);
//                }
//            }

//            throw new NotImplementedException();
//        }

//        public static Type GetTypeBy(Assembly assem, string name, bool ignore)
//        {
//            var a = AppDomain.CurrentDomain.GetAssemblies();
//            foreach (var assembly in a)
//            {
//                var t = assembly.GetType(name);
//                if (t != null)
//                {
//                    return t;
//                }
//            }

//            return null;
//        }

//        public static Assembly GetAssemblyByName(AssemblyName assemblyName)
//        {
//            return null;
//        }

//        public static Assembly GetSystemAssembly(this ModuleDefinition module)
//        {
//            return Assembly.LoadFile(module.FullyQualifiedName);
//        }

//        #endregion



//        #region System.Type to Modo.Cecil

//        public static TypeDefinition GetTypeDefinition(this Type typ)
//        {
//            var module = ModuleDefinition.ReadModule(typ.Assembly.Location);
//            var im = module.Import(typ);
//            var td = im.Resolve();

//            return td;
//        }
//        #endregion


//        #region System.Type Method

//        public static object Wykonaj(this MethodInfo methodInfo, object instancja, params object[] parameters)
//        {
//            if (methodInfo.IsGenericMethod)
//            {
//                //mamy generyczną funkcję
//                if (instancja == null)
//                {
//                    //generyczną i statyczną
//                    var param = parameters[0];
//                    var paramType = param.GetType();
//                    if (paramType.IsConstructedGenericType == false && methodInfo.IsGenericMethodDefinition == true)
//                    {
//                        //generyczna z nie rozwiązanym parametrem generycznym
//                        var genericType = paramType.GenericTypeArguments;
//                        methodInfo = methodInfo.MakeGenericMethod(genericType[0]);
//                    }
//                }
//            }
//            var ret = methodInfo.Invoke(instancja, parameters);
//            return ret;
//        }

//        public static MethodInfo GetMethod(this Type typ, MethodReference methodReference)
//        {
//            var genericMethod = methodReference as GenericInstanceMethod;
//            if (genericMethod != null)
//            {
//                //mamy generyczną metode (która ma parametr generyczny)
//                var methodDefinition = methodReference.Resolve();
//                var metody = typ.GetMethods().Where(m => m.Name == methodDefinition.Name);
//                var methoda = metody.FirstOrDefault(m => m.CzyTakieSameMetody(methodDefinition));
//                if (genericMethod.GenericArguments.Count > 0) //jeśli jest parametr generyczny to zamieniam go z T
//                {
//                    var genericTypesToResolveMethod = new List<Type>();
//                    //sprawdzam czy metoda jest konkretnego typu czy T i wymaga rozwinięcia
//                    foreach (var genericArgument in genericMethod.GenericArguments)
//                    {
//                        if (genericArgument.IsGenericParameter)
//                        {
//                            genericTypesToResolveMethod.Add(typ.GenericTypeArguments[0]);
//                        }
//                        else
//                        {
//                            genericTypesToResolveMethod.Add(genericArgument.GetSystemType());
//                        }
//                    }
//                    methoda = methoda.MakeGenericMethod(
//                        genericTypesToResolveMethod.ToArray()
//                        );
//                }
//                return methoda;
//            }
//            else
//            {
//                //mamy metodę zwykłą, 
//                //ale może ona zwracać parametry generyczne T
//                if (methodReference.ContainsGenericParameter == false)
//                {
//                    //mamy metodę zwracającą zwykłe typy
//                    var methodDefinition = methodReference.Resolve();
//                    //metody o danej nazwie
//                    var metody = typ.GetMethods().Where(m => m.Name == methodDefinition.Name);
//                    return metody.FirstOrDefault(m => m.CzyTakieSameMetody(methodDefinition));
//                }
//                else
//                {
//                    //metoda zwraca generyka którego trzeba konstruować
//                    var methodDefinition = methodReference.Resolve();
//                    //methodDefinition.ReturnType = typ.GenericTypeArguments[0].GetTypeDefinition();
//                    //metody o danej nazwie
//                    var metody = typ.GetMethods().Where(m => m.Name == methodDefinition.Name).ToList();
//                    var metoda = metody.FirstOrDefault(m => m.CzyTakieSameMetody(methodDefinition));
//                    if (metoda.IsGenericMethodDefinition && typ.GenericTypeArguments.Length == 1)
//                    {
//                        metoda = metoda.MakeGenericMethod(typ.GenericTypeArguments[0]);
//                    }
//                    return metoda;
//                }
//            }
//        }

//        public static bool CzyTakieSameMetody(this MethodInfo methodInfo, MethodReference methodReference)
//        {
//            //czy metody generyczne
//            if (methodInfo.IsGenericMethod != methodReference.HasGenericParameters)
//            {
//                return false;
//            }

//            var parametryMetody = methodInfo.GetParameters();
//            if (parametryMetody.Length != methodReference.Parameters.Count)
//            {
//                return false;
//            }

//            //mamy taką samą ilość parametrów - sprawdzam ich typy
//            var monoParam = methodReference.Parameters;
//            for (int i = 0; i < parametryMetody.Length; i++)
//            {
//                if (parametryMetody[i].ParameterType.IsGenericParameter == true &&
//                    monoParam[i].ParameterType.IsGenericParameter == true)
//                {
//                    //mamy dwa generyczne parametry - sprawdzam ich nazwy
//                    if (parametryMetody[i].ParameterType.Name != monoParam[i].ParameterType.Name)
//                    {
//                        return false;
//                    }
//                }
//                else if (parametryMetody[i].ParameterType.IsGenericType == true &&
//                             monoParam[i].ParameterType.IsGenericInstance == true)
//                {
//                    //mamy generyczne typy
//                    if (parametryMetody[i].ParameterType.Name != monoParam[i].ParameterType.Name)
//                    {
//                        return false;
//                    }
//                }
//                else
//                {
//                    if (parametryMetody[i].ParameterType.IsGenericParameter == false &&
//                        monoParam[i].ParameterType.IsGenericParameter == false)
//                    {
//                        //mamy zwykłe parametry
//                        if (parametryMetody[i].ParameterType != monoParam[i].ParameterType.GetSystemType())
//                        {
//                            return false;
//                        }
//                    }
//                    else
//                    {
//                        //mamy różne parametry (generyczny i zwykły)
//                        return false;
//                    }
//                }
//            }


//            //Czy takie same typy zwraca
//            if (methodInfo.ReturnType.IsGenericParameter == true &&
//                methodReference.ReturnType.IsGenericParameter == true)
//            {
//                //zwracany jest prametr generyczny np. T, TSource...
//                //porównuję nazwy
//                if (methodInfo.ReturnType.Name != methodReference.ReturnType.Name)
//                {
//                    return false;
//                }
//            }
//            else if (methodInfo.ReturnType.IsGenericType == methodReference.ReturnType.IsGenericInstance)
//            {
//                if (methodInfo.ReturnType.IsGenericType == false &&
//                    methodReference.ReturnType.IsGenericInstance == false)
//                {
//                    //mamy zwykłe parametry
//                    if (methodInfo.ReturnType != methodReference.ReturnType.GetSystemType())
//                    {
//                        return false;
//                    }
//                }
//                else
//                {
//                    //mamy dwa generyczne parametry - sprawdzam ich nazwy
//                    if (methodInfo.ReturnType.Name != methodReference.ReturnType.Name)
//                    {
//                        return false;
//                    }
//                }
//            }
//            else
//            {
//                //mamy generyczny typ i zwykły
//                return false;
//            }

//            return true;
//        }

//        #endregion

//    }
//}