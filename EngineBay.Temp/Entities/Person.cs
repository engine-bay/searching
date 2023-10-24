namespace SearchingTemp.Entities
{
  public class Person
  {
    public Guid Id { get; set; }
    public string FirstNames { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }

    public HashSet<string> ShingleSet { get; set; }
    public List<int> OneHotVector { get; set; }

    public ICollection<PiiVector> PiiVectors { get; set; }

    public override string ToString()
    {
      return $"{FirstNames} {LastName} {EmailAddress}";
    }
  }
}