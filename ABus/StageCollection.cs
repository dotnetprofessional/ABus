using System.Collections.ObjectModel;

namespace ABus
{
    public class StageCollection : KeyedCollection<string, Stage>
    {
        protected override string GetKeyForItem(Stage item)
        {
            return item.Name;
        }
    }
}
