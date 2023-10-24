namespace SearchingTemp.Algorithms.LSH
{
  public class Shingling
  {
    public HashSet<string> ShingledData { get; protected set; }

    public Shingling(string data, int shingleSize)
    {
      this.ShingledData = this.Shingle(data, shingleSize);
    }

    private HashSet<string> Shingle(string data, int shingleSize)
    {
      var shingleSet = new HashSet<string>();
      for (int i = 0; i < data.Length - shingleSize + 1; i++)
      {
        shingleSet.Add(data.Substring(i, shingleSize));
      }

      return shingleSet;
    }
  }
}

// Number of min hash vectors
// Vocabulary
// Vocabulary Count
// Storing min hash function