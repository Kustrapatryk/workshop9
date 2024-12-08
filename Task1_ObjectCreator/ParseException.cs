using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1_ObjectCreator
{
    internal class ParseException(string message) : Exception(message);
}
