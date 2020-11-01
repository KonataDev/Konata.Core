using System;
using System.Linq;
using System.Collections.Generic;

namespace Konata.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var allTests = GetAllTests();

            var testCount = allTests.Count();
            var testPassCount = 0;
            var testFailedCount = 0;

            foreach (var element in allTests)
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
                        ++testPassCount;
                        Console.WriteLine($"[  OK  ] Pass => {testName}");
                    }
                    else
                    {
                        ++testFailedCount;
                        Console.ForegroundColor = ConsoleColor.Red;
                        {
                            Console.Error.WriteLine($"[FAILED] Test failed => {testName}");

                            if (testExpection != null)
                            {
                                Console.Error.WriteLine("");
                                Console.Error.WriteLine(testExpection.Message);
                                Console.Error.WriteLine(testExpection.StackTrace);
                            }
                        }
                        Console.ResetColor();
                    }

                    Console.WriteLine("");
                }
            }

            Console.WriteLine($"Test statistics for the project: ");
            Console.WriteLine($"Tests = { testCount}, Pass = { testPassCount}, Failed = { testFailedCount} ({(float)testPassCount / testCount * 100F}% pass)");

#if DEBUG
            Console.Read();
#endif
        }

        static IEnumerable<Type> GetAllTests()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(t => t.GetTypes())
                       .Where(t => t.IsClass && (t.FullName.IndexOf("<>") == -1) && t.Namespace == "Konata.Test.Tests");
        }
    }
}
