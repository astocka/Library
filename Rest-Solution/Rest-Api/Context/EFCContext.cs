using Microsoft.EntityFrameworkCore;
using Rest_Api.Models;

namespace Rest_Api.Context
{
    /// <summary>
    ///     Klasa kontekstu dostępu do bazy danych.
    /// </summary>
    public class EFCContext : DbContext
    {
        /// <summary>
        ///     Książki.
        /// </summary>
        public DbSet<BookModel> Books { get; set; }

        /// <summary>
        ///     Czytelnicy.
        /// </summary>
        public DbSet<ReaderModel> Readers { get; set; }

        /// <summary>
        ///     Relacja między Książkami, a Czytelnikami.
        /// </summary>
        public DbSet<BookReaderModel> BookReader { get; set; }

        #region Overrides of DbContext

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("MainDb"); // Wykorzystanie Bazy Danych w Pamięci RAM.
        }

        #endregion
    }
}