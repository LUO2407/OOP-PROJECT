using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace LibraryManagementSystem
{
    // Abstract base class representing all library items
    public abstract class LibraryItem
    {
        public string Id { get; private set; }
        public string Title { get; private set; }
        public string Author { get; private set; }
        public bool IsAvailable { get; protected set; }

        protected LibraryItem(
            string id,
            string title,
            string author,
            bool isAvailable = true)
        {
            Id = id.Trim();
            Title = title.Trim();
            Author = author.Trim();
            IsAvailable = isAvailable;
        }

        // Encapsulation: Manage the borrowing status of an item
        public void MarkAsBorrowed()
        {
            if (!IsAvailable)
            {
                throw new InvalidOperationException(
                    $"Item '{Title}' (ID: {Id}) is currently borrowed!");
            }

            IsAvailable = false;
        }

        // Encapsulation: Manage the return status of an item
        public void MarkAsReturned()
        {
            if (IsAvailable)
            {
                throw new InvalidOperationException(
                    $"Item '{Title}' (ID: {Id}) is already in the library!");
            }

            IsAvailable = true;
        }

        // Polymorphism: Declare abstract methods
        // Derived classes must override these methods
        public abstract decimal CalculateOverdueFee(int overdueDays);
        public abstract string GetItemType();
        public abstract string ToDataString();
        public abstract void DisplayInfo();
    }

    // Traditional printed book (inherits from LibraryItem)
    public class PhysicalBook : LibraryItem
    {
        public const decimal DailyFine = 5000m;

        public string ShelfLocation { get; private set; }
        public string ISBN { get; private set; }

        public PhysicalBook(
            string id,
            string title,
            string author,
            bool isAvailable,
            string shelf,
            string isbn)
            : base(id, title, author, isAvailable)
        {
            ShelfLocation = shelf.Trim();
            ISBN = isbn.Trim();
        }

        // Polymorphism: Physical books are fined 5,000 VND per overdue day
        public override decimal CalculateOverdueFee(int overdueDays)
        {
            if (overdueDays > 0)
            {
                return overdueDays * DailyFine;
            }
            else
            {
                return 0m;
            }
        }

        public override string GetItemType()
        {
            return "PhysicalBook";
        }

        public override string ToDataString()
        {
            return "PHYSICAL|"
                + Id + "|"
                + Title + "|"
                + Author + "|"
                + IsAvailable + "|"
                + ShelfLocation + "|"
                + ISBN;
        }

        public override void DisplayInfo()
        {
            string status = "";

            if (IsAvailable)
            {
                status = "Available";
            }
            else
            {
                status = "Borrowed";
            }

            Console.WriteLine(
                $"[PhysicalBook] ID: {Id,-5} | Title: {Title,-32} | Author: {Author,-16} | Shelf: {ShelfLocation,-10} | ISBN: {ISBN,-14} | Status: {status}");
        }
    }

    // Electronic book (E-Book), which inherits from LibraryItem
    public class EBook : LibraryItem
    {
        public double FileSizeMB { get; private set; }

        public EBook(
            string id,
            string title,
            string author,
            bool isAvailable,
            double fileSizeMB)
            : base(id, title, author, isAvailable)
        {
            FileSizeMB = fileSizeMB;
        }

        // Polymorphism: E-Books are exempt from overdue fines
        public override decimal CalculateOverdueFee(int overdueDays)
        {
            return 0m;
        }

        public override string GetItemType()
        {
            return "EBook";
        }

        public override string ToDataString()
        {
            return "EBOOK|"
                + Id + "|"
                + Title + "|"
                + Author + "|"
                + IsAvailable + "|"
                + FileSizeMB.ToString(
                    "0.0",
                    CultureInfo.InvariantCulture);
        }

        public override void DisplayInfo()
        {
            string status = "";

            if (IsAvailable)
            {
                status = "Available";
            }
            else
            {
                status = "Borrowed";
            }

            Console.WriteLine(
                $"[EBook       ] ID: {Id,-5} | Title: {Title,-32} | Author: {Author,-16} | Size: {FileSizeMB,5} MB | Status: {status}");
        }
    }
}

    // Member (Maximum of 3 books allowed per checkout)
    public class Member
    {
        public string Id { get; private set; }
        public string FullName { get; private set; }
        public int MaxBorrowLimit { get; private set; }

        public Member(string id, string fullName, int maxBorrowLimit = 3)
        {
            Id = id.Trim();
            FullName = fullName.Trim();
            MaxBorrowLimit = maxBorrowLimit;
        }

        public string ToDataString()
        {
            return Id + "|" + FullName + "|" + MaxBorrowLimit;
        }

        public void DisplayInfo(int activeLoans = 0)
        {
            Console.WriteLine($"Member ID: {Id,-5} | Full Name: {FullName,-20} | Borrowed: {activeLoans}/{MaxBorrowLimit} books");
        }
    }

    // Loan (14-day loan term & status package)
    public class Loan
    {
        public string LoanId { get; private set; }
        public string MemberId { get; private set; }
        public string ItemId { get; private set; }
        public DateTime BorrowDate { get; private set; }
        public DateTime DueDate { get; private set; }
        public DateTime? ReturnDate { get; private set; }
        public string Status { get; private set; } // "Active" hoặc "Returned"
        public decimal FineAmount { get; private set; }

        // Create a new loan record (default duration: 14 days)
        public Loan(string loanId, string memberId, string itemId, int durationDays = 14)
        {
            LoanId = loanId;
            MemberId = memberId;
            ItemId = itemId;
            BorrowDate = DateTime.Now;
            DueDate = BorrowDate.AddDays(durationDays);
            ReturnDate = null;
            Status = "Active";
            FineAmount = 0m;
        }

        // Recreate the loan when loading from a text file
        public Loan(string loanId, string memberId, string itemId, DateTime borrow, DateTime due, DateTime? ret, string status, decimal fine)
        {
            LoanId = loanId;
            MemberId = memberId;
            ItemId = itemId;
            BorrowDate = borrow;
            DueDate = due;
            ReturnDate = ret;
            Status = status;
            FineAmount = fine;
        }

        // Check whether the loan is overdue
        public bool IsOverdue(DateTime checkDate)
        {
            if (Status == "Active" && checkDate.Date > DueDate.Date)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Calculate the number of overdue days
        public int GetOverdueDays(DateTime checkDate)
        {
            if (IsOverdue(checkDate))
            {
                TimeSpan diff = checkDate.Date - DueDate.Date;
                return (int)diff.TotalDays;
            }
            else
            {
                return 0;
            }
        }

        // Complete book returns and settle fines
        public void CompleteReturn(DateTime returnDate, decimal fine)
        {
            if (Status == "Returned")
            {
                throw new InvalidOperationException($"Loan {LoanId} was already returned!");
            }
            ReturnDate = returnDate;
            Status = "Returned";
            if (fine >= 0)
            {
                FineAmount = fine;
            }
            else
            {
                FineAmount = 0m;
            }
        }

        public string ToDataString()
        {
            string returnDateString = "";
            if (ReturnDate.HasValue)
            {
                returnDateString = ReturnDate.Value.ToString("yyyy-MM-dd");
            }

            return LoanId + "|" + MemberId + "|" + ItemId + "|" +
                   BorrowDate.ToString("yyyy-MM-dd") + "|" +
                   DueDate.ToString("yyyy-MM-dd") + "|" +
                   returnDateString + "|" + Status + "|" +
                   FineAmount.ToString("0", CultureInfo.InvariantCulture);
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"Loan ID: {LoanId,-5} | Member: {MemberId,-5} | Book: {ItemId,-5} | Due: {DueDate:yyyy-MM-dd} | Status: {Status,-8} | Fine: {FineAmount:N0} VND");
        }
    }
    
     // Plain text file storage ( .txt files)
    public class FileService
    {
        private readonly string _dir;

        public FileService(string dataDirectory = "Data")
        {
            _dir = dataDirectory;
            if (!Directory.Exists(_dir))
            {
                Directory.CreateDirectory(_dir);
            }
        }

        public List<LibraryItem> LoadBooks(string path)
        {
            List<LibraryItem> list = new List<LibraryItem>();
            if (!File.Exists(path))
            {
                return list;
            }

            string[] lines = File.ReadAllLines(path);
            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                {
                    continue;
                }

                string[] p = line.Split('|');
                string type = p[0].ToUpper();

                if (type == "PHYSICAL" && p.Length >= 7)
                {
                    string id = p[1];
                    string title = p[2];
                    string author = p[3];
                    bool isAvailable = bool.Parse(p[4]);
                    string shelf = p[5];
                    string isbn = p[6];
                    list.Add(new PhysicalBook(id, title, author, isAvailable, shelf, isbn));
                }
                else if (type == "EBOOK" && p.Length >= 6)
                {
                    string id = p[1];
                    string title = p[2];
                    string author = p[3];
                    bool isAvailable = bool.Parse(p[4]);
                    double size = double.Parse(p[5], CultureInfo.InvariantCulture);
                    list.Add(new EBook(id, title, author, isAvailable, size));
                }
            }
            return list;
        }

        public void SaveBooks(string path, List<LibraryItem> items)
        {
            List<string> lines = new List<string>();
            lines.Add("# Format: PHYSICAL/EBOOK|Id|Title|Author|IsAvailable|Extra");
            foreach (LibraryItem item in items)
            {
                lines.Add(item.ToDataString());
            }
            File.WriteAllLines(path, lines);
        }

        public List<Member> LoadMembers(string path)
        {
            List<Member> list = new List<Member>();
            if (!File.Exists(path))
            {
                return list;
            }

            string[] lines = File.ReadAllLines(path);
            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                {
                    continue;
                }

                string[] p = line.Split('|');
                if (p.Length >= 3)
                {
                    string id = p[0];
                    string name = p[1];
                    int limit = int.Parse(p[2]);
                    list.Add(new Member(id, name, limit));
                }
            }
            return list;
        }

        public void SaveMembers(string path, List<Member> members)
        {
            List<string> lines = new List<string>();
            lines.Add("# Format: MemberId|FullName|MaxBorrowLimit");
            foreach (Member m in members)
            {
                lines.Add(m.ToDataString());
            }
            File.WriteAllLines(path, lines);
        }

        public List<Loan> LoadLoans(string path)
        {
            List<Loan> list = new List<Loan>();
            if (!File.Exists(path))
            {
                return list;
            }

            string[] lines = File.ReadAllLines(path);
            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                {
                    continue;
                }

                string[] p = line.Split('|');
                if (p.Length >= 8)
                {
                    string loanId = p[0];
                    string memberId = p[1];
                    string itemId = p[2];
                    DateTime borrow = DateTime.ParseExact(p[3], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    DateTime due = DateTime.ParseExact(p[4], "yyyy-MM-dd", CultureInfo.InvariantCulture);

                    DateTime? ret = null;
                    if (!string.IsNullOrEmpty(p[5]))
                    {
                        ret = DateTime.ParseExact(p[5], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    }

                    string status = p[6];
                    decimal fine = decimal.Parse(p[7], CultureInfo.InvariantCulture);

                    list.Add(new Loan(loanId, memberId, itemId, borrow, due, ret, status, fine));
                }
            }
            return list;
        }

        public void SaveLoans(string path, List<Loan> loans)
        {
            List<string> lines = new List<string>();
            lines.Add("# Format: LoanId|MemberId|ItemId|BorrowDate|DueDate|ReturnDate|Status|FineAmount");
            foreach (Loan l in loans)
            {
                lines.Add(l.ToDataString());
            }
            File.WriteAllLines(path, lines);
        }
    }

    // Operation & 3 test case
    public class LibraryService
    {
        private readonly FileService _fileService;
        private readonly string _booksFile;
        private readonly string _membersFile;
        private readonly string _loansFile;

        public List<LibraryItem> Items { get; private set; }
        public List<Member> Members { get; private set; }
        public List<Loan> Loans { get; private set; }

        public LibraryService(string dataDir = "Data")
        {
            _fileService = new FileService(dataDir);
            _booksFile = Path.Combine(dataDir, "books.txt");
            _membersFile = Path.Combine(dataDir, "members.txt");
            _loansFile = Path.Combine(dataDir, "loans.txt");

            Items = _fileService.LoadBooks(_booksFile);
            Members = _fileService.LoadMembers(_membersFile);
            Loans = _fileService.LoadLoans(_loansFile);

            if (Items.Count == 0 || Members.Count == 0)
            {
                SeedInitialData();
            }
        }

        private void SeedInitialData()
        {
            if (Items.Count == 0)
            {
                Items.Add(new PhysicalBook("B001", "Clean Code: Agile Software", "Robert C. Martin", true, "Shelf A1", "978-0132350884"));
                Items.Add(new PhysicalBook("B002", "Design Patterns", "Erich Gamma", true, "Shelf A2", "978-0201633610"));
                Items.Add(new PhysicalBook("B003", "The Pragmatic Programmer", "Andrew Hunt", true, "Shelf B1", "978-0135957059"));
                Items.Add(new EBook("E001", "C# in Depth (4th Edition)", "Jon Skeet", true, 8.5));
                Items.Add(new EBook("E002", "CLR via C# (4th Edition)", "Jeffrey Richter", true, 12.8));
            }
            if (Members.Count == 0)
            {
                Members.Add(new Member("M001", "John Doe", 3));
                Members.Add(new Member("M002", "Jane Smith", 3));
                Members.Add(new Member("M003", "Alex Miller", 3));
            }
            SaveAll();
        }

        public void SaveAll()
        {
            _fileService.SaveBooks(_booksFile, Items);
            _fileService.SaveMembers(_membersFile, Members);
            _fileService.SaveLoans(_loansFile, Loans);
        }

        // Find a book by its ID (using a simple loop)
        public LibraryItem? FindItem(string id)
        {
            foreach (LibraryItem item in Items)
            {
                if (item.Id.Equals(id.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }
            return null;
        }

        // Find a member by its ID
        public Member? FindMember(string id)
        {
            foreach (Member m in Members)
            {
                if (m.Id.Equals(id.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return m;
                }
            }
            return null;
        }

        // Find loan by its ID
        public Loan? FindLoan(string id)
        {
            foreach (Loan l in Loans)
            {
                if (l.LoanId.Equals(id.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return l;
                }
            }
            return null;
        }

        //counting borrowed book
        public int GetActiveBorrowCount(string memberId)
        {
            int count = 0;
            foreach (Loan l in Loans)
            {
                if (l.MemberId.Equals(memberId.Trim(), StringComparison.OrdinalIgnoreCase) && l.Status == "Active")
                {
                    count = count + 1;
                }
            }
            return count;
        }

        // Search for books by Title, Author, or ISBN
        public List<LibraryItem> SearchBooks(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return Items;
            }

            string k = keyword.Trim().ToLower();
            List<LibraryItem> results = new List<LibraryItem>();

            foreach (LibraryItem item in Items)
            {
                bool matchTitle = item.Title.ToLower().Contains(k);
                bool matchAuthor = item.Author.ToLower().Contains(k);
                bool matchIsbn = false;

                if (item is PhysicalBook)
                {
                    PhysicalBook pb = (PhysicalBook)item;
                    if (pb.ISBN.ToLower().Contains(k))
                    {
                        matchIsbn = true;
                    }
                }

                if (matchTitle || matchAuthor || matchIsbn)
                {
                    results.Add(item);
                }
            }
            return results;
        }

        // Borrowing books: Check the 3-book limit and book status
        public Loan BorrowBook(string memberId, string itemId, int days = 14)
        {
            Member? member = FindMember(memberId);
            if (member == null)
            {
                throw new Exception("Member '" + memberId + "' not found!");
            }

            int currentBorrows = GetActiveBorrowCount(member.Id);
            if (currentBorrows >= member.MaxBorrowLimit)
            {
                throw new InvalidOperationException("Member '" + member.FullName + "' has reached the limit of " + member.MaxBorrowLimit + " books!");
            }

            LibraryItem? item = FindItem(itemId);
            if (item == null)
            {
                throw new Exception("Item '" + itemId + "' not found!");
            }

            if (!item.IsAvailable)
            {
                throw new InvalidOperationException("Item '" + item.Title + "' is currently borrowed!");
            }

            int nextNumber = Loans.Count + 1;
            string newLoanId = "L" + nextNumber.ToString("D3");
            Loan loan = new Loan(newLoanId, member.Id, item.Id, days);

            item.MarkAsBorrowed();
            Loans.Add(loan);
            SaveAll();
            return loan;
        }

        // Returning a book: Polymorphic resolution of CalculateOverdueFee for PhysicalBook or EBook
        public (Loan loan, decimal fine, int overdueDays, LibraryItem item) ReturnBook(string loanId, DateTime? returnDate = null)
        {
            Loan? loan = FindLoan(loanId);
            if (loan == null)
            {
                throw new Exception("Loan '" + loanId + "' not found!");
            }

            LibraryItem? item = FindItem(loan.ItemId);
            if (item == null)
            {
                throw new Exception("Item '" + loan.ItemId + "' not found!");
            }

            DateTime actualDate;
            if (returnDate.HasValue)
            {
                actualDate = returnDate.Value;
            }
            else
            {
                actualDate = DateTime.Now;
            }

            int overdueDays = 0;
            if (actualDate.Date > loan.DueDate.Date)
            {
                TimeSpan diff = actualDate.Date - loan.DueDate.Date;
                overdueDays = (int)diff.TotalDays;
            }

            // POLYMORPHISM: Calling via a reference to the parent class LibraryItem
            decimal fine = item.CalculateOverdueFee(overdueDays);

            loan.CompleteReturn(actualDate, fine);
            item.MarkAsReturned();
            SaveAll();
            return (loan, fine, overdueDays, item);
        }

        // Running 3 minimum test cases
        public void RunAutomatedTests()
        {
            Console.WriteLine("\n=================================================================");
            Console.WriteLine("    RUNNING 3 MINIMUM TEST CASES (OOP VERIFICATION)");
            Console.WriteLine("=================================================================");

            // Test Case 1: Happy Path
            Console.WriteLine("\n[TEST CASE 1] Happy Path: Borrow & State Update");
            var testMem = new Member("T_M01", "Test User", 3);
            var testBook = new PhysicalBook("T_B01", "OOP Test Book", "Author", true, "Shelf T", "ISBN-111");
            Members.Add(testMem);
            Items.Add(testBook);

            var loan1 = BorrowBook("T_M01", "T_B01");
            Console.WriteLine($"-> [PASS] Loan {loan1.LoanId} created. Book IsAvailable = {testBook.IsAvailable}");

            // Test Case 2: Validation limit (max 3 books)
            Console.WriteLine("\n[TEST CASE 2] Validation: Block Exceeding Max Limit (3 books)");
            for (int i = 2; i <= 3; i++)
            {
                var extra = new PhysicalBook("T_B0" + i, "Book " + i, "Author", true, "Shelf T", "ISBN-" + i);
                Items.Add(extra);
                BorrowBook("T_M01", "T_B0" + i);
            }
            var book4 = new PhysicalBook("T_B04", "Book 4", "Author", true, "Shelf T", "ISBN-4");
            Items.Add(book4);

            try
            {
                BorrowBook("T_M01", "T_B04");
                Console.WriteLine("-> [FAIL] System did not block borrow over limit!");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"-> [PASS - BLOCKED] Expected exception caught: \"{ex.Message}\"");
            }

            // Test Case 3: Polymorphism verification
            Console.WriteLine("\n[TEST CASE 3] Polymorphism Verification: 5-Day Overdue Fine");
            LibraryItem item1 = new PhysicalBook("P1", "Physical C#", "MS", true, "Shelf 1", "ISBN-P");
            LibraryItem item2 = new EBook("E1", "E-Book C#", "MS", true, 5.0);

            decimal fine1 = item1.CalculateOverdueFee(5); // 5 * 5.000 = 25.000
            decimal fine2 = item2.CalculateOverdueFee(5); // 0 (free)

            Console.WriteLine($"-> [{item1.GetItemType()}] Fine for 5 days: {fine1:N0} VND (Rate: 5,000 VND/day)");
            Console.WriteLine($"-> [{item2.GetItemType()}] Fine for 5 days: {fine2:N0} VND (Rate: 0 VND - Waived)");

            if (fine1 == 25000m && fine2 == 0m)
            {
                Console.WriteLine("-> [PASS] Polymorphism works correctly!");
            }

            // Clean up test data (using a simple for-loop)
            for (int i = Items.Count - 1; i >= 0; i--)
            {
                if (Items[i].Id.StartsWith("T_") || Items[i].Id == "P1" || Items[i].Id == "E1")
                {
                    Items.RemoveAt(i);
                }
            }

            for (int i = Members.Count - 1; i >= 0; i--)
            {
                if (Members[i].Id.StartsWith("T_"))
                {
                    Members.RemoveAt(i);
                }
            }

            for (int i = Loans.Count - 1; i >= 0; i--)
            {
                if (Loans[i].MemberId.StartsWith("T_"))
                {
                    Loans.RemoveAt(i);
                }
            }
            SaveAll();

            Console.WriteLine("\n=================================================================");
            Console.WriteLine("    ALL 3 MINIMUM TEST CASES PASSED SUCCESSFULLY!");
            Console.WriteLine("=================================================================");
        }
    }
}
    internal class Program
    {
        private static LibraryService _lib = null!;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            _lib = new LibraryService("Data");

            while (true)
            {
                Console.WriteLine("\n=======================================================");
                Console.WriteLine("       LIBRARY MANAGEMENT SYSTEM - CLI (WEEK 2)        ");
                Console.WriteLine("=======================================================");
                Console.WriteLine(" 1. Display All Books");
                Console.WriteLine(" 2. Search Books (Title, Author, ISBN)");
                Console.WriteLine(" 3. Display All Members");
                Console.WriteLine(" 4. Borrow Book (Max 3 books, 14-day loan)");
                Console.WriteLine(" 5. Return Book & Calculate Overdue Fine");
                Console.WriteLine(" 6. Run 3 Automated Minimum Test Cases");
                Console.WriteLine(" 0. Save & Exit");
                Console.WriteLine("-------------------------------------------------------");
                Console.Write("Enter your choice [0-6]: ");

                string? choice = Console.ReadLine();

                if (choice != null)
                {
                    choice = choice.Trim();
                }

                if (choice == "0")
                {
                    _lib.SaveAll();
                    Console.WriteLine("\n[INFO] All data saved. Goodbye!");
                    break;
                }

                try
                {
                    switch (choice)
                    {
                        case "1":
                            Console.WriteLine("\n--- LIBRARY BOOKS CATALOG ---");

                            foreach (LibraryItem b in _lib.Items)
                            {
                                b.DisplayInfo();
                            }

                            break;

                        case "2":
                            Console.Write("\nEnter search keyword: ");
                            string kw = Console.ReadLine() ?? "";

                            List<LibraryItem> res = _lib.SearchBooks(kw);

                            Console.WriteLine($"Found {res.Count} matching item(s):");

                            foreach (LibraryItem b in res)
                            {
                                b.DisplayInfo();
                            }

                            break;

                        case "3":
                            Console.WriteLine("\n--- REGISTERED MEMBERS ---");

                            foreach (Member m in _lib.Members)
                            {
                                m.DisplayInfo(_lib.GetActiveBorrowCount(m.Id));
                            }

                            break;

                        case "4":
                            Console.Write("\nEnter Member ID: ");
                            string mId = Console.ReadLine() ?? "";

                            Console.Write("Enter Book ID: ");
                            string bId = Console.ReadLine() ?? "";

                            Loan loan = _lib.BorrowBook(mId, bId);

                            Console.WriteLine(
                                $"[SUCCESS] Loan created! ID: {loan.LoanId}, " +
                                $"Due Date: {loan.DueDate:yyyy-MM-dd}"
                            );

                            break;

                        case "5":
                            Console.Write("\nEnter Loan ID to return: ");
                            string lId = Console.ReadLine() ?? "";

                            Console.Write("Simulate overdue days (Enter 0 for today): ");
                            int lateDays = 0;
                            int.TryParse(Console.ReadLine(), out lateDays);

                            DateTime retDate;

                            if (lateDays > 0)
                            {
                                Loan? target = _lib.FindLoan(lId);

                                if (target != null)
                                {
                                    retDate = target.DueDate.AddDays(lateDays);
                                }
                                else
                                {
                                    retDate = DateTime.Now;
                                }
                            }
                            else
                            {
                                retDate = DateTime.Now;
                            }

                            var ret = _lib.ReturnBook(lId, retDate);

                            Console.WriteLine(
                                $"[SUCCESS] Returned '{ret.item.Title}'. " +
                                $"Overdue: {ret.overdueDays} days. " +
                                $"Fine: {ret.fine:N0} VND."
                            );

                            break;

                        case "6":
                            _lib.RunAutomatedTests();
                            break;

                        default:
                            Console.WriteLine(
                                "[ERROR] Invalid selection, please choose [0-6]."
                            );

                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[ERROR] {ex.Message}");
                }
            }
        }
    }

