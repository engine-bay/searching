using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SearchingTemp.Entities;
using SearchingTemp.Persistence;

namespace SearchingTemp.Algorithms.LSH
{
  public class Search
  {
    private const int ShingleSize = 2; // move to some common config
    private const int NumberOfMinHashVectors = 100;
    private SearchingDbContext dbContext;

    public Search(SearchingDbContext dbContext)
    {
      this.dbContext = dbContext;
    }

    public async Task<List<SearchResult>> SearchForTerm(string searchTerm)
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


      var shingleSet = new Shingling(searchTerm, ShingleSize).ShingledData;

      var oneHotVector = new List<int>();
      var vocabulary = vocab.VocabularyItem.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();

      foreach (var item in vocabulary)
      {
        oneHotVector.Add(shingleSet.Contains(item) ? 1 : 0);
      }

      var signature = new Signature(oneHotVector, minHashFunction, vocab.Count).SignatureVector;

      string inputValues = string.Join(",", signature);
      var inputParam = new SqlParameter("@inputValues", SqlDbType.VarChar) { Value = inputValues };

      var query = @"
        DECLARE @queryVector TABLE (vectorValue INT);
        INSERT INTO @queryVector (vectorValue)
        SELECT value FROM STRING_SPLIT(@inputValues, ',');

        WITH IntersectionCount AS (
            SELECT 
                a.personId,
                COUNT(*) AS count
            FROM 
                [dbo].[PIIVectors] a
                JOIN @queryVector q ON a.vectorValue = q.vectorValue
            GROUP BY 
                a.personId
        ),
        UnionCount AS (
            SELECT 
                a.personId,
                COUNT(*) AS count
            FROM (
                SELECT vectorValue, personId FROM [dbo].[PIIVectors] WHERE personId IN (SELECT personId FROM IntersectionCount)
                UNION
                SELECT vectorValue, NULL FROM @queryVector
            ) a
            GROUP BY 
                a.personId
        )
        SELECT 
            i.personId AS PersonId,
            i.count * 1.0 / u.count AS JaccardSimilarity
        FROM 
            IntersectionCount i
            JOIN UnionCount u ON i.personId = u.personId
        ORDER BY 
            JaccardSimilarity DESC;
        ";

//      var results = this.dbContext.Database.SqlQuery<SearchResult>(query, inputParam);

      var results = this.dbContext.SearchResults.FromSqlRaw(query, inputParam);

      var result = await results.ToListAsync();
      return result;
    }
  }
}

