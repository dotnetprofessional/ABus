using System.Collections.Generic;

namespace ABus
{
    public static class Extensions
    {
        public static IEnumerable<LinkedListNode<T>> Nodes<T>(this LinkedList<T> list)
        {
            // Usage:
            // var matchingNode = list.Nodes().FirstOrDefault(n => n.Value.Id == myId);
            for (var node = list.First; node != null; node = node.Next)
            {
                yield return node;
            }
        }
    }
}
