using System;
using System.Collections.Generic;

namespace ABus
{
    public class Stage
    {
        public Stage(string name)
        {
            this.Name = name;
            this.Tasks = new LinkedList<Type>();
        }

        public string Name { get; private set; }

        public LinkedList<Type> Tasks { get; private set; }
    }
}