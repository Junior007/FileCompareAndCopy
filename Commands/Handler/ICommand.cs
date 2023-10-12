using FileCompareAndCopy.Commands.Request;
using FileCompareAndCopy.Commands.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCompareAndCopy.Commands.Handler
{
    internal interface ICommand
    {
        IResponse Execute(IRequest request);
    }
}
