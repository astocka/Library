using System.Collections.Generic;
using Newtonsoft.Json;

namespace Rest_Api.Models
{
    /// <summary>
    ///     Model dla Książki.
    /// </summary>
    public class BookModel
    {
        /// <summary>
        ///     Indetyfikator.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        ///     Tytuł.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Autor.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        ///     Kolekcja Czytelników Książki.
        /// </summary>
        [JsonIgnore]
        public ICollection<BookReaderModel> Readers { get; set; }
    }
}