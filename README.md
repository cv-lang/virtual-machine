# .NET virtual-machine
<img width="150" src="https://github.com/cv-lang/virtual-machine/blob/master/Cvl.VirtualMachine/Cvl.VirtualMachine.Standard/Icons/logo-virtualmachine-m.png?raw=true" alt="virtualmachine-logo"/>

.NET Virtual machine written in C #. It allows you to perform the compiled .net code (IL). The code is executed instruction by instruction. (Virtual machine emulates all instructions from IL).
Execute code can be hibernated and then restarted (even on another machine after serialization).


## Allow to:
- Application checkpointing - allowing to stop execution (suspending/hibernation) save execution state to disk and later load state from disk and resume execution.
- Hibernation and process recovery
- Thread serialization

## Example

```CSharp
public class HibernateWorkflow
{
    public int InputParametr { get; set; }
    public string Start()
    {
        //do some work
        for (int i = 0; i < 10; i++)
        {
            SomeInterpretedFunction();
        }

        //after restore (in another thread/computer)
        //do some work
        for (int i = 0; i < 10; i++)
        {
            SomeInterpretedFunction();
        }

        return "Helow World " + InputParametr;
    }

    [Interpret]
    public void SomeInterpretedFunction()
    {
        //do some work
        InputParametr++;

        //hibernate executed method
        VirtualMachine.VirtualMachine.Hibernate();
    }
}
```

launching code
```CSharp
var proces = new HibernateWorkflowSimple() { InputParametr = 10 };
var vm = new VirtualMachine.VirtualMachine();
vm.Start(proces);
var serializedVMXml = vm.Serialize();
object retFromSerializedVM = "";

while (vm.Status == VirtualMachineState.Hibernated)
{
    vm = VirtualMachine.VirtualMachine.Deserialize(serializedVMXml);
    retFromSerializedVM = vm.Resume();
    serializedVMXml = vm.Serialize();
}

var inProcProces = new HibernateWorkflowSimple() { InputParametr = 10 };
var retInProcProces = inProcProces.Start();
Assert.AreEqual(retInProcProces, retFromSerializedVM);
```

## NuGet 'Cvl.VirtualMachine'
PM> Install-Package Cvl.VirtualMachine -Version 0.9.1

[NuGet package Cvl.VirtualMachine](https://www.nuget.org/packages/Cvl.VirtualMachine/)

## Dependencies

[Mono.Reflection](https://github.com/jbevain/mono.reflection)

[SharpSerializer](http://sharpserializer.com/en/index.html)

