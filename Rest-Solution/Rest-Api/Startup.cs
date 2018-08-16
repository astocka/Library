using System;
using Bogus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rest_Api.Context;
using Rest_Api.Extensions;
using Rest_Api.Models;

namespace Rest_Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment hostingEnvironment)
        {
        }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddMvc().AddJsonOptions(x => x.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver());
            serviceCollection.AddDbContext<EFCContext>();
        }

        public void Configure(IApplicationBuilder applicationBuilder, IHostingEnvironment hostingEnvironment, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            applicationBuilder.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, PATCH");
                await next.Invoke();
            });

            applicationBuilder.UseCors(builder =>
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
            );

            applicationBuilder.UseMvc();

            DbInit();
        }

        /// <summary>
        ///     Wypełnia bazę w pamięci przykładowymi danymi. Dodatkowo wyświetla dodane dane w konsoli.
        /// </summary>
        protected void DbInit()
        {
            using (var context = new EFCContext())
            {
                var bookFaker = new Faker<BookModel>();
                bookFaker.RuleFor(x => x.Title, faker => faker.Lorem.Word().ToUpperFirstLetter());
                bookFaker.RuleFor(x => x.Author, faker => $"{faker.Person.FirstName} {faker.Person.LastName}");
                var bookModels = bookFaker.Generate(6);

                foreach (var b in bookModels)
                {
                    Console.WriteLine($"Book: {b.ID}, {b.Title}, {b.Author}");
                }

                var readerFaker = new Faker<ReaderModel>();
                readerFaker.RuleFor(x => x.Name, faker => $"{faker.Person.FirstName} {faker.Person.LastName}");
                readerFaker.RuleFor(x => x.Age, faker => faker.Random.Number(10, 100));
                var readerModels = readerFaker.Generate(8);

                foreach (var r in readerModels)
                {
                    Console.WriteLine($"Reader: {r.ID}, {r.Name}, {r.Age}");
                }

                var bookReaderFaker = new Faker<BookReaderModel>();
                bookReaderFaker.RuleFor(x => x.BookID, faker => faker.IndexFaker);
                bookReaderFaker.RuleFor(x => x.ReaderID, faker => faker.IndexFaker);
                bookReaderFaker.RuleFor(x => x.LendDate, faker => faker.Date.Past());
                var bookReaderModels = bookReaderFaker.Generate(2);

                foreach (var br in bookReaderModels)
                {
                    Console.WriteLine($"Book-Reader: {br.ID}, {br.BookID}, {br.ReaderID}, {br.LendDate}");
                }

                context.Books.AddRange(bookModels);
                context.Readers.AddRange(readerModels);
                context.BookReader.AddRange(bookReaderModels);
                context.SaveChanges();
            }
        }
    }
}