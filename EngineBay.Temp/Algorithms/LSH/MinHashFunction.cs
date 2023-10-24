using SearchingTemp.Extensions;

namespace SearchingTemp.Algorithms.LSH
{
  public class MinHashFunction
  {
    public List<List<int>> Hashes { get; protected set; }

    public MinHashFunction(int vocabularySize, int nBits)
    {
      
      this.Hashes = new List<List<int>>();
      
      for (int i = 0; i < nBits; i++)
      {
        this.Hashes.Add(CreateHashFunction(vocabularySize));
      }
    }

    private static List<int> CreateHashFunction(int size)
    {
      var minHashVector = new List<int>();
      for (int i = 1; i <= size; i++)
      {
        minHashVector.Add(i);
      }

      minHashVector.Shuffle();
      return minHashVector;
    }
  }
}