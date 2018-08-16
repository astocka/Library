using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rest_Api.Context;
using Rest_Api.Models;

namespace Rest_Api.Controllers
{
    /// <summary>
    ///     Kontroler API dla Wypożyczenia Książki przez Czytelnika.
    /// </summary>
    [Route("api/[controller]")]
    public class LendController : Controller
    {
        /// <summary>
        ///     Kontekst.
        /// </summary>
        protected EFCContext Context { get; set; }

        /// <summary>
        ///     Konstruktor.
        /// </summary>
        /// <param name="context">Obiekt Kontekstu.</param>
        public LendController(EFCContext context)
        {
            Context = context;
        }

        /// <summary>
        ///     Akcja Read List, zwracająca wszystkie wypożyczone książki. Dostępna pod API: <strong>/api/lend</strong>.
        /// </summary>
        /// <returns>Tablica z obiektami.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            var list = Context.BookReader.Include(x => x.Book).Include(x => x.Reader).Select(x => new
            {
                ID = x.ID,
                BookID = x.BookID,
                ReaderID = x.ReaderID,
                LendDate = x.LendDate,
                Name = x.Reader.Name,
                Title = x.Book.Title
            }).ToArray();
            return Ok(list);
        }

        /// <summary>
        ///     Akcja Create, tworząca nowy obiekt. Dostępna pod API: <strong>/api/lend</strong>.
        /// </summary>
        /// <param name="bookID">Identyfikator książki do wypożyczenia.</param>
        /// <param name="readerID">Identyfikator czytelnika wypożyczającego książkę.</param>
        /// <param name="date">Data wypożycznia książki.</param>
        /// <returns>Kod 201 wraz z adresem utworzonego zasobu lub kod 409, gdy zasób nie może zostać utworzony z powodu konfliktu.</returns>
        [HttpPost]
        public IActionResult Post(string bookID, string readerID, string lendDate)
        {
            int bookIDParse = 0;
            int readerIDParse = 0;
            DateTime dateParse = DateTime.MinValue;
            try
            {
                bookIDParse = int.Parse(bookID);
                readerIDParse = int.Parse(readerID);
                dateParse = DateTime.Parse(lendDate);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
            var record = Context.BookReader.SingleOrDefault(x => x.BookID == bookIDParse);
            if (record != null)
            {
                return StatusCode(409);
            }
            var bookReaderModel = new BookReaderModel()
            {
                BookID = bookIDParse,
                ReaderID = readerIDParse,
                LendDate = dateParse
            };
            Context.BookReader.Add(bookReaderModel);
            Context.SaveChanges();
            return StatusCode(201);
        }

        /// <summary>
        ///     Akcja Delete, usuwająca zasób. Dostępna pod API: <strong>/api/lend/1</strong>.
        /// </summary>
        /// <param name="id">Identyfikator zasobu.</param>
        /// <returns>
        ///     Kod 404 jeśli nie można odnaleźć zasobu o podanym identyfikatorze. Kod 200 jeśli zasób został usunięty
        ///     pomyślnie.
        /// </returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var record = Context.BookReader.SingleOrDefault(x => x.ID == id);
            if (record == null)
            {
                return NotFound();
            }
            Context.BookReader.Remove(record);
            Context.SaveChanges();
            return Ok();
        }
    }
}