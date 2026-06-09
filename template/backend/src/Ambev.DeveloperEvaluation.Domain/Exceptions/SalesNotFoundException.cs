using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Exceptions;
public class SalesNotFoundException : Exception
{
    public SalesNotFoundException(string message) : base(message)
    {
    }

    public SalesNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
