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
using Mono.Reflection;
using Cvl.ApplicationServer.Logs;

namespace Cvl.VirtualMachine
{
    [Interpret]
    public class ProcesAction
    {
        public Action Action { get; set; }

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


    // wirutalna maszyna .net - base on https://www.ecma-international.org/wp-content/uploads/ECMA-335_6th_edition_june_2012.pdf
    public class VirtualMachine
    {
        public VirtualMachine()
        {
            Thread = new ThreadOfControl() { WirtualnaMaszyna = this };
        }

        public ThreadOfControl Thread { get; set; }



        //public bool CzyWykonywacInstrukcje { get; private set; } = true;



        public InstructionsFactory instructionsFactory = new InstructionsFactory();
        public long BreakpointIterationNumber { get; set; } = -1;

        public void WykonajMetode()
        {

        }

        /// <summary>
        /// Wykonuje pojedyńczą instrukcję/krok
        /// </summary>
        public void Step()
        {
            Thread.Step();
        }

        public void StepOver()
        {
            Thread.StepOver();
        }

        /// <summary>
        /// Wykonuje kod do podanego numeru iteracji
        /// </summary>
        /// <param name="iterationNumber"></param>
        public void ExecuteToIteration(long iterationNumber)
        {
            Thread.ExecuteToIteration(iterationNumber);
        }

        public void Execute()
        {
            Thread.Execute();
        }

        private void start(string nazwaMetody, params object[] parametety)
        {
            var process = parametety.First();
            var typ = process.GetType();
            var startMethod = typ.GetMethod(nazwaMetody);//typDef.Methods.FirstOrDefault(mm => mm.Name == nazwaMetodyStartu);

            start(startMethod, parametety);
        }

        

        private void sprawdziInstrukcje(MethodInfo methodInfo, List<Instruction> brakujaceInstrukcje, int level)
        {
            if (level <= 0 || methodInfo == null || methodInfo.IsAbstract)
            { return; }

            var instructions = methodInfo.GetInstructions();
            foreach (var instruction in instructions)
            {
                var i = instructionsFactory.UtworzInstrukcje(instruction, this);
                if (i == null)
                {
                    brakujaceInstrukcje.Add(instruction);
                }

                if (i is Call call)
                {
                    var mi = instruction.Operand as MethodInfo;
                    sprawdziInstrukcje(mi, brakujaceInstrukcje, level - 1);
                }

            }
        }

        private void prepereToExecution(MethodInfo methodInfo, params object[] parametety)
        {
            var process = parametety.First();
            var typ = process.GetType();
            var m = new MethodState(methodInfo, this, process);
            m.Logger = Logger;
            Thread.PushAktualnaMetode(m);

            var brakujaceInstrukcje = new List<Instruction>();
            //sprawdziInstrukcje(methodInfo, brakujaceInstrukcje, 1);

            if (brakujaceInstrukcje.Any())
            {
                throw new Exception($"Brak instrukcji: {string.Join(",", brakujaceInstrukcje.Select(x => x.ToString()))}");
            }

            m.WczytajInstrukcje();
            //HardwareContext.Stos.PushObject(process);
            m.LocalArguments.Wczytaj(parametety);
        }

        private void start(MethodInfo methodInfo, params object[] parametety)
        {
            prepereToExecution(methodInfo, parametety);

            //m.Instrukcje = new List<InstructionBase>() { new CallStart(m) { HardwareContext = this.HardwareContext } };
            Thread.Execute();
        }

        public VirtualMachineResult<T> Resume<T>(object hibernateResumeParameter = null)
        {
            Thread.AktualnaMetoda.PushObject(hibernateResumeParameter);
            Thread.AktualnaMetoda.NumerWykonywanejInstrukcji++;
            Thread.Status = VirtualMachineState.Executing;
            Thread.AktualnaMetoda.CzyWykonywacInstrukcje = true;

            Thread.Execute();
            if (Thread.Status == VirtualMachineState.Hibernated)
            {
                return new VirtualMachineResult<T>() { State = Thread.Status };
            }

            var ret = (T)Thread.Result;
            var result = new VirtualMachineResult<T>() { State = Thread.Status, Result = ret };
            return result;
        }

        public VirtualMachineResult<T> Start<T>(string nazwaMetody, params object[] parametet)
        {
            Thread = new ThreadOfControl() { WirtualnaMaszyna = this };
            start(nazwaMetody, parametet);
            if (Thread.Status == VirtualMachineState.Hibernated)
            {
                return new VirtualMachineResult<T>() { State = Thread.Status };
            }

            T ret;

            if (typeof(T) == typeof(bool))
            {
                dynamic r = Thread.Result;
                ret = Convert.ToBoolean(r);
            }
            else
            {
                ret = (T)Thread.Result;
            }
            var result = new VirtualMachineResult<T>() { State = Thread.Status, Result = ret };
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
            start(p.Method, p.Target);
        }

        public void ActionToExecute(Action p)
        {
            var proces = new ProcesAction();
            proces.Action = p;
            prepereToExecution(p.Method, p.Target);
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
            Thread.AktualnaMetoda.CzyWykonywacInstrukcje = false;
            Thread.Status = VirtualMachineState.Hibernated;
            Thread.HibernateParams = parameters;
        }

        /// <summary>
        /// Metoda służy do kończenia wykonywania wirtualnej maszyny
        /// Wywoływana z procesu który interpretowany jest przez wirtualną maszynę
        /// </summary>
        public void EndProcessVirtualMachine()
        {
            Thread.AktualnaMetoda.CzyWykonywacInstrukcje = false;
            Thread.Status = VirtualMachineState.Executed;
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

            if (string.IsNullOrEmpty(InterpreteFullNameTypes) == false)
            {
                var namespaces = InterpreteFullNameTypes.Split(';');
                if (namespaces.Any(x => mr.DeclaringType.FullName.Contains(x)))
                {
                    return false; //interpretujrmy
                }
            }

            return true; //w innym wypadku wykonujemy metody
        }



        #endregion

        #region Eventy logów

        public ILogMonitor LogMonitor { get; set; }
        public Logger Logger { get; set; }

        private int callLevel = 0;
        internal void EventRet(object ret = null)
        {
            callLevel--;
            LogMonitor?.EventRet(ret, Thread.NumerIteracji);

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

            LogMonitor?.EventCall(method, parameters, callLevel, Thread.NumerIteracji);



        }

        internal void EventThrowException(Exception rzuconyWyjatek)
        {
            var text = $"Wyjątek rzucony {rzuconyWyjatek.Message}";
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
            return Thread.HibernateParams;
        }

        #endregion
    }
}
