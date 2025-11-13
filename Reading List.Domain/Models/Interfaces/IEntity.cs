namespace Reading_List.Domain.Models.Interfaces
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}