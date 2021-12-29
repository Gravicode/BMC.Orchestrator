using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravicode.ExpressionCode
{
    public static class ExpressionExtensions
    {
        // Converts a character to lower-case for the default culture.
        public static char ToLower(this char c)
        {
            return ToLower(c, CultureInfo.CurrentCulture);
        }
        // Converts a character to lower-case for the specified culture.
         // <;<;Not fully implemented>;>;
        public static char ToLower(char c, CultureInfo culture)
        {
            if (culture == null)
                throw new ArgumentNullException("culture");
            Contract.EndContractBlock();
            return culture.TextInfo.ToLower(c);
        }

    }
}
