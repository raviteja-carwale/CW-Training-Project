using System;
using System.Configuration;
using Training.BusinessLogic;

namespace Training.DriverTest
{
    public class DriverClass
    {
        static void Main(string[] args)
        {
            DataProcessor dp= new DataProcessor();
            //UserProfile user = new UserProfile{ Id = 24, FirstName = "Maria", LastName = "Sharapova", DateOfBirth = Utils.ParseDate("19870419"), Gender = 'F'};
            //int id = dp.UpdateUserProfile(user);
            //var x = ConfigurationManager.AppSettings["enableMemCache"];
            Console.WriteLine(dp.GetNumberOfUsers());
            Console.ReadLine();
        }
    }
}
