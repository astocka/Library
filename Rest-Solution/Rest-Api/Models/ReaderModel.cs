using System.Collections.Generic;
using Newtonsoft.Json;

namespace Rest_Api.Models
{
    /// <summary>
    ///     Model dla Czytelnika.
    /// </summary>
    public class ReaderModel
    {
        /// <summary>
        ///     Identyfikator.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        ///     Nazwa Czytelnika.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Wiek Czytelnika.
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        ///     Lista Książek wypożyczonych przez Czytelnika.
        /// </summary>
        [JsonIgnore]
        public ICollection<BookReaderModel> Books { get; set; }
    }
}