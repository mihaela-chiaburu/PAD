using Common.Models;

namespace MovieAPI.Repositories
{
    public interface IRepository<T> where T : DBdocument
    {
        List<T> GetAllRecord();
        T InsertRecord(T record);
        T GetRecordById(Guid id);
        void UpsertRecord(T record);
        void DeleteRecord(Guid id);
    }
}
