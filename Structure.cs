using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OceStory
{
    public class OceCommonStruc
    {
        public struct ListCompareAttribure {

            public string ActAttributeName;
            public string TarAttributeName;

            public ListCompareAttribure(string actAttributeName, string tarAttributeName) {
                ActAttributeName = actAttributeName;
                TarAttributeName = tarAttributeName;
            }

            public void Display()
            {
                Console.WriteLine($"ActAttributeName: {ActAttributeName}, TarAttributeName: {TarAttributeName}");
            }
        }
    }
}
