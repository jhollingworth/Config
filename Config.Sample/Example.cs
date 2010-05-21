using Castle.Components.Validator;

namespace Config.Sample
{
    public class Example
    {
        [ValidateRange(0, 11)]
        public int LevelOfAwesome { get; set; }

        [ValidateLength(0, 50)]
        public string HowAwesome { get; set; }
        
        public bool IsAwesome { get; set; }
    }
}