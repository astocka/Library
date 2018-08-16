using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Rest_Api.Context;
using Rest_Api.Models;

namespace Rest_Api.Controllers
{
    /// <summary>
    ///     Kontroler API dla Czytelników.
    /// </summary>
    [Route("api/[controller]")]
    public class ReadersController : Controller
    {
        /// <summary>
        ///     Kontekst.
        /// </summary>
        protected EFCContext Context { get; set; }

        /// <summary>
        ///     Konstruktor.
        /// </summary>
        /// <param name="context">Obiekt Kontekstu.</param>
        public ReadersController(EFCContext context)
        {
            Context = context;
        }

        /// <summary>
        ///     Akcja Read List, zwracająca wszystkich czytelników. Dostępna pod API: <strong>/api/readers</strong>.
        /// </summary>
        /// <returns>Tablica z obiektami.</returns>
        [HttpGet]
        public IEnumerable<ReaderModel> Get() => Context.Readers.ToArray();

        /// <summary>
        ///     Akcja Read, zwracająca obiekt o podanym ID. Dostępna pod API: <strong>/api/readers/1</strong>.
        /// </summary>
        /// <param name="id">Pole ID obiektu, który zostanie zwrócony.</param>
        /// <returns>Znaleziony obiekt lub kod 404, gdy nie odnaleziono szukanego obiektu.</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var record = Context.Readers.SingleOrDefault(x => x.ID == id);
            if (record == null)
            {
                return NotFound();
            }
            return Ok(record);
        }

        /// <summary>
        ///     Akcja Create, tworząca nowy obiekt. Dostępna pod API: <strong>/api/readers</strong>.
        /// </summary>
        /// <param name="name">Imię i nazwisko czytelnika.</param>
        /// <param name="age">Wiek czytelnika.</param>
        /// <returns>Kod 201 wraz z adresem utworzonego zasobu lub kod 409, gdy zasób nie może zostać utworzony z powodu konfliktu.</returns>
        [HttpPost]
        public IActionResult Post(string name, string age)
        {
            int parseAge = 0;
            try
            {
                parseAge = int.Parse(age);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
            var record = Context.Readers.SingleOrDefault(x => x.Name == name && x.Age == parseAge);
            if (record != null)
            {
                return StatusCode(409);
            }
            var readerModel = new ReaderModel()
            {
                Name = name,
                Age = parseAge
            };
            Context.Readers.Add(readerModel);
            Context.SaveChanges();
            return CreatedAtAction("Get", readerModel.ID);
        }

        /// <summary>
        ///     Akcja Update, aktualizująca obiekt. Dostępna pod API: <strong>/api/readers/1</strong>.
        /// </summary>
        /// <param name="id">Identyfikator aktualizowanego obiektu.</param>
        /// <param name="name">Nowe imię i nazwisko czytelnika.</param>
        /// <param name="age">Nowy wiek czytelnika.</param>
        /// <returns>
        ///     Kod 204 jeśli brak danych do aktualizacji, kod 404 jeśli nie można odnaleźć zasobu o podanym identyfikatorze.
        ///     Kod 200 jeśli aktualizacja przebiegła pomyślnie.
        /// </returns>
        [HttpPut("{id}")]
        public IActionResult Put(int id, string name, string age)
        {
            int parseAge = 0;
            try
            {
                parseAge = int.Parse(age);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
            if (string.IsNullOrWhiteSpace(name) || parseAge < 10)
            {
                return StatusCode(204);
            }
            var record = Context.Readers.SingleOrDefault(x => x.ID == id);
            if (record == null)
            {
                return NotFound();
            }
            record.Name = name;
            record.Age = parseAge;
            Context.SaveChanges();
            return Ok();
        }

        /// <summary>
        ///     Akcja Delete, usuwająca zasób. Dostępna pod API: <strong>/api/readers/1</strong>.
        /// </summary>
        /// <param name="id">Identyfikator zasobu.</param>
        /// <returns>
        ///     Kod 404 jeśli nie można odnaleźć zasobu o podanym identyfikatorze. Kod 200 jeśli zasób został usunięty
        ///     pomyślnie.
        /// </returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var record = Context.Readers.SingleOrDefault(x => x.ID == id);
            if (record == null)
            {
                return NotFound();
            }
            Context.Readers.Remove(record);
            Context.SaveChanges();
            return Ok();
        }
    }
}