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
using Cvl.VirtualMachine.Core.Tools;

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

    public class VirtualMachineResult<T>
    {
        public VirtualMachineState State { get; set; }
        public T Result { get; set; }
    }

    public class VirtualMachine
    {
        public VirtualMachine()
        {
            HardwareContext = new HardwareContext() { WirtualnaMaszyna = this };
        }

        public HardwareContext HardwareContext { get; set; }

        

        //public bool CzyWykonywacInstrukcje { get; private set; } = true;

        

        public InstructionsFactory instructionsFactory = new InstructionsFactory();
        public long BreakpointIterationNumber { get; set; } = -1;

        public void WykonajMetode()
        {

        }
        private void start(string nazwaMetody,  params object[] parametety)
        {
            var process = parametety.First();
            var typ = process.GetType();
            var startMethod = typ.GetMethod(nazwaMetody);//typDef.Methods.FirstOrDefault(mm => mm.Name == nazwaMetodyStartu);
            
            start(startMethod, parametety);
        }

        

        private void start(MethodInfo methodInfo, params object[] parametety)
        {
            var process = parametety.First();
            var typ = process.GetType();
            var m = new MethodState(methodInfo, this, process);
            m.WczytajInstrukcje();
            HardwareContext.AktualnaMetoda = m;
            //HardwareContext.Stos.PushObject(process);
            m.LocalArguments.Wczytaj(parametety);
            

            //m.Instrukcje = new List<InstructionBase>() { new CallStart(m) { HardwareContext = this.HardwareContext } };
            HardwareContext.Execute();
        }

        public VirtualMachineResult<T> Resume<T>(object hibernateResumeParameter = null)
        {
            HardwareContext.PushObject(hibernateResumeParameter);
            HardwareContext.AktualnaMetoda.NumerWykonywanejInstrukcji++;
            HardwareContext.Status = VirtualMachineState.Executing;
            HardwareContext.CzyWykonywacInstrukcje = true;

            HardwareContext.Execute();
            if (HardwareContext.Status == VirtualMachineState.Hibernated)
            {
                return new VirtualMachineResult<T>() { State = HardwareContext.Status };
            }

            var ret = (T)HardwareContext.PopObject();
            var result = new VirtualMachineResult<T>() { State = HardwareContext.Status, Result = ret };
            return result;
        }

        public VirtualMachineResult<T> Start<T>(string nazwaMetody, params object[] parametet)
        {
            HardwareContext = new HardwareContext() { WirtualnaMaszyna = this };
            start(nazwaMetody, parametet);
            if (HardwareContext.Status == VirtualMachineState.Hibernated)
            {
                return new VirtualMachineResult<T>() { State = HardwareContext.Status };
            }

            var ret = (T)HardwareContext.PopObject();
            var result = new VirtualMachineResult<T>() { State = HardwareContext.Status, Result = ret };
            return result;
        }

        public T StartTestExecution<T>(string nazwaMetody, params object[] parametet)
        {
            var res = Start<T>(nazwaMetody, parametet: parametet);
            return res.Result;
        }

        public void Start(Action p)
        {
            var proces = new ProcesAction();
            proces.Action = p;
            start(p.Method, true, p.Target);
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
                var m = new MethodState(metoda, this, instancjaObiektu);
                var i = m.PobierzInstrukcjeMetody(); //pobierma instrukcje metody - jeśli brakuje jakiejś instrukcji rzuca wyjątek
            }
        }

        #region Hibernation and restoring

        /// <summary>
        /// Metoda służy do hibernowania wirtualnej maszyny
        /// Wywoływana z procesu który interpretowany jest przez wirtualną maszynę
        /// </summary>
        public void HibernateVirtualMachine(object[] parameters)
        {
            HardwareContext.CzyWykonywacInstrukcje = false;
            HardwareContext.Status = VirtualMachineState.Hibernated;
            HardwareContext.HibernateParams = parameters;
        }

        /// <summary>
        /// Metoda służy do kończenia wykonywania wirtualnej maszyny
        /// Wywoływana z procesu który interpretowany jest przez wirtualną maszynę
        /// </summary>
        public void EndProcessVirtualMachine()
        {
            HardwareContext.CzyWykonywacInstrukcje = false;
            HardwareContext.Status = VirtualMachineState.Executed;
        }

        /// <summary>
        /// Metoda służy do hibernowania wirtualnej maszyny
        /// Wywoływana z procesu który interpretowany jest przez wirtualną maszynę
        /// </summary>
        public static object Hibernate(params object[] parameters)
        {
            return null;
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

        public ILogMonitor LogMonitor { get; set; }

        private int callLevel = 0;
        internal void EventRet(object ret=null)
        {
            callLevel--;
            LogMonitor?.EventRet(ret, HardwareContext.NumerIteracji);

            //var text = $".. Ret: ''{ret ?? "null"}''";

            //if (ret is ICollection collection)
            //{
            //    text += $" count:{collection.Count}";
            //}

            //File.AppendAllText("wm-log.txt", text+ Environment.NewLine);
            //Console.WriteLine(text);
        }

        internal void EventCall(MethodBase method, List<object> parameters)
        {
            callLevel++;

            LogMonitor?.EventCall(method, parameters, callLevel, HardwareContext.NumerIteracji);


            
        }

        internal void EventThrowException(Exception rzuconyWyjatek)
        {
            var text= $"Wyjątek rzucony {rzuconyWyjatek.Message}";
            //File.AppendAllText("wm-log.txt", Environment.NewLine + text + Environment.NewLine);
            Console.WriteLine(text);
        }

        internal void EventHandleException(string v)
        {
            var text = $"ExceptionHandler {v}";
            //File.AppendAllText("wm-log.txt", Environment.NewLine + text + Environment.NewLine);
            Console.WriteLine(text);
        }

        public object[] GetHibernateParams()
        {
            return HardwareContext.HibernateParams;
        }

        #endregion
    }
}
