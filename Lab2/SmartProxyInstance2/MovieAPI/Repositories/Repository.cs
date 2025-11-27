
using Common.Models;

namespace MovieAPI.Repositories
{
    public class Repository<T> : IRepository<T> where T : DBdocument
    {
        private readonly ApplicationDbContext _context;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<T> GetAllRecord()
        {
            return _context.Set<T>().ToList();
        }

        public T GetRecordById(Guid id)
        {
            return _context.Set<T>().Find(id);
        }

        public T InsertRecord(T record)
        {
            _context.Set<T>().Add(record);
            _context.SaveChanges();
            return record;
        }

        public void DeleteRecord(Guid id)
        {
            var entity = _context.Set<T>().Find(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                _context.SaveChanges();
            }
        }

        public void UpsertRecord(T record)
        {
            var existing = _context.Set<T>().Find(record.Id);
            if (existing == null)
            {
                _context.Set<T>().Add(record);
            }
            else
            {
                _context.Entry(existing).CurrentValues.SetValues(record);
            }
            _context.SaveChanges();
        }

    }
}
