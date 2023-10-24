using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SearchingTemp.Entities;
using SearchingTemp.Models;
using SearchingTemp.Persistence;

namespace SearchingTemp.Algorithms.LSH
{
  public class Lsh
  {
    private const int ShingleSize = 2;
    private const int NumberOfMinHashVectors = 100;

    private SearchingDbContext dbContext;

    public Lsh(SearchingDbContext dbContext)
    {
      this.dbContext = dbContext;
    }

    public async Task AddPerson(PersonDto personDto)
    {
      var vocab = await this.dbContext.Vocabularies.FirstOrDefaultAsync();
      if (vocab is null)
      {
        throw new NullReferenceException(nameof(vocab));
      }

      var minHash = await this.dbContext.MinHashFunction.FirstOrDefaultAsync();
      if (minHash is null)
      {
        throw new NullReferenceException(nameof(minHash));
      }

      var minHashFunction = JsonSerializer.Deserialize<List<List<int>>>(minHash.Data);
      if (minHashFunction is null)
      {
        throw new NullReferenceException(nameof(minHashFunction));
      }
      
      var person = new Person
      {
        FirstNames = personDto.FirstNames,
        LastName = personDto.LastName,
        EmailAddress = personDto.EmailAddress
      };
      person.ShingleSet = new Shingling(person.ToString(), ShingleSize).ShingledData;
      

      person.OneHotVector = new List<int>();
      var vocabulary = vocab.VocabularyItem.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();

      foreach (var item in vocabulary)
      {
        person.OneHotVector.Add(person.ShingleSet.Contains(item) ? 1 : 0);
      }

      var signature = new Signature(person.OneHotVector, minHashFunction, vocabulary.Count);
      var piiVectors = new List<PiiVector>();
      signature.SignatureVector.ToList().ForEach(y =>
      {
        var piiVector = new PiiVector();
        piiVector.Id = Guid.NewGuid();
        piiVector.VectorValue = y;
        piiVectors.Add(piiVector);
      });
      person.PiiVectors = piiVectors;
      
      await this.dbContext.Persons.AddAsync(person);
      await this.dbContext.SaveChangesAsync();
    }
  }
}