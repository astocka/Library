using System;
using Newtonsoft.Json;

namespace Rest_Api.Models
{
    /// <summary>
    ///     Model łączący Książki z Czytelnikami.
    /// </summary>
    public class BookReaderModel
    {
        /// <summary>
        ///     Identyfikator.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        ///     Książka.
        /// </summary>
        [JsonIgnore]
        public BookModel Book { get; set; }

        /// <summary>
        ///     Czytelnik.
        /// </summary>
        [JsonIgnore]
        public ReaderModel Reader { get; set; }

        /// <summary>
        ///     Identyfikator Książki.
        /// </summary>
        public int BookID { get; set; }

        /// <summary>
        ///     Identyfikator Czytelnika.
        /// </summary>
        public int ReaderID { get; set; }

        /// <summary>
        ///     Data Wypożyczenia.
        /// </summary>
        public DateTime? LendDate { get; set; }
    }
}