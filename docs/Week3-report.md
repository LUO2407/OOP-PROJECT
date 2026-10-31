  WEEK 3 REPORT
---
| Member | Assigned Tasks |
| :--- | :--- |
| **Phạm Nguyễn Thùy Trâm** | • **Search Refactoring & CLI Menu:** Refactor `SearchBooks()` using polymorphism (removing manual `is PhysicalBook` type-checking) and integrate the existing `Loan.DisplayInfo()` method into `Program.Main()` to view loan records.<br>• **Documentation & Class Diagram:** Update the class design diagram and author the code refactoring explanation for the Week 3 Report. |

1. Summary of completed work
Search refactoring (polymorphism):
•LibraryItem: added the virtual method MatchesKeyword(), which matches by Title or Author.
•PhysicalBook: overrides it to also match by ISBN. EBook reuses the default version.
•SearchBooks(): now only calls item.MatchesKeyword(keyword), so the manual is PhysicalBook check is gone. An empty keyword returns all items.
CLI menu:
•Menu option 7 loops through all loans and calls the existing Loan.DisplayInfo(). If there are none, it prints "No loan records found."
Input validation checks:
•LibraryItem: throws ArgumentException if id, title, or author is empty or whitespace.
•PhysicalBook / EBook: checks that shelf and isbn are not empty, and fileSizeMB is greater than 0.
•Member: checks that id and fullName are not empty, and maxBorrowLimit is greater than 0.
•Loan: checks that IDs are not empty, duration is greater than 0, and dates make sense (due >= borrow, return >= borrow).
Automated tests (expanded from 3 to 6):
•Test 4: invalid inputs trigger ArgumentException.
•Tests 5 and 6: borrowing an unavailable item or returning a loan twice triggers InvalidOperationException.
•Test data is deleted after the run, so books.txt, members.txt, and loans.txt stay clean.
Class diagram update:
•PhysicalBook and EBook inherit from LibraryItem; PhysicalBook overrides MatchesKeyword().
•LibraryService contains LibraryItem, Member, and Loan lists and uses FileService for storage.
•Loan refers to Member and LibraryItem through MemberId and ItemId.

2. Personal reflection and verification
Before / after code comparison
Component	Before	After
SearchBooks()	Checked is PhysicalBook to search by ISBN.	Calls MatchesKeyword() polymorphically; no type checks.
Loan records	Loan.DisplayInfo() existed but was never used in the menu.	Menu option 7 shows all loans through Loan.DisplayInfo().
LibraryItem / Member	Accepted inputs without checking.	Throws ArgumentException if empty, whitespace, or limit <= 0.
EBook	Saved fileSizeMB directly.	Throws ArgumentException if file size is <= 0.
Loan.CompleteReturn	Only checked if already returned.	Also throws ArgumentException if the return date is earlier than the borrow date.
RunAutomatedTests	Checked only 3 success cases.	Expanded to 6 cases covering error handling, invalid states, and data cleanup.
Key code improvements
•Extensibility: a new item type (for example AudioBook) only needs to override MatchesKeyword(), with no change to SearchBooks() (Open/Closed Principle).
•Data protection: inputs are checked right when objects are created, keeping invalid data out of memory.
•Early error catching: bad data is rejected before it is saved to the text files in Data/, preventing parsing errors when the app restarts.
•Clear error types: ArgumentException for bad inputs and InvalidOperationException for invalid actions. Main() catches them and prints [ERROR] ....

| **Lê Thiện Luân** | • **Input Validation & Automated Testing:** Add input validation guards across `LibraryItem`, `Member`, and `Loan`, and expand `RunAutomatedTests()` with exception-handling test scenarios.<br>• **Documentation & Report Compilation:** Author the Exception Handling section and compile the final Week 3 Report. |
---
Le Thien Luan Report
---
## 1. Summary of completed work

1. **Added input validation checks:**
   - `LibraryItem`: Throws `ArgumentException` if `id`, `title`, or `author` is empty or whitespace.
   - `PhysicalBook` / `EBook`: Checks that `shelf` and `isbn` are not empty, and `fileSizeMB` is greater than 0.
   - `Member`: Checks that `id` and `fullName` are not empty, and `maxBorrowLimit` is greater than 0.
   - `Loan`: Checks that IDs are not empty, borrow duration is greater than 0, and dates make sense (`due >= borrow` and `returnDate >= borrowDate`).

2. **Expanded automated tests (from 3 to 6 cases):**
   - Test 4: Checks that invalid inputs correctly trigger `ArgumentException`.
   - Tests 5 & 6: Checks that borrowing an unavailable item or returning a loan twice triggers `InvalidOperationException`.
   - Code cleanup: Automatically deletes test data after running so text files (`books.txt`, `members.txt`, `loans.txt`) stay safe, and formats method calls neatly on single lines.

---

## 2. Personal reflection and verification

### Before / after code comparison

| Component | Before | After |
| :--- | :--- | :--- |
| `LibraryItem` / `Member` | Accepted inputs without checking. | Throws `ArgumentException` if empty, whitespace, or limit <= 0. |
| `EBook` | Saved `fileSizeMB` directly. | Throws `ArgumentException` if file size is <= 0. |
| `Loan.CompleteReturn` | Only checked if already returned. | Throws `ArgumentException` if return date is earlier than borrow date. |
| `RunAutomatedTests` | Checked only 3 success cases. | Expanded to 6 cases covering error handling, invalid states, and data cleanup. |

### Key code improvements

- **Data protection:** Inputs are checked right when objects are created, keeping invalid data out of memory.
- **Early error catching:** Rejects bad data before saving to text files (`Data/`), preventing file parsing errors when the app restarts.
- **Clear error types:** Uses `ArgumentException` for bad inputs and `InvalidOperationException` for invalid actions.
