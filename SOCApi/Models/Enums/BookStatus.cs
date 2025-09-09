using System.ComponentModel;

namespace SOCApi.Models.Enums
{
    public enum BookStatus
    {
        [Description("Available for checkout")]
        Available = 1,
        
        [Description("Currently lent out")]
        Lent = 2,
        
        [Description("Reserved for future checkout")]
        Reserved = 3,
        
        [Description("Damaged and needs repair")]
        Damaged = 4,
        
        [Description("Lost or missing")]
        Lost = 5,
        
        [Description("Retired from circulation")]
        Retired = 6
    }
}