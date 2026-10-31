# WEEK 3 REPORT
### Group Progress Summary (Week 3)
<sub>- In Week 3, we completed adding input validation guards across the <code>LibraryItem</code>, <code>Member</code>, and <code>Loan</code> classes to proactively throw exceptions when empty or invalid data is detected. Additionally, I expanded the <code>RunAutomatedTests()</code> method from 3 to 6 automated test scenarios to accurately verify exception handling cases while adding an automatic cleanup mechanism for test data. We authored the exception handling section and compiled the complete Week 3 report.</sub>
---
| Member | Assigned Tasks |
| :--- | :--- |
| **Phạm Nguyễn Thùy Trâm** | • **Search Refactoring & CLI Menu:** Refactor `SearchBooks()` using polymorphism (removing manual `is PhysicalBook` type-checking) and integrate the existing `Loan.DisplayInfo()` method into `Program.Main()` to view loan records.<br>• **Documentation & Class Diagram:** Update the class design diagram and author the code refactoring explanation for the Week 3 Report. |
| **Lê Thiện Luân** | • **Input Validation & Automated Testing:** Add input validation guards across `LibraryItem`, `Member`, and `Loan`, and expand `RunAutomatedTests()` with exception-handling test scenarios.<br>• **Documentation & Report Compilation:** Author the Exception Handling section and compile the final Week 3 Report. |
---
Pham Nguyen Thuy Tram Report
---
## 1. Summary of Completed Work

### Search Refactoring (Polymorphism)

- **LibraryItem:** Added the virtual method `MatchesKeyword()`, which matches by Title or Author.
- **PhysicalBook:** Overrides `MatchesKeyword()` to also match by ISBN. `EBook` reuses the default implementation.
- **SearchBooks():** Now calls `item.MatchesKeyword(keyword)` without manual `is PhysicalBook` type-checking. An empty keyword returns all items.

### CLI Menu

- Added menu option 7 to view all loan records.
- The program loops through all loans and calls the existing `Loan.DisplayInfo()` method.
- If there are no loan records, it displays `"No loan records found."`.

### Class Diagram Update

- `PhysicalBook` and `EBook` inherit from `LibraryItem`. `PhysicalBook` overrides `MatchesKeyword()`.
- `LibraryService` contains lists of `LibraryItem`, `Member`, and `Loan`, and uses `FileService` for data storage.
- `Loan` refers to `Member` and `LibraryItem` through `MemberId` and `ItemId`.

## 2. Personal Reflection and Verification

### Before / After Code Comparison

| Component | Before | After |
|---|---|---|
| `SearchBooks()` | Checked `is PhysicalBook` to search by ISBN. | Calls `MatchesKeyword()` polymorphically without type checks. |
| Loan records | `Loan.DisplayInfo()` existed but was not used in the menu. | Menu option 7 displays all loans through `Loan.DisplayInfo()`. |
| `LibraryItem` / `Member` | Accepted inputs without validation. | Throws `ArgumentException` for empty or invalid inputs. |
| `EBook` | Saved `FileSizeMB` directly. | Throws `ArgumentException` if file size is <= 0. |
| `Loan.CompleteReturn()` | Only checked whether the loan was already returned. | Also throws `ArgumentException` if the return date is earlier than the borrow date. |
| `RunAutomatedTests()` | Checked only 3 test cases. | Expanded to 6 cases covering error handling, invalid states, and data cleanup. |

### Key Code Improvements

- **Extensibility:** A new item type (for example, `AudioBook`) only needs to override `MatchesKeyword()`. No changes to `SearchBooks()` are required (Open/Closed Principle).

- **Data Protection:** Inputs are validated when objects are created, preventing invalid data from entering the system.

- **Early Error Catching:** Invalid data is rejected before being saved to the text files in the `Data/` folder, helping prevent parsing errors when the application restarts.

- **Clear Error Types:** `ArgumentException` is used for invalid inputs, while `InvalidOperationException` is used for invalid operations. `Main()` catches exceptions and displays `[ERROR]` messages.
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

---
WEEK 4 PLAN
---
| Member | Assigned Tasks | 
| :--- | :--- |
| **Phạm Nguyễn Thùy Trâm** | • Implement the `Audiobook` class inheriting from `LibraryItem` (3,000 VND/day overdue fee) *without modifying any legacy class code*.<br>• Draw the final UML Class Diagram matching 100% of the actual C# codebase.<br>• Draft 5 Q&A pairs (focused on OCP, Polymorphism, and UML) and complete the Inheritance & Polymorphism mapping table. | 
| **Lê Thiện Luân** | • Integrate unit tests for `Audiobook`.<br>• Write the self-reflection, and finalize the Week 4 Report.<br>• Draft 5 Q&A pairs (focused on encapsulation, data invariants, and file persistence) and complete the encapsulation and abstraction mapping table. | 
