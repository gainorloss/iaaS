namespace Dev.Core
{
    public class UserOutput
    {
        public int Age { get; set; }
        public Child Child { get; set; }
        public string Gender { get; set; }
    }

    public enum Gender
    {
        Male,
        Female
    }
}