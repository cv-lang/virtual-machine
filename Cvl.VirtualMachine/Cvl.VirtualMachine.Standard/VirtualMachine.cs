using Cvl.VirtualMachine.Core;
using Cvl.VirtualMachine.Core.Attributes;
using Cvl.VirtualMachine.Instructions;
using Cvl.VirtualMachine.Instructions.Calls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cvl.VirtualMachine
{
    [Interpret]
    public class ProcesAction
    {
        public Action Action { get;  set; }

        public void Start()
        {
            Action.Invoke();
        }
    }

    public class VirtualMachine
    {
        public VirtualMachine()
        {
            HardwareContext = new HardwareContext() { WirtualnaMaszyna = this };
        }

        public HardwareContext HardwareContext { get; set; }

        

        public bool CzyWykonywacInstrukcje { get; private set; } = true;

        

        public InstructionsFactory instructionsFactory = new InstructionsFactory();
        public long BreakpointIterationNumber { get; set; } = -1;

        public void WykonajMetode()
        {

        }
        public void Start(string nazwaMetody, params object[] parametety)
        {
            var process = parametety.First();
            var typ = process.GetType();
            var startMethod = typ.GetMethod(nazwaMetody);//typDef.Methods.FirstOrDefault(mm => mm.Name == nazwaMetodyStartu);
            
            Start(startMethod, parametety);
        }

        

        public void Start(MethodInfo methodInfo, params object[] parametety)
        {
            var process = parametety.First();
            var typ = process.GetType();
            var m = new Metoda(methodInfo, this, process);
            m.WczytajInstrukcje();
            HardwareContext.AktualnaMetoda = m;
            //HardwareContext.Stos.PushObject(process);
            m.LocalArguments.Wczytaj(parametety);
            

            //m.Instrukcje = new List<InstructionBase>() { new CallStart(m) { HardwareContext = this.HardwareContext } };
            HardwareContext.Execute();
        }

        public T Start<T>(string nazwaMetody, params object[] parametet)
        {
            HardwareContext = new HardwareContext() { WirtualnaMaszyna = this };
            Start(nazwaMetody, parametet);
            var ret = HardwareContext.PopObject();
            return (T)ret;
        }

        public void Start(Action p)
        {
            var proces = new ProcesAction();
            proces.Action = p;
            Start(p.Method, p.Target);
        }

        public void WalidujMetodyObiektu(object instancjaObiektu)
        {
            var typ = instancjaObiektu.GetType();
            //var foldre = typ.Assembly.Location;
            //var module = Mono.Cecil.ModuleDefinition.ReadModule(foldre);
            //var typy = module.GetTypes();

            //var typDef = typy.First(t => t.FullName == typ.FullName);
            //var metody = typDef.Methods;
            foreach (var metoda in typ.GetMethods())
            {
                var m = new Metoda(metoda, this, instancjaObiektu);
                var i = m.PobierzInstrukcjeMetody(); //pobierma instrukcje metody - jeśli brakuje jakiejś instrukcji rzuca wyjątek
            }
        }

        #region Hibernation and restoring

        /// <summary>
        /// Metoda służy do hibernowania wirtualnej maszyny
        /// Wywoływana z procesu który interpretowany jest przez wirtualną maszynę
        /// </summary>
        public void HibernateVirtualMachine()
        {
            CzyWykonywacInstrukcje = false;
            HardwareContext.Status = VirtualMachineState.Hibernated;
        }

        /// <summary>
        /// Metoda służy do kończenia wykonywania wirtualnej maszyny
        /// Wywoływana z procesu który interpretowany jest przez wirtualną maszynę
        /// </summary>
        public void EndProcessVirtualMachine()
        {
            CzyWykonywacInstrukcje = false;
            HardwareContext.Status = VirtualMachineState.Executed;
        }

        /// <summary>
        /// Metoda służy do hibernowania wirtualnej maszyny
        /// Wywoływana z procesu który interpretowany jest przez wirtualną maszynę
        /// </summary>
        public static void Hibernate()
        {

        }

        /// <summary>
        /// Metoda służy do kończenia wykonywania wirtualnej maszyny
        /// Wywoływana z procesu który interpretowany jest przez wirtualną maszynę
        /// </summary>
        public static void EndProcess()
        {

        }



        #endregion

        #region Interprete choce
        public string InterpreteFullNameTypes { get; set; }

        internal bool CzyWykonacCzyInterpretowac(MethodBase mr)
        {
            var czyKlasaMaAtrybut = mr.DeclaringType.CustomAttributes.Any(a => a.AttributeType.FullName == typeof(InterpretAttribute).FullName);
            var czyMetodaMaAtrybut = mr.CustomAttributes.Any(a => a.AttributeType.FullName == typeof(InterpretAttribute).FullName);


            if (czyKlasaMaAtrybut || czyMetodaMaAtrybut)
            {
                return false; //interpertujemy
            }

            if(string.IsNullOrEmpty(InterpreteFullNameTypes) == false)
            {
                var namespaces = InterpreteFullNameTypes.Split(';');
                if( namespaces.Any(x=> mr.DeclaringType.FullName.Contains(x)) )
                {
                    return false; //interpretujrmy
                }
            }

            return true; //w innym wypadku wykonujemy metody
        }



        #endregion

        #region Eventy logów

        private int callLevel = 0;
        internal void EventRet(object ret=null)
        {
            callLevel--;
            var text = $".. Ret: ''{ret ?? "null"}''";

            if (ret is ICollection collection)
            {
                text += $" count:{collection.Count}";
            }

            File.AppendAllText("wm-log.txt", text+ Environment.NewLine);
            Console.WriteLine(text);
        }

        internal void EventCall(MethodBase method, List<object> parameters)
        {
            callLevel++;
            var n = new StringBuilder();
            for (int i = 0; i < callLevel; i++)
            {
                n.Append(" ");
            }

            string para = "";
            foreach (object parameter in parameters)
            {
                var p = parameter?.ToString() ?? "null";
                if (p.Length > 100)
                {
                    p = p.Substring(0, 99);
                }

                para += p + ", ";
            }

            n.Append($"{method.Name} ({para}) iter:{HardwareContext.NumerIteracji} ");

            var text = n.ToString();
            File.AppendAllText("wm-log.txt", Environment.NewLine + text);
            Console.Write(text);
        }

        internal void EventThrowException(Exception rzuconyWyjatek)
        {
            var text= $"Wyjątek rzucony {rzuconyWyjatek.Message}";
            File.AppendAllText("wm-log.txt", Environment.NewLine + text + Environment.NewLine);
            Console.WriteLine(text);
        }

        internal void EventHandleException(string v)
        {
            var text = $"ExceptionHandler {v}";
            File.AppendAllText("wm-log.txt", Environment.NewLine + text + Environment.NewLine);
            Console.WriteLine(text);
        }

        #endregion
    }
}
