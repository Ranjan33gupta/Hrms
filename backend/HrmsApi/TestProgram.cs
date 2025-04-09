using System;
using System.Threading.Tasks;

namespace HrmsApi
{
    public class TestProgram
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Starting employee addition test...");
            await TestAddEmployee.AddEmployeeTest();
            Console.WriteLine("Test completed.");
        }
    }
}
