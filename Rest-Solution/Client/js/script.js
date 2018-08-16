$(function() {
    const apiURL = "http://localhost:49879/api/";

    // show all books
    getAllBooks();

    // alerts
    var alert = $("div[role='alert']");
    alert.hide(); // alert is hidden by default

    // author
    $("header").children("nav").eq(0).children("span").text("Author: Aleksandra Stocka");

    // BOOKS -> FUNCTIONS

    // add new book
    function addNewBook() {
        var title = $("input[name='title']").val();
        var author = $("input[name='author']").val();
        if (title != "" && author != "") {
            var newBook = {
                Title: title,
                Author: author
            }
            $.ajax({
                url: apiURL + "books",
                type: "POST",
                data: newBook
            }).done(function(resp) {
                alert.removeClass("alert-warning").removeClass("alert-danger").addClass("alert-success").text("The book has been successfully added.").fadeIn("slow").fadeOut(6000);
                $("input[name='title']").val("");
                $("input[name='author']").val("");

                $("#booksTable").find("tbody").empty();
                getAllBooks();

            }).fail(function(err) {
                alert.removeClass("alert-success").removeClass("alert-warning").addClass("alert-danger").text("The book already exists in the database!").fadeIn();
            })
        } else {
            alert.removeClass("alert-success").removeClass("alert-danger").addClass("alert-warning").text("Incorrect data. Try again.").fadeIn();
        }
    }

    // edit book
    function editBook(ID, editedBook) {

        var modifiedBook = {
            Title: editedBook.Title,
            Author: editedBook.Author
        }

        $.ajax({
            url: apiURL + "books/" + ID,
            type: "PUT",
            data: modifiedBook
        }).done(function(resp) {
            alert.removeClass("alert-warning").removeClass("alert-danger").addClass("alert-success").text("The update was successful.").fadeIn("slow").fadeOut(6000);
            $("input[name='title']").val("");
            $("input[name='author']").val("");

            $("#booksTable").find("tbody").empty();
            getAllBooks();
            $("button.saveBook").removeClass("saveBook").addClass("addBook").html("<i class='fa fa-plus'></i> Add");
        }).fail(function(err) {
            alert.removeClass("alert-success").removeClass("alert-warning").addClass("alert-danger").text("Book with the given ID does not exist.").show();
        })
    }

    //delete book
    function deleteBook(bookID) {

        $.ajax({
            url: apiURL + "books/" + bookID,
            type: "DELETE",
        }).done(function(resp) {
            alert.removeClass("alert-warning").removeClass("alert-danger").addClass("alert-success").text("The book has been deleted.").fadeIn("slow").fadeOut(6000);
            $("input[name='title']").val("");
            $("input[name='author']").val("");

            $("#booksTable").find("tbody").empty();
            getAllBooks();

        }).fail(function(err) {
            alert.removeClass("alert-success").removeClass("alert-warning").addClass("alert-danger").text("The book with the given ID has not been found.").fadeIn();
        })
    }

    // BOOKS -> EVENTS

    // add book event
    $("#booksForm").on("click", "button.addBook", function(e) {
        e.preventDefault();
        addNewBook();
    })

    // edit book event
    $("#booksTable").on("click", "button.editBook", function(e) {
        e.preventDefault();
        var bookID = $(this).closest("tr").attr("data-book-id");
        var title = $(this).closest("tr").children().eq(0).text();
        var author = $(this).closest("tr").children().eq(1).text();
        var saveButton = $("button.addBook").removeClass("addBook").addClass("saveBook").html("<i class='fa fa-plus'></i> Save");

        var titleInput = $("input[name='title']").val(title);
        var authorInput = $("input[name='author']").val(author);
        var newTitle = "";
        var newAuthor = "";

        saveButton.on("click", function() {
            newTitle = titleInput.val();
            newAuthor = authorInput.val();

            if (bookID > 0 && newTitle != "" && newTitle.length > 0 && newAuthor != "" && newAuthor.length > 0) {
                var editedBook = {
                    Title: newTitle,
                    Author: newAuthor
                }

                editBook(bookID, editedBook);
            } else {
                alert.removeClass("alert-success").removeClass("alert-danger").addClass("alert-warning").text("No data for the update. Try again.").fadeIn();
            }
        })
    })

    // delete book event
    $("#booksTable").on("click", "button.deleteBook", function(e) {
        var bookID = $(this).closest("tr").attr("data-book-id");
        var lendBookID = $("#lendedBooksTable").find("tbody").find("tr").filter(`[data-book-id='${bookID}']`).attr("data-book-id");

        if (bookID != lendBookID) {
            bootbox.confirm({
                title: "Delete the book.",
                message: "Are you sure you want to delete the selected book?",
                buttons: {
                    cancel: {
                        label: '<i class="fa fa-times"></i> No',
                        className: 'btn-danger'
                    },
                    confirm: {
                        label: '<i class="fa fa-check"></i> Yes',
                        className: 'btn-success'
                    }
                },
                callback: function(result) {
                    if (result == true) {
                        deleteBook(bookID);
                    } else {}
                }
            });
        } else {
            alert.removeClass("alert-success").removeClass("alert-warning").addClass("alert-danger").text("The lended book can not be deleted.").fadeIn().fadeOut(6000);
        }
    })

    // lend book button
    $("#booksTable").on("click", "button.lendBook", function(e) {
        var bookID = $(this).closest("tr").attr("data-book-id");
        var readerID = 0;

        bootbox.alert({
            message: "Choose the reader.",
            backdrop: true
        });

        $("#readersTable").on("click", "button.selectReader", function(e) {
            readerID = $(this).closest("tr").attr("data-reader-id");

            var newLendBook = {
                BookID: bookID,
                ReaderID: readerID,
            }

            if (bookID > 0 && readerID > 0) {
                getAllBooks();
                getAllReaders();
                lendBook(newLendBook);
            } else {
                alert.removeClass("alert-success").removeClass("alert-danger").addClass("alert-warning").text("Incorrect data. Try again.").fadeIn();
            }
        })
    })

    // get all books
    function getAllBooks() {

        $.ajax({
            url: apiURL + "books/"
        }).done(function(response) {
            renderAllBooks(response);
        }).fail(function(error) {})
    }
    // render all books to table
    function renderAllBooks(books) {
        var booksTable = $("#booksTable").find("tbody");
        for (var i = 0; i < books.length; i++) {
            var newRow = $("<tr data-book-id=" + books[i].ID + "></tr>");

            var titleCol = $("<td>").text(books[i].Title);
            titleCol.appendTo(newRow);
            var authorCol = $("<td>").text(books[i].Author);
            authorCol.appendTo(newRow);

            var buttons = $(`<td><div class="button-group">
            <button class="btn btn-primary btn-sm editBook">Edit</button>
            <button class="btn btn-danger btn-sm deleteBook">Delete</button>
            <button class="btn btn-info btn-sm lendBook">Lend</button>
            </div>
            </td>`);
            buttons.appendTo(newRow);
            newRow.appendTo(booksTable);
        }
    }

    // get one book
    function getOneBook(bookID) {
        $.ajax({
            url: apiURL + "books/" + bookID
        }).done(function(resp) {
            renderBook(resp);
        }).fail(function(err) {})
    }

    // render one book to table
    function renderBook(book) {
        var booksTable = $("#booksTable").find("tbody");

        var newRow = $("<tr data-book-id=" + book.ID + "></tr>");
        var titleCol = $("<td>").text(book.Title);
        titleCol.appendTo(newRow);

        var authorCol = $("<td>").text(book.Author);
        authorCol.appendTo(newRow);
        var buttons = $(`<td><div class="button-group">
                <button class="btn btn-primary btn-sm editBook">Edit</button>
                <button class="btn btn-danger btn-sm deleteBook">Delete</button>
                <button class="btn btn-info btn-sm lendBook">Lend</button>
                </div>
                </td>`);

        buttons.appendTo(newRow);
        newRow.appendTo(booksTable);
    }

    // READERS -> FUNCTIONS
    getAllReaders();

    // get all readers
    function getAllReaders() {

        $.ajax({
            url: apiURL + "readers/"
        }).done(function(response) {
            renderAllReaders(response);

        }).fail(function(error) {})
    }

    function renderAllReaders(readers) {
        var readersTable = $("#readersTable").find("tbody");
        for (var i = 0; i < readers.length; i++) {
            var newRow = $("<tr data-reader-id=" + readers[i].ID + "></tr>");
            var nameCol = $("<td>").text(readers[i].Name);
            nameCol.appendTo(newRow);

            var ageCol = $("<td>").text(readers[i].Age);
            ageCol.appendTo(newRow);
            var buttons = $(`<td><div class="button-group">
        <button class="btn btn-primary btn-sm editReader">Edit</button>
        <button class="btn btn-danger btn-sm deleteReader">Delete</button>
        <button class="btn btn-info btn-sm selectReader">Select</button>
        </div>
        </td>`);

            buttons.appendTo(newRow);
            newRow.appendTo(readersTable);
        }
    }

    // add new reader function
    function addNewReader() {
        var name = $("input[name='readerName']").val();
        var age = $("input[name='readerAge']").val();

        var tempArr = name.split(" ");
        var wordsInName = tempArr.length;

        if (name != "" && name.length > 1 && wordsInName > 1 && age != "" && age > 0 && age < 130) {
            var newReader = {
                Name: name,
                Age: age
            }
            $.ajax({
                url: apiURL + "readers",
                type: "POST",
                data: newReader
            }).done(function(resp) {
                alert.removeClass("alert-warning").removeClass("alert-danger").addClass("alert-success").text("The reader has been successfully added.").fadeIn("slow").fadeOut(6000);
                $("input[name='readerName']").val("");
                $("input[name='readerAge']").val("");

                $("#readersTable").find("tbody").empty();
                getAllReaders();

            }).fail(function(err) {
                alert.removeClass("alert-success").removeClass("alert-warning").addClass("alert-danger").text("The reader already exists in the database!").fadeIn();
            })
        } else {
            alert.removeClass("alert-success").removeClass("alert-danger").addClass("alert-warning").text("Incorrect data. Try again.").fadeIn();
        }
    }

    // edit reader
    function editReader(ID, editedReader) {

        var modifiedReader = {
            Name: editedReader.Name,
            Age: editedReader.Age
        }

        $.ajax({
            url: apiURL + "readers/" + ID,
            type: "PUT",
            data: modifiedReader,
        }).done(function(resp) {
            alert.removeClass("alert-warning").removeClass("alert-danger").addClass("alert-success").text("The update was successful.").fadeIn("slow").fadeOut(6000);
            $("input[name='readerName']").val("");
            $("input[name='readerAge']").val("");

            $("#readersTable").find("tbody").empty();
            getAllReaders();
            $("button.saveReader").removeClass("saveReader").addClass("addReader").html("<i class='fa fa-plus'></i> Add");
        }).fail(function(err) {
            alert.removeClass("alert-success").removeClass("alert-warning").addClass("alert-danger").text("The reader with the given ID does not exist.").show();
        })
    }

    //delete reader
    function deleteReader(readerID) {

        $.ajax({
            url: apiURL + "readers/" + readerID,
            type: "DELETE",
        }).done(function(resp) {
            alert.removeClass("alert-warning").removeClass("alert-danger").addClass("alert-success").text("The reader has been successfully deleted.").fadeIn("slow").fadeOut(6000);
            $("input[name='readerName']").val("");
            $("input[name='readerAge']").val("");

            $("#readersTable").find("tbody").empty();
            getAllReaders();

        }).fail(function(err) {
            alert.removeClass("alert-success").removeClass("alert-warning").addClass("alert-danger").text("The reader with the given ID has not been found.").fadeIn();
        })
    }

    // READERS -> EVENTS 

    // add reader event
    $("#readersForm").on("click", "button.addReader", function(e) {
        e.preventDefault();
        addNewReader();
    })

    // edit reader event
    $("#readersTable").on("click", "button.editReader", function(e) {
        e.preventDefault();

        var readerID = $(this).closest("tr").attr("data-reader-id");
        var name = $(this).closest("tr").children().eq(0).text();
        var age = $(this).closest("tr").children().eq(1).text();
        var saveButton = $("button.addReader").removeClass("addReader").addClass("saveReader").html("<i class='fa fa-plus'></i> Save");

        var nameInput = $("input[name='readerName']").val(name);
        var ageInput = $("input[name='readerAge']").val(age);
        var newName = "";
        var newAge = "";

        saveButton.on("click", function() {
            newName = nameInput.val();
            newAge = ageInput.val();

            if (readerID > 0 && newName != "" && newName.length > 0 && age > 0 && age < 130) {
                var editedReader = {
                    Name: newName,
                    Age: newAge
                }
                editReader(readerID, editedReader);
            } else {
                alert.removeClass("alert-success").removeClass("alert-danger").addClass("alert-warning").text("No data for the update. Try again.").fadeIn();
            }
        })
    })


    // delete book event
    $("#readersTable").on("click", "button.deleteReader", function(e) {
        var readerID = $(this).closest("tr").attr("data-reader-id");
        var lendBookID = $("#lendedBooksTable").find("tbody").find("tr").filter(`[data-reader-id='${readerID}']`).attr("data-reader-id");

        if (readerID != lendBookID) {
            bootbox.confirm({
                title: "Delete the reader.",
                message: "Are you sure you want to delete the selected reader?",
                buttons: {
                    cancel: {
                        label: '<i class="fa fa-times"></i> No',
                        className: 'btn-danger'
                    },
                    confirm: {
                        label: '<i class="fa fa-check"></i> Yes',
                        className: 'btn-success'
                    }
                },
                callback: function(result) {
                    if (result == true) {
                        deleteReader(readerID);
                    } else {
                        alert.removeClass("alert-success").removeClass("alert-danger").addClass("alert-warning").text("Incorrect data. Try again.").fadeIn();
                    }
                }
            });
        } else {
            alert.removeClass("alert-success").removeClass("alert-warning").addClass("alert-danger").text("You can not delete a reader who has lended books.").fadeIn().fadeOut(6000);
        }

    })

    // LEND -> FUNCTIONS

    $("#lendedBooksTable").find("tbody").empty();
    getLendedBookList();

    // LEND -> EVENTS

    // return book events
    $("#lendedBooksTable").on("click", "button.returnBook", function(e) {
        var lendID = $(this).closest("tr").attr("data-lendedbook-id");

        bootbox.confirm({
            title: "Return the book.",
            message: "Please confirm returning the book",
            buttons: {
                cancel: {
                    label: '<i class="fa fa-times"></i> Cancel',
                    className: 'btn-danger'
                },
                confirm: {
                    label: '<i class="fa fa-check"></i> OK',
                    className: 'btn-success'
                }
            },
            callback: function(result) {
                if (result == true) {
                    returnBook(lendID);
                } else {
                    alert.removeClass("alert-success").removeClass("alert-danger").addClass("alert-warning").text("Incorrect data. Try again.").fadeIn();
                }
            }
        });
    })

    // get list of lended books
    function getLendedBookList() {

        $.ajax({
            url: apiURL + "lend"
        }).done(function(response) {
            renderLendedBooks(response);

        }).fail(function(error) {})
    }

    function renderLendedBooks(lendedBooks) {
        var lendedBooksTable = $("#lendedBooksTable").find("tbody");
        for (var i = 0; i < lendedBooks.length; i++) {
            var newRow = $("<tr data-lendedBook-id=" + lendedBooks[i].ID + " data-book-id=" + lendedBooks[i].BookID + " data-reader-id=" + lendedBooks[i].ReaderID + "></tr>");
            var titleCol = $("<td>").text(lendedBooks[i].Title);
            titleCol.appendTo(newRow);

            var nameCol = $("<td>").text(lendedBooks[i].Name);
            nameCol.appendTo(newRow);

            var lendDateCol = $("<td>").text(lendedBooks[i].LendDate);
            lendDateCol.appendTo(newRow);

            var buttons = $(`<td><div class="button-group">
        <button class="btn btn-warning btn-sm returnBook">Return</button>
        </div>
        </td>`);

            buttons.appendTo(newRow);
            newRow.appendTo(lendedBooksTable);
        }
    }

    // lend book function
    function lendBook(newLendBook) {

        function yyyymmdd() {
            var now = new Date();
            var y = now.getFullYear();
            var m = now.getMonth() + 1;
            var d = now.getDate();
            var mm = m < 10 ? '0' + m : m;
            var dd = d < 10 ? '0' + d : d;
            return '' + y + "-" + mm + "-" + dd;
        }
        var lendDate = yyyymmdd();

        var lendBook = {
            BookID: newLendBook.BookID,
            ReaderID: newLendBook.ReaderID,
            LendDate: lendDate
        }

        $.ajax({
            url: apiURL + "lend",
            type: "POST",
            data: lendBook
        }).done(function(resp) {
            alert.removeClass("alert-warning").removeClass("alert-danger").addClass("alert-success").text("The book has been lended.").fadeIn("slow").fadeOut(6000);
            $("#lendedBooksTable").find("tbody").empty();
            getLendedBookList();

        }).fail(function(err) {
            alert.removeClass("alert-success").removeClass("alert-warning").addClass("alert-danger").text("The book is already lended!").fadeIn();
        })
    }

    // return book function
    function returnBook(lendID) {
        $.ajax({
            url: apiURL + "lend/" + lendID,
            type: "DELETE",
        }).done(function(resp) {
            alert.removeClass("alert-warning").removeClass("alert-danger").addClass("alert-success").text("The book has been returned.").fadeIn("slow").fadeOut(6000);

            $("#lendedBooksTable").find("tbody").empty();
            getLendedBookList();

        }).fail(function(err) {
            alert.removeClass("alert-success").removeClass("alert-warning").addClass("alert-danger").text("The lending with the given ID has not been found.").fadeIn();
        })
    }
})