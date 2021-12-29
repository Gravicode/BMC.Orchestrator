using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSQL;
using TSQL.Statements;
using TSQL.Tokens;

namespace BMC.StreamProcessor
{
    public class ParseSQLSample
    {
        public void Run()
        {
			{
				TSQLSelectStatement select = TSQLStatementReader.ParseStatements(@"
					SELECT OrderDateKey, SUM(SalesAmount) AS TotalSales
					FROM 
					WHERE OrderDateKey > 5
					GROUP BY OrderDateKey
					HAVING OrderDateKey > 20010000
					ORDER BY OrderDateKey;")[0] as TSQLSelectStatement;

				Console.WriteLine("SELECT:");
				foreach (TSQLToken token in select.Select.Tokens)
				{
					Console.WriteLine("\ttype: " + token.Type.ToString() + ", value: " + token.Text);
				}

				if (select.From != null)
				{
					Console.WriteLine("FROM:");
					foreach (TSQLToken token in select.From.Tokens)
					{
						Console.WriteLine("\ttype: " + token.Type.ToString() + ", value: " + token.Text);
					}
				}

				if (select.Where != null)
				{
					Console.WriteLine("WHERE:");
					foreach (TSQLToken token in select.Where.Tokens)
					{
						Console.WriteLine("\ttype: " + token.Type.ToString() + ", value: " + token.Text);
					}
				}

				if (select.GroupBy != null)
				{
					Console.WriteLine("GROUP BY:");
					foreach (TSQLToken token in select.GroupBy.Tokens)
					{
						Console.WriteLine("\ttype: " + token.Type.ToString() + ", value: " + token.Text);
					}
				}

				if (select.Having != null)
				{
					Console.WriteLine("HAVING:");
					foreach (TSQLToken token in select.Having.Tokens)
					{
						Console.WriteLine("\ttype: " + token.Type.ToString() + ", value: " + token.Text);
					}
				}

				if (select.OrderBy != null)
				{
					Console.WriteLine("ORDER BY:");
					foreach (TSQLToken token in select.OrderBy.Tokens)
					{
						Console.WriteLine("\ttype: " + token.Type.ToString() + ", value: " + token.Text);
					}
				}
			}
   //         {
			//	foreach (TSQLToken token in TSQLTokenizer.ParseTokens(@"
			//		CREATE VIEW [HumanResources].[vEmployee] 
			//		AS 
			//		SELECT 
			//			e.[BusinessEntityID]
			//			,p.[Title]
			//			,p.[FirstName]
			//			,p.[MiddleName]
			//			,p.[LastName]
			//			,p.[Suffix]
			//			,e.[JobTitle]  
			//			,pp.[PhoneNumber]
			//			,pnt.[Name] AS [PhoneNumberType]
			//			,ea.[EmailAddress]
			//			,p.[EmailPromotion]
			//			,a.[AddressLine1]
			//			,a.[AddressLine2]
			//			,a.[City]
			//			,sp.[Name] AS [StateProvinceName] 
			//			,a.[PostalCode]
			//			,cr.[Name] AS [CountryRegionName] 
			//			,p.[AdditionalContactInfo]
			//		FROM [HumanResources].[Employee] e
			//			INNER JOIN [Person].[Person] p
			//			ON p.[BusinessEntityID] = e.[BusinessEntityID]
			//			INNER JOIN [Person].[BusinessEntityAddress] bea 
			//			ON bea.[BusinessEntityID] = e.[BusinessEntityID] 
			//			INNER JOIN [Person].[Address] a 
			//			ON a.[AddressID] = bea.[AddressID]
			//			INNER JOIN [Person].[StateProvince] sp 
			//			ON sp.[StateProvinceID] = a.[StateProvinceID]
			//			INNER JOIN [Person].[CountryRegion] cr 
			//			ON cr.[CountryRegionCode] = sp.[CountryRegionCode]
			//			LEFT OUTER JOIN [Person].[PersonPhone] pp
			//			ON pp.BusinessEntityID = p.[BusinessEntityID]
			//			LEFT OUTER JOIN [Person].[PhoneNumberType] pnt
			//			ON pp.[PhoneNumberTypeID] = pnt.[PhoneNumberTypeID]
			//			LEFT OUTER JOIN [Person].[EmailAddress] ea
			//			ON p.[BusinessEntityID] = ea.[BusinessEntityID];"))
			//	{
			//		Console.WriteLine("type: " + token.Type.ToString() + ", value: " + token.Text);
			//	}

			//}
		}
    }
}
