using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace BookKeeping.App.Helpers
{
   public class ExpressionHelper
    {
       private readonly Regex filterExpressionRegex = new Regex(@"^(\s*)(?<field>\w+)(\s*)(?<operator>\W+)(\s*)(?<value>.+)", RegexOptions.Compiled);

       public Func<T, bool> GetFilter<T>(string expression)
       {
           if (!string.IsNullOrWhiteSpace(expression) && filterExpressionRegex.IsMatch(expression))
           {
               var groups = filterExpressionRegex.Match(expression).Groups;
               return GetFilter<T>(groups["field"].Value.Trim(),
                   groups["operator"].Value.Trim(),
                   groups["value"].Value.Trim());
           }
           return null;
       }

       public Func<T, bool> GetFilter<T>(string fieldStr, string operatorStr, string valueStr)
       {
           var parameter = Expression.Parameter(typeof(T));
           var field = Expression.Property(parameter, fieldStr);
           Expression value = null;
           decimal tempDecimal;
           bool tempBoolean;
           if (decimal.TryParse(valueStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CurrentCulture, out tempDecimal))
           {
               value = Expression.Constant(tempDecimal);
           }
           else if (bool.TryParse(valueStr, out tempBoolean))
           {
               value = Expression.Constant(tempBoolean);
           }
           else value = Expression.Constant(valueStr);

           Expression @operator = null;
           switch (operatorStr.Replace(" ", string.Empty))
           {
               case "<":
                   @operator = Expression.LessThan(field, value);
                   break;

               case "<=":
                   @operator = Expression.LessThanOrEqual(field, value);
                   break;

               case "=":
                   @operator = Expression.Equal(field, value);
                   break;

               case ">":
                   @operator = Expression.GreaterThan(field, value);
                   break;

               case ">=":
                   @operator = Expression.GreaterThanOrEqual(field, value);
                   break;
                   throw new NotSupportedException();
           }
           return Expression.Lambda<Func<T, bool>>(@operator, parameter).Compile();
       }
    }
}
