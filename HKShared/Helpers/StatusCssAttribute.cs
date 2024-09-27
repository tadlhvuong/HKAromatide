using System;
using System.Collections.Generic;
using System.Text;

namespace HKShared.Helpers
{
    public class StatusCssAttribute : Attribute
    {
        public string Name { get; private set; }

        public StatusCssAttribute(string name)
        {
            Name = name;
        }
    }
}
