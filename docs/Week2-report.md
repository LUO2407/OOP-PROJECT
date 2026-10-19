| Member | Assigned Tasks |
| --- | --- |
| **Phạm Nguyễn Thùy Trâm** | Implement the item hierarchy: abstract class `LibraryItem` and two derived classes `PhysicalBook` and `EBook`, applying Polymorphism to `CalculateOverdueFee`.Implement the search feature to query items by Title, Author, or ISBN using appropriate data structures.Design and build a command-line interface (CLI) to demonstrate the borrow/return lifecycle end-to-end.Author the Self-Explanation section for Inheritance and Polymorphism, detailing the exact classes and methods applied in the codebase.|
| **Lê Thiện Luân** | Implement the domain entities `Member` and `Loan`, applying Encapsulation to manage internal state and borrowing rules (maximum 3 items per member, 14-day loan duration).Build the file persistence layer `FileService`.Code and execute the Minimum Test Cases (Happy Path, validation/error rejection, and Polymorphism verification), exporting test execution logs as evidence.Author the Self-Explanation section for Encapsulation and compile the final Week 2 Report.|
---
Phạm Nguyễn Thùy Trâm report
---
## **I. SYLLABUS MAPPING**

| Core OOP Concept | Specific Class & Method Applied | Design Purpose & Technical Solution | 
| :--- | :--- | :--- | 
| **Inheritance** | • Base class: `public abstract class LibraryItem`<br>• Derived classes: `PhysicalBook : LibraryItem`, `EBook : LibraryItem`<br>• Base constructor invocation: `: base(id, title, author, isAvailable)` | Eliminates code duplication by centralizing common cataloging properties (`Id`, `Title`, `Author`, `IsAvailable`) in the abstract base class. Extends specialized attributes: `ShelfLocation`, `ISBN` for physical books; `FileSizeMB` for e-books. | 
| **Polymorphism** | • Abstract method: `public abstract decimal CalculateOverdueFee(int overdueDays);` in `LibraryItem`<br>• Overridden in: `PhysicalBook.CalculateOverdueFee(...)`<br>• Overridden in: `EBook.CalculateOverdueFee(...)`<br>• Dispatch site: `item.CalculateOverdueFee(...)` | Implements runtime polymorphism to differentiate overdue penalty rules without using procedural `switch` or type-checking statements (`is`). Physical books incur 5,000 VND/day, while digital e-books incur 0 VND penalty. | 
| **Collections & Search** | • Collection: `List<LibraryItem>`<br>• Method: `LibraryService.SearchBooks(keyword)` | Stores library items in an in-memory generic list. Traverses elements to match keywords against Title, Author, or ISBN (case-insensitive) without mutating the catalog. | 
| **CLI & User Interaction** | • `Program.Main()` interactive menu loop<br>• Formatted tabular display: `DisplayInfo()` in `PhysicalBook` and `EBook` | Provides a clean, numbered console interface allowing users to browse, search, borrow, return items, and view formatted tables. | 

## **II. SELF-EXPLANATION OF CORE OOP CONCEPTS**

### 1. Inheritance 
* **Specific Classes & Methods in Codebase:**
  * Base class: `public abstract class LibraryItem`
  * Derived classes: `public class PhysicalBook : LibraryItem` and `public class EBook : LibraryItem`
* **Technical Solution:**
  Derived classes invoke the base constructor via `: base(id, title, author, isAvailable)` to reuse initialization logic, and extend domain-specific fields:
  ```csharp
  // Subclass PhysicalBook inherits from LibraryItem
  public class PhysicalBook : LibraryItem
  {
      public string ShelfLocation { get; private set; }
      public string ISBN { get; private set; }

      public PhysicalBook(string id, string title, string author, bool isAvailable, string shelf, string isbn)
          : base(id, title, author, isAvailable)
      {
          ShelfLocation = shelf.Trim();
          ISBN = isbn.Trim();
      }
  }
  ```
* **Concrete Rationale:**
Physical books and e-books share identical cataloging attributes (`Id`, `Title`, `Author`, `IsAvailable`). Defining these attributes once in `LibraryItem` avoids code duplication. Furthermore, inheritance establishes the `is-a` relationship, allowing different item types to be stored uniformly in a single `List<LibraryItem>`.

---
### 2. Polymorphism 
* **Specific Classes & Methods in Codebase:**
  * Abstract base signature: `public abstract decimal CalculateOverdueFee(int overdueDays);` in `LibraryItem`
  * Subclass override in `PhysicalBook`: Returns `overdueDays * 5000m`
  * Subclass override in `EBook`: Returns `0m`
  * Dispatch site: `decimal fine = item.CalculateOverdueFee(overdueDays);` inside `LibraryService.ReturnBook()`
* **Technical Solution:**
  The abstract method defines the uniform interface. Each subclass implements its specific overdue policy:
  ```csharp
  // PhysicalBook: 5,000 VND / day
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

  // EBook: 0 VND penalty (waived)
  public override decimal CalculateOverdueFee(int overdueDays)
  {
      return 0m;
  }
  ```
* **Concrete Rationale:**
  Physical books occupy shelf space and suffer physical wear, justifying a 5,000 VND/day overdue fine to ensure timely circulation. E-books are digital copies with zero wear and can be revoked automatically, so fines are waived. Polymorphism allows the borrow/return service to compute fees dynamically without knowing the concrete subtype at compile time, strictly adhering to the Open/Closed Principle (OCP).

---

## **III. ARCHITECTURE & CLASS DESIGN**

```text
                       +--------------------------------------------------------------+
                       |                  <<abstract>> LibraryItem                    |
                       +--------------------------------------------------------------+
                       | + Id : string { get; private set; }                          |
                       | + Title : string { get; private set; }                       |
                       | + Author : string { get; private set; }                      |
                       | + IsAvailable : bool { get; protected set; }                 |
                       +--------------------------------------------------------------+
                       | + MarkAsBorrowed() : void                                    |
                       | + MarkAsReturned() : void                                    |
                       | + <<abstract>> CalculateOverdueFee(overdueDays: int): decimal|
                       | + <<abstract>> ToDataString() : string                       |
                       | + <<abstract>> DisplayInfo() : void                          |
                       +--------------------------------------------------------------+
^                                ^
                                       | (Inherits)                     | (Inherits)
             +-------------------------+                                +-------------------------+
             |                                                                                    |
+--------------------------------------+                                        +--------------------------------------+
|             PhysicalBook             |                                        |                EBook                 |
+--------------------------------------+                                        +--------------------------------------+
| + ShelfLocation : string             |                                        | + FileSizeMB : double                |
| + ISBN : string                      |                                        +--------------------------------------+
+--------------------------------------+                                        | + CalculateOverdueFee(overdueDays)   |
| + CalculateOverdueFee(overdueDays)   |                                        |   --> returns 0m                     |
|   --> returns days * 5000m           |                                        | + ToDataString() : string            |
| + ToDataString() : string            |                                        | + DisplayInfo() : void               |
| + DisplayInfo() : void               |                                        +--------------------------------------+
+--------------------------------------+
```
---
Lê Thiện Luân report 
---

## **I. SYLLABUS MAPPING**

| Core OOP Concept / Area | Specific Class & Method Applied | Design Purpose & Technical Solution | 
| :--- | :--- | :--- | 
| **Encapsulation** | • `Member`: `MaxBorrowLimit` (`private set`), constructor initialization<br>• `Loan`: `CompleteReturn(date, fine)`, `ReturnDate`, `Status`, `FineAmount` (`private set`)<br>• `LibraryItem`: `MarkAsBorrowed()`, `MarkAsReturned()`, `IsAvailable` (`protected set`) | Enforces domain invariants and protects internal state integrity. Guarantees that business constraints (max 3 books, loan settlement) cannot be bypassed by external code. | 
| **File Persistence** | • `FileService.LoadBooks()`, `SaveBooks()`<br>• `FileService.LoadMembers()`, `SaveMembers()`<br>• `FileService.LoadLoans()`, `SaveLoans()` | Reads and writes human-readable `.txt` files with `\|` delimiters. Automatically ignores comments (`#`) and restores historical domain state on application startup. | 
| **State Rehydration** | • `Loan` reconstruction constructor: `Loan(id, memberId, itemId, borrow, due, ret, status, fine)` | Reconstructs historical transactions from disk without triggering initial loan date or return validations, preserving audit history. | 
| **Automated Verification** | • `LibraryService.RunAutomatedTests()` | Implements an isolated, automated test harness that executes all 3 minimum test cases and asserts expected outcomes programmatically. | 

---

## **II. SELF-EXPLANATION OF CORE OOP CONCEPTS**

### Encapsulation
* **Specific Classes & Methods in Codebase:**
  * `Member` class: Properties `Id`, `FullName`, and `MaxBorrowLimit` use `private set` to prevent external modification after initialization.
  * `Loan` class: Properties `BorrowDate`, `DueDate`, `ReturnDate`, `Status`, and `FineAmount` are strictly protected with `private set`.
  * `LibraryItem` class: Property `IsAvailable` has `protected set` and can only be altered through `MarkAsBorrowed()` and `MarkAsReturned()`.
* **Technical Solution:**
  State modifications are strictly encapsulated inside dedicated methods containing validation guards:
  ```csharp
  // Encapsulated state transition in Loan
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
  ```
* **Concrete Rationale:**
  Without encapsulation, external calling code could directly write `loan.Status = "Returned"` without recording the return timestamp, computing late fees, or marking the associated book available again. Encapsulation guarantees that all state transitions follow rigid business rules, preventing inconsistent data states.

---

## **III. FILE PERSISTENCE ARCHITECTURE**

The system stores persistent data in the `Data/` directory using clean, pipe-delimited (`|`) text files:

| File Name | Format Specification | Sample Entry |
| :--- | :--- | :--- |
| `books.txt` | `PHYSICAL\|Id\|Title\|Author\|IsAvailable\|Shelf\|ISBN`<br>`EBOOK\|Id\|Title\|Author\|IsAvailable\|FileSizeMB` | `PHYSICAL\|B001\|Clean Code: Agile Software\|Robert C. Martin\|True\|Shelf A1\|978-0132350884`<br>`EBOOK\|B003\|Head First Design Patterns\|Eric Freeman\|True\|12.5` |
| `members.txt` | `MemberId\|FullName\|MaxBorrowLimit` | `M001\|John Doe\|3`<br>`M002\|Jane Smith\|3` |
| `loans.txt` | `LoanId\|MemberId\|ItemId\|BorrowDate\|DueDate\|ReturnDate\|Status\|FineAmount` | `L001\|M001\|B003\|2026-10-01\|2026-10-15\|2026-10-10\|Returned\|0` |

### Persistence Implementation Highlights:
1. **Comment Parsing:** Lines beginning with `#` are treated as schema headers/comments and skipped during parsing.
2. **Rehydration Constructor:** A specialized constructor `Loan(...)` accepts historical timestamps and fine amounts to rehydrate past records from `loans.txt`.
3. **Transactional Safety:** Data files are updated via `File.WriteAllLines` only after the business operations succeed in RAM, preventing corrupted writes.

---

## **IV. MINIMUM TEST CASES EXECUTION & EVIDENCE**

The system implements the 3 required minimum test cases under `LibraryService.RunAutomatedTests()` (Menu option `6`). All tests execute automatically and output verifiable evidence:

| Test Case | Scenario Description & Inputs | Expected Behavior | Actual Execution Result | Status |
| :--- | :--- | :--- | :--- | 
| **1. Happy Path** | Member `T_M01` borrows physical book `T_B01` for 14 days. | • Active loan created.<br>• Book `IsAvailable` transitions to `False`. | `[PASS] Loan L003 created. Book IsAvailable = False` | 
| **2. Validation & Error Rejection** | Member `T_M01` borrows 3 books (`T_B01` to `T_B03`), then attempts to borrow a 4th book (`T_B04`). | System blocks the 4th checkout and throws `InvalidOperationException`. | `[PASS - BLOCKED] Expected exception caught: "Member 'Test User' has reached the limit of 3 books!"` |
| **3. Polymorphism Verification** | Physical book `P1` and E-book `E1` are both evaluated at 5 days overdue via `LibraryItem` reference. | • `P1.CalculateOverdueFee(5)` = 25,000 VND<br>• `E1.CalculateOverdueFee(5)` = 0 VND | `[PhysicalBook] Fine for 5 days: 25,000 VND (Rate: 5,000 VND/day)`<br>`[EBook] Fine for 5 days: 0 VND (Rate: 0 VND - Waived)`<br>`-> [PASS] Polymorphism works correctly!` | 

### Execution Log Output (from Console):
```text
=================================================================
    RUNNING 3 MINIMUM TEST CASES (OOP VERIFICATION)
=================================================================

[TEST CASE 1] Happy Path: Borrow & State Update
-> [PASS] Loan L003 created. Book IsAvailable = False

[TEST CASE 2] Validation: Block Exceeding Max Limit (3 books)
-> [PASS - BLOCKED] Expected exception caught: "Member 'Test User' has reached the limit of 3 books!"

[TEST CASE 3] Polymorphism Verification: 5-Day Overdue Fine
-> [PhysicalBook] Fine for 5 days: 25,000 VND (Rate: 5,000 VND/day)
-> [EBook] Fine for 5 days: 0 VND (Rate: 0 VND - Waived)
-> [PASS] Polymorphism works correctly!

=================================================================
    ALL 3 MINIMUM TEST CASES PASSED SUCCESSFULLY!
=================================================================
```

---
  WEEK 3 PLAN
---
| Member | Assigned Tasks |
| :--- | :--- |
| **Phạm Nguyễn Thùy Trâm** | • **Search Refactoring & CLI Menu:** Refactor `SearchBooks()` using polymorphism (removing manual `is PhysicalBook` type-checking) and integrate the existing `Loan.DisplayInfo()` method into `Program.Main()` to view loan records.<br>• **Documentation & Class Diagram:** Update the class design diagram and author the code refactoring explanation for the Week 3 Report. |
| **Lê Thiện Luân** | • **Input Validation & Automated Testing:** Add input validation guards across `LibraryItem`, `Member`, and `Loan`, and expand `RunAutomatedTests()` with exception-handling test scenarios.<br>• **Documentation & Report Compilation:** Author the Exception Handling section and compile the final Week 3 Report. |
