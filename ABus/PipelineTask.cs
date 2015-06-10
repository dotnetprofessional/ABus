using System;

namespace ABus
{
    public class PipelineTask : IComparable
    {
        public PipelineTask(string name, Type task)
        {
            Name = name;
            Task = task;
        }

        public string Name { get; private set; }

        public Type Task { get; private set; }

        public int CompareTo(object obj)
        {
            return String.Compare(this.Name, obj.ToString(), StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return this.Name.Equals(obj.ToString());
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}