namespace UdemyGrabberWPF.ExtensionMethods
{
    public static class StringExtension
    {
        public static bool ValidURL(this string str)
        {
            if (str.Contains("udemy.com") && str.Contains("couponCode"))
            {
                return true;
            }
            return false;
        }
    }
}
