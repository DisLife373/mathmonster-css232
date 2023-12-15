using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBMath
{
    public interface IMathParser
    {
        double Parse(string expression, bool isRadians = true);
    }
}
