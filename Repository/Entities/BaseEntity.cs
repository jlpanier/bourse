using SQLite;

namespace Repository.Entities
{
    public abstract class BaseEntity
    {
        protected BaseEntity()
        {
        }

        public virtual void Save(SQLiteConnection db)
        {
            db.InsertOrReplace(this);
        }

        public virtual void Remove(SQLiteConnection db)
        {
            db.Delete(this);
        }
    }
}
