using System.ComponentModel;

namespace Domain.Enums
{
    public enum CourseLevel
    {
        [Description("Khóa Nhập Môn")]
        Beginner = 1,
        [Description("Khóa Nền Tảng")]
        Basic = 2,
        [Description("Khóa Luyện Tập Nâng Cao")]
        Advance = 3
    }
}
