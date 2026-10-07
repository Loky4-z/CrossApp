namespace Core.Domain;

// Бонус 2: Інваріант, що охоплює ДВІ сутності
public class LibraryService
{
    public void IssueBook(Reader reader, BookCopy book)
    {
        // Ось правило, яке зачіпає одразу дві сутності:
        if (reader.OpenLoansCount >= 5)
            throw new InvalidOperationException("Ліміт: у читача вже є 5 відкритих видач.");

        if (book.IsIssued)
            throw new InvalidOperationException("Ця книга вже видана.");

        // ... логіка видачі ...
    }
}

// Заглушки сутностей просто для демонстрації
public class Reader { public int OpenLoansCount { get; set; } }
public class BookCopy { public bool IsIssued { get; set; } }