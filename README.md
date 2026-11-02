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
var serializedVMXml = VirtualMachine.SerializeVirtualMachine(vm);
object retFromSerializedVM = "";

while (vm.Status == VirtualMachineState.Hibernated)
{
    vm = VirtualMachine.DeserializeVirtualMachine(serializedVMXml);
    retFromSerializedVM = vm.Resume();
    serializedVMXml = vm.Serialize();
}

var inProcProces = new HibernateWorkflowSimple() { InputParametr = 10 };
var retInProcProces = inProcProces.Start();
Assert.AreEqual(retInProcProces, retFromSerializedVM);
```

## Saved state

`VirtualMachine.SerializeVirtualMachine` writes the whole object graph field by field, including private fields, and keeps shared references. The result is JSON with a format version, read back with `VirtualMachine.DeserializeVirtualMachine`. It can be stored in a PostgreSQL `jsonb` column: it never contains `\u0000` and does not depend on the order of keys.

- Fields marked `[NonSerialized]` are skipped. Restore them in `IDeserializedObject.AfterDeserialization`, which runs after the whole graph is loaded. Constructors are not called during restore.
- A type that holds resources (`Task`, `Stream`, `HttpClient`...) stops the save with `StateSerializationException` naming the path to the field, so state is never lost silently.
- Saved state is trusted data. Do not load it from an untrusted source.
- Static fields are not part of the state.

Not supported inside interpreted methods:

- `async` methods with `[Interpret]`. Call them from code without the attribute and wait for the result before hibernating.
- `Span` and `params ReadOnlySpan`, for example `string.Join("|", a, b, c, d, e)`. Pass an array instead.

Known limitations:

- `VirtualMachine.Hibernate` called from a method that is not interpreted does nothing and execution continues. Mark every method on the path to it with `[Interpret]`.
- Changing the code of a process before resuming is not detected yet.
- In Release builds, `if (o != null)` on an `object` holding a boxed `0` or `false` is treated as null.

## Tests

Run `dotnet test` in `Cvl.VirtualMachine.UnitTest`, in both Debug and Release, because the compiler emits different IL for each.

Setting `CVL_VM_ZAPIS_CO_KROK=1` makes `StartTestExecution` save and restore the whole machine after every instruction, so each instruction becomes a possible hibernation point.

## NuGet 'Cvl.VirtualMachine'
PM> Install-Package Cvl.VirtualMachine

[NuGet package Cvl.VirtualMachine](https://www.nuget.org/packages/Cvl.VirtualMachine/)

## Dependencies

[Mono.Reflection](https://github.com/jbevain/mono.reflection)

