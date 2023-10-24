namespace SearchingTemp.Algorithms.LSH
{
  public class Signature
  {
    public HashSet<int> SignatureVector { get; protected set; }
    
    public Signature(List<int> vector, List<List<int>> minHashFunction, int vocubularySize)
    {
      this.SignatureVector = new HashSet<int>();
      foreach (var func in minHashFunction)
      {
        for (int i = 1; i <= vocubularySize; i++)
        {
          var index = func.IndexOf(i);
          var signatureValue = vector[index];
          if (signatureValue == 1)
          {
            this.SignatureVector.Add(index);
            break;
          }
        }
      }
    }
  }
}

