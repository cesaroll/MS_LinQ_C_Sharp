using System;
using System.CodeDom;
using System.Globalization;

namespace LanguageExtensions
{
    public class MyFileInfo
    {
        public string Name { get; set; }
        public long Length { get; set; }
        public DateTime CreationTime { get; set; }

        public override string ToString()
        {
            return String.Format("{0} {1,15:N0}  {2}",
                CreationTime.ToString("g", DateTimeFormatInfo.InvariantInfo),
                Length, Name);
        }
    }
}