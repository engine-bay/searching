namespace SearchingTemp.Entities
{
  public class Vocabulary
  {
    public Guid Id { get; set; }
    public DateTime CreateDateTime { get; set; }
    public string VocabularyItem { get; set; } // Hash this
    public int Count { get; set; }
  }
}

