namespace Michael.Net.Domain
{
    public interface IEntity
    {
        public long Id { get; set; }
        public Guid Guid { get; set; }
    }
}
