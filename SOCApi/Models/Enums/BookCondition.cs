using System.ComponentModel;

namespace SOCApi.Models.Enums
{
    public enum BookCondition
    {
        [Description("Brand new, never used")]
        New = 1,
        
        [Description("Excellent condition, minimal wear")]
        LikeNew = 2,
        
        [Description("Very good condition, light wear")]
        VeryGood = 3,
        
        [Description("Good condition, normal wear")]
        Good = 4,
        
        [Description("Fair condition, noticeable wear")]
        Fair = 5,
        
        [Description("Poor condition, significant wear")]
        Poor = 6
    }
}