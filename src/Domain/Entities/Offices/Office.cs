namespace MyApp.Domain.Entities.Offices;

public class Office : StandardNamedEntity
{
    public override int MinNameLength => Constants.MinimumNameLength;
    public override int MaxNameLength => Constants.MaximumNameLength;
    public Office() { }
    public Office(Guid id, string name) : base(id, name) { }
}
