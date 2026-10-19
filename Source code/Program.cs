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

    protected LibraryItem(string id, string title, string author, bool isAvailable = true)
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
            throw new InvalidOperationException($"Item '{Title}' (ID: {Id}) is currently borrowed!");
        }
        IsAvailable = false;
    }

    // Encapsulation: Manage the return status of an item
    public void MarkAsReturned()
    {
        if (IsAvailable)
        {
            throw new InvalidOperationException($"Item '{Title}' (ID: {Id}) is already in the library!");
        }
        IsAvailable = true;
    }

    // Polymorphism: Declare abstract methods
    // Derived classes must override these methods and provide their own implementations
    public abstract decimal CalculateOverdueFee(int overdueDays);
    public abstract string GetItemType();
    public abstract string ToDataString();
    public abstract void DisplayInfo();
}

// Traditional printed book (inherits from LibraryItem)
public class PhysicalBook : LibraryItem
{
    public const decimal DailyFine = 5000m; // Fine of 5,000 VND per overdue day
    public string ShelfLocation { get; private set; }
    public string ISBN { get; private set; }

    public PhysicalBook(string id, string title, string author, bool isAvailable, string shelf, string isbn)
        : base(id, title, author, isAvailable)
    {
        ShelfLocation = shelf.Trim();
        ISBN = isbn.Trim();
    }

    // Polymorphism: Physical books are fined 5,000 VND for each overdue day
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
        return "PHYSICAL|" + Id + "|" + Title + "|" + Author + "|" + IsAvailable + "|" + ShelfLocation + "|" + ISBN;
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

        Console.WriteLine($"[PhysicalBook] ID: {Id,-5} | Title: {Title,-32} | Author: {Author,-16} | Shelf: {ShelfLocation,-10} | ISBN: {ISBN,-14} | Status: {status}");
    }
}

// Electronic book (E-Book), which inherits from LibraryItem
public class EBook : LibraryItem
{
    public double FileSizeMB { get; private set; }

    public EBook(string id, string title, string author, bool isAvailable, double fileSizeMB)
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
        return "EBOOK|" + Id + "|" + Title + "|" + Author + "|" + IsAvailable + "|" + FileSizeMB.ToString("0.0", CultureInfo.InvariantCulture);
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

        Console.WriteLine($"[EBook       ] ID: {Id,-5} | Title: {Title,-32} | Author: {Author,-16} | Size: {FileSizeMB,5} MB | Status: {status}");
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
}
