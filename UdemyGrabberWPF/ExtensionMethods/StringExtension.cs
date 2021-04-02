using System.Collections.Generic;

namespace UdemyGrabberWPF.ExtensionMethods
{
    public static class StringExtension
    {
        public static bool ValidURL(this string str, List<string> checkedCourses)
        {
            if (str.Contains("udemy.com") && str.Contains("couponCode"))
            {
                const string removeInLink = "https://www.udemy.com/course/";
                string checkedCourse = str[removeInLink.Length..];
                if (checkedCourses.Contains(checkedCourse))
                {
                    return false;
                }
                checkedCourses.Add(checkedCourse);
                return true;
            }
            return false;
        }
    }
}
