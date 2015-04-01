using System.Collections.ObjectModel;

namespace ABus.Contracts
{
    public class MetaDataCollection : KeyedCollection<string, MetaData>
    {
        protected override string GetKeyForItem(MetaData item)
        {
            return item.Name;
        }
    }
}