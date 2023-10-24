namespace SearchingTemp.Entities
{
  public class PiiVector
  {
    public Guid PersonId { get; set; }
    public Guid Id { get; set; }
    
    public int VectorValue { get; set; }
    
    public Person Person { get; set; }
  }
}