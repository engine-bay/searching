using System.Text.Json;
using SearchingTemp.Entities;
using SearchingTemp.Persistence;

namespace SearchingTemp.Algorithms.LSH
{
  public class BulkLsh
  {
    public List<Person> PersonsData { get; protected set; }
    public Vocabulary VocabularyData { get; protected set; }
    public MinHash MinHashFunction { get; set; }

    private const int ShingleSize = 2;
    private const int NumberOfMinHashVectors = 100;

    public BulkLsh(List<Person> personsData)
    {
      personsData.ForEach(x => x.ShingleSet = new Shingling(x.ToString(), ShingleSize).ShingledData);

      var vocabulary = personsData.Select(x => x.ShingleSet)
        .Aggregate(new HashSet<string>(), (acc, set) => acc.Union(set).ToHashSet());
      //encrypt and persist vocabulary and size

      var vocab = new Vocabulary
      {
        Count = vocabulary.Count,
        VocabularyItem = string.Join(", ", vocabulary),
        CreateDateTime = DateTime.Now
      };
      this.VocabularyData = vocab;

      //persist min hash function
      var minHashFunctions = new MinHashFunction(vocabulary.Count, NumberOfMinHashVectors).Hashes;

      this.MinHashFunction = new MinHash
      {
        Data = JsonSerializer.Serialize(minHashFunctions)
      };

      personsData.ForEach(x =>
      {
        x.OneHotVector = new List<int>();
        foreach (var item in vocabulary)
        {
          x.OneHotVector.Add(x.ShingleSet.Contains(item) ? 1 : 0);
        }

        var vectorId = new Guid();
        var signature = new Signature(x.OneHotVector, minHashFunctions, vocabulary.Count);
        var piiVectors = new List<PiiVector>();
        signature.SignatureVector.ToList().ForEach(y =>
        {
          var piiVector = new PiiVector();
          piiVector.Id = Guid.NewGuid();
          piiVector.VectorValue = y;
          piiVectors.Add(piiVector);
        });
        x.PiiVectors = piiVectors;
      });

      this.PersonsData = personsData;
    }
  }
}

