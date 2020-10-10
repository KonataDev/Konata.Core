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
                    Console.WriteLine($"{(testResult ? "[  OK  ] Pass" : "[FAILED] Test failed")} => {testName}");

                    if (testExpection != null)
                    {
                        Console.WriteLine(testExpection.Message);
                        Console.WriteLine(testExpection.StackTrace);
                    }
                }
            }
            Console.ReadKey();
        }

        static IEnumerable<Type> GetAllTests()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(t => t.GetTypes())
                       .Where(t => t.IsClass && t.Namespace == "Konata.Test.Tests");
        }
    }
}