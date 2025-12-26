using Android.Content;
using Android.Views;

namespace Bourse.Views
{
    /// <summary>
    /// equivalent de ArrayAdapter :
    /// - pas de lock, non threadsafe
    /// - non filtrable
    /// + acces direct à la liste sous jacente
    ///   sans wrapper dans une JavaList
    /// </summary>
    public abstract class SimpleListAdapter<T> : BaseAdapter<T>
    {
        public List<T> CurrentItems { get; private set; }
        protected Context Context { get; private set; }
        protected LayoutInflater Inflater { get; private set; }

        protected SimpleListAdapter(Context context, List<T> items)
        {
            CurrentItems = items;
            Context = context;
            Inflater = LayoutInflater.From(Context);
        }

        protected SimpleListAdapter(Context context)
        {
            CurrentItems = new List<T>();
            Context = context;
            Inflater = LayoutInflater.From(Context);
        }

        public override T this[int position] => CurrentItems[position];

        public override int Count => CurrentItems.Count;

        public override long GetItemId(int position) => position;

        public virtual void Reset(IEnumerable<T> newContent = null)
        {
            CurrentItems.Clear();
            if (newContent != null) CurrentItems.AddRange(newContent);
            NotifyDataSetChanged();
        }

        public void Add(T item)
        {
            CurrentItems.Add(item);
        }
    }

}
