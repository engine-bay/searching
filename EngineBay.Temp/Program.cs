using System.Diagnostics;
using Bogus;
using Microsoft.EntityFrameworkCore;
using SearchingTemp.Algorithms.LSH;
using SearchingTemp.Models;
using SearchingTemp.Persistence;
using Person = SearchingTemp.Entities.Person;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SearchingDbContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.None);


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/persons", async (SearchingDbContext db, PersonDto personDto) =>
{
  var lsh = new Lsh(db);
  
  await lsh.AddPerson(personDto);
  return Results.Ok();
});

app.MapGet("/persons/fake/{count}", (int count) =>
{
  if (count <= 0)
  {
    return Results.BadRequest();
  }

  var personFaker = new Faker<PersonDto>()
    .RuleFor(p => p.FirstNames, f => f.Name.FirstName())
    .RuleFor(p => p.LastName, f => f.Name.LastName())
    .RuleFor(p => p.EmailAddress, (f, p) => f.Internet.Email(p.FirstNames, p.LastName));

  var persons = personFaker.Generate(count);
  return Results.Ok(persons);
});

app.MapPost("/persons/fake/", async (SearchingDbContext db, InsertionCountDto insertionCountDto) =>
{
  if (insertionCountDto.Count <= 0)
  {
    return Results.BadRequest();
  }

  var timer = new Stopwatch();
  timer.Start();

  var personFaker = new Faker<Person>()
    .RuleFor(p => p.FirstNames, f => f.Name.FirstName())
    .RuleFor(p => p.LastName, f => f.Name.LastName())
    .RuleFor(p => p.EmailAddress, (f, p) => f.Internet.Email(p.FirstNames, p.LastName));

  var persons = personFaker.Generate(insertionCountDto.Count);
  timer.Stop();

  Console.WriteLine($"Time taken to generate fake data: {timer.Elapsed}");

  timer.Start();

  var lsh = new BulkLsh(persons);
  timer.Stop();

  Console.WriteLine($"Time taken generate vectors: {timer.Elapsed}");

  timer.Start();
  await db.Persons.AddRangeAsync(lsh.PersonsData);
  await db.Vocabularies.AddAsync(lsh.VocabularyData);
  await db.MinHashFunction.AddAsync(lsh.MinHashFunction);
  await db.SaveChangesAsync();
  Console.WriteLine($"Time taken to insert data: {timer.Elapsed}");
  timer.Stop();

  return Results.Ok();
});

app.MapGet("/persons/search/{term}", async (SearchingDbContext db, string term) =>
{
  if (term.Equals(""))
  {
    return Results.BadRequest();
  }
  var timer = new Stopwatch();
  timer.Start();
  var searching = new Search(db);
  var result = await searching.SearchForTerm(term);
  timer.Stop();
  var model = new
  {
    TimeTaken = timer.Elapsed,
    Data = result
  };
  return Results.Ok(model);
});

app.MapGet("v2/persons/search/{term}", async (SearchingDbContext db, string term) =>
{
  if (term.Equals(""))
  {
    return Results.BadRequest();
  }
  var timer = new Stopwatch();
  timer.Start();
 
  var searching = new Search(db);
  var jaccardResult = await searching.SearchForTerm(term);
  
  var topTen = jaccardResult
    .OrderByDescending(x => x.JaccardSimilarity)
    .Take(10)
    .Select(x => x.PersonId)
    .ToList();
  
 var persons = db.Persons.Where(x => topTen.Contains(x.Id)).ToList();
  
 var result = persons.Join(jaccardResult, x => x.Id, y => y.PersonId, (person, jaccard) =>
   new {
    Similarity = jaccard.JaccardSimilarity,
    FirstNames = person.FirstNames,
    LastName = person.LastName,
    EmailAddress = person.EmailAddress,
    Id =  person.Id
   });
  
  timer.Stop();
  var model = new
  {
    TimeTaken = timer.Elapsed,
    Data = result.OrderByDescending(x => x.Similarity)
  };
  return Results.Ok(model);
});

app.Run();
