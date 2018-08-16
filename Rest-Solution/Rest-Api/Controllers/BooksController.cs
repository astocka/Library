using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Rest_Api.Context;
using Rest_Api.Models;

namespace Rest_Api.Controllers
{
    /// <summary>
    ///     Kontroler API dla Książek.
    /// </summary>
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        /// <summary>
        ///     Kontekst.
        /// </summary>
        protected EFCContext Context { get; set; }

        /// <summary>
        ///     Konstruktor.
        /// </summary>
        /// <param name="context">Obiekt Kontekstu.</param>
        public BooksController(EFCContext context)
        {
            Context = context;
        }

        /// <summary>
        ///     Akcja Read List, zwracająca wszystkie książki. Dostępna pod API: <strong>/api/books</strong>.
        /// </summary>
        /// <returns>Tablica z obiektami.</returns>
        [HttpGet]
        public IEnumerable<BookModel> Get() => Context.Books.ToArray();

        /// <summary>
        ///     Akcja Read, zwracająca obiekt o podanym ID. Dostępna pod API: <strong>/api/books/1</strong>.
        /// </summary>
        /// <param name="id">Pole ID obiektu, który zostanie zwrócony.</param>
        /// <returns>Znaleziony obiekt lub kod 404, gdy nie odnaleziono szukanego obiektu.</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var record = Context.Books.SingleOrDefault(x => x.ID == id);
            if (record == null)
            {
                return NotFound();
            }
            return Ok(record);
        }

        /// <summary>
        ///     Akcja Create, tworząca nowy obiekt. Dostępna pod API: <strong>/api/books</strong>.
        /// </summary>
        /// <param name="title">Tytuł książki.</param>
        /// <param name="author">Autor książki.</param>
        /// <returns>Kod 201 wraz z adresem utworzonego zasobu lub kod 409, gdy zasób nie może zostać utworzony z powodu konfliktu.</returns>
        [HttpPost]
        public IActionResult Post(string title, string author)
        {
            var record = Context.Books.SingleOrDefault(x => x.Title == title && x.Author == author);
            if (record != null)
            {
                return StatusCode(409);
            }
            var bookModel = new BookModel()
            {
                Title = title,
                Author = author
            };
            Context.Books.Add(bookModel);
            Context.SaveChanges();
            return CreatedAtAction("Get", bookModel.ID);
        }

        /// <summary>
        ///     Akcja Update, aktualizująca obiekt. Dostępna pod API: <strong>/api/books/1</strong>.
        /// </summary>
        /// <param name="id">Identyfikator aktualizowanego obiektu.</param>
        /// <param name="title">Nowy tytuł książki.</param>
        /// <param name="author">Nowy autor książki.</param>
        /// <returns>
        ///     Kod 204 jeśli brak danych do aktualizacji, kod 404 jeśli nie można odnaleźć zasobu o podanym identyfikatorze.
        ///     Kod 200 jeśli aktualizacja przebiegła pomyślnie.
        /// </returns>
        [HttpPut("{id}")]
        public IActionResult Put(int id, string title,  string author)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author))
            {
                return StatusCode(204);
            }
            var record = Context.Books.SingleOrDefault(x => x.ID == id);
            if (record == null)
            {
                return NotFound();
            }
            record.Title = title;
            record.Author = author;
            Context.SaveChanges();
            return Ok();
        }

        /// <summary>
        ///     Akcja Delete, usuwająca zasób. Dostępna pod API: <strong>/api/books/1</strong>.
        /// </summary>
        /// <param name="id">Identyfikator zasobu.</param>
        /// <returns>
        ///     Kod 404 jeśli nie można odnaleźć zasobu o podanym identyfikatorze. Kod 200 jeśli zasób został usunięty
        ///     pomyślnie.
        /// </returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var record = Context.Books.SingleOrDefault(x => x.ID == id);
            if (record == null)
            {
                return NotFound();
            }
            Context.Books.Remove(record);
            Context.SaveChanges();
            return Ok();
        }
    }
}