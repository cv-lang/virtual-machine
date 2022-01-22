using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace Cvl.VirtualMachine.UnitTest.Proces.Model
{
    public class Logger 
    { 
        public string LogData { get; set; }
    }
    public class ApplicationMonitor
    {
        public Logger StartLogs(
      Expression<Func<object>> param1 = null,
      Expression<Func<object>> param2 = null,
      Expression<Func<object>> param3 = null,
      object param4 = null,
      object param5 = null,
      [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0,
      string message = "",
      string externalId = null)
        {
            Logger logger = new Logger();
            var sb = new StringBuilder();
            sb.AppendLine(externalId);
            sb.AppendLine(memberName);
            sb.AppendLine(sourceFilePath);
            sb.AppendLine(sourceLineNumber.ToString());
            sb.AppendLine(message);
            sb.AppendLine(param4?.ToString());
            sb.AppendLine(param5?.ToString());
            
            return logger;
        }

    }
}
