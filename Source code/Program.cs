using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace LibraryManagementSystem
{
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
