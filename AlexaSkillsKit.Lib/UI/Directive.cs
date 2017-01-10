using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexaSkillsKit.UI
{
    public class Directive
    {
        public string Type { get; set; }
        public string PlayBehavior { get; set; }
        public AudioItem AudioItem { get; set; }
    }

    public class AudioItem
    {
        public Stream Stream { get; set; }
    }

    public class Stream
    {
        public string Token { get; set; }
        public string Url { get; set; }
        public int OffsetInMilliseconds { get; set; }
    }
    
}
