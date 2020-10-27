using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Konata.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var element in GetAllTests())
            {
                var testResult = false;
                var testName = $"Konata.Test.{element.Name}";
                Exception testExpection = null;

                Console.WriteLine($"[ .... ] Testing => {testName}");

                try
                {
                    var testInstance = Activator.CreateInstance(element);
                    var testMethod = element.GetMethod("Run");
                    testResult = (bool)testMethod.Invoke(testInstance, null);
                }
                catch (Exception e)
                {
                    testExpection = e;
                }
                finally
                {
                    if (testResult)
                    {
                        Console.WriteLine($"[  OK  ] Pass => {testName}");
                    }
                    else
                    {
                        Console.Error.WriteLine($"[FAILED] Test failed => {testName}");

                        if (testExpection != null)
                        {
                            Console.Error.WriteLine("");
                            Console.Error.WriteLine(testExpection.Message);
                            Console.Error.WriteLine(testExpection.StackTrace);
                        }
                    }

                    Console.WriteLine("");
                }
            }
            Console.ReadKey();
        }

        static IEnumerable<Type> GetAllTests()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(t => t.GetTypes())
                       .Where(t => t.IsClass && (t.FullName.IndexOf("<>") == -1) && t.Namespace == "Konata.Test.Tests");
        }
    }
}
