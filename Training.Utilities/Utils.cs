using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Training.Entities;

namespace Training.Utilities
{
    public static class Utils
    {
        // Parse string to required Date format.
        public static bool ValidDate(DateTime date)
        {
            return !date.Equals(DateTime.MinValue);
        }

        public static bool ValidName(string name)
        {
            return name.Length < 21 && Regex.IsMatch(name, @"^[a-zA-Z]+[a-zA-Z\s]+$");
        }

        public static bool ValidGender(char gender)
        {
            gender = char.ToUpperInvariant(gender);
            return gender.Equals('M') || gender.Equals('F');
        }

        public static bool ValidUser(UserProfile user)
        {
            if (user == null)
            {
                return false;
            }
            if (user.FirstName == null || user.LastName == null || user.Gender == 0)
            {
                return false;
            }
            return true;
        }
    }
}
