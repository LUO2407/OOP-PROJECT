# WEEK 4 REPORT

| Member | Assigned Tasks | 
| :--- | :--- |
| **Phạm Nguyễn Thùy Trâm** | • Implement the `Audiobook` class inheriting from `LibraryItem` (3,000 VND/day overdue fee) *without modifying any legacy class code*.<br>• Draw the final UML Class Diagram matching 100% of the actual C# codebase.<br>• Draft 5 Q&A pairs (focused on OCP, Polymorphism, and UML) and complete the Inheritance & Polymorphism mapping table. | 
| **Lê Thiện Luân** | • Integrate unit tests for `Audiobook`.<br>• Write the self-reflection, and finalize the Week 4 Report.<br>• Draft 5 Q&A pairs (focused on encapsulation, data invariants, and file persistence) and complete the encapsulation and abstraction mapping table. | 
---
## LE THIEN LUAN REPORT
### **I. Self-reflection and verification**

#### **1. Summary of completed work**
* **Audiobook unit tests integration:** Expanded `RunAutomatedTests()` with 4 dedicated test cases for `Audiobook` to verify the 3,000 VND/day overdue fee calculation via `LibraryItem`, keyword search by `Narrator`, input validation guards (`ArgumentException`), and end-to-end borrow/return flow with a 4-day overdue fine .
* **Data invariants protection:** Ensured all domain entities (`LibraryItem`, `Member`, `Loan`, `Audiobook`) strictly validate parameters directly within their constructors.
* **File persistence layer & cleanup:** `FileService` accurately reads and writes `AUDIOBOOK` records. The automated test suite cleans up generated test data upon completion to keep persistent storage files clean.

#### **2. Before / after comparison table**

| Comparison | Before (Week 3) | After (Week 4) |
| :--- | :--- | :--- |
| **Automated test cases** | 6 test cases (happy path, limit check, fee polymorphism, input guards, state errors). | **10 test cases** (added 4 test cases for `Audiobook`: 3k/day fine, narrator search, input guards, borrow/return flow). |
| **File persistence** | Supported reading/writing 2 record types: `PHYSICAL` and `EBOOK`. | Added support for reading/writing `AUDIOBOOK` records with 7 standardized fields. |
| **Data invariants** | Validation guards applied to `PhysicalBook` and `EBook`. | Extended validation guards to `Audiobook` (`narrator` non-empty, `durationMinutes > 0`). |
| **Test data cleanup** | Removed test objects prefixed with `T_`, `P1`, `E1`. | Automated cleanup algorithm removes all generated items, members, and loans across all 10 test scenarios. |

---

### **II. 5 Question and answer pairs**

**Q1: Why does `IsAvailable` have a `protected set`, and how is its state transition encapsulated?**
* **Answer:** `IsAvailable` uses `protected set` to prevent external code from directly modifying an item's availability status. State transitions are strictly encapsulated within `MarkAsBorrowed()` and `MarkAsReturned()`, which enforce domain guards that throw an `InvalidOperationException` if an item is already borrowed or returned.

**Q2: How does the `Loan` class enforce data invariants during new loan creation and file rehydration?**
* **Answer:** For new checkouts, the constructor validates non-empty IDs, ensures `durationDays > 0`, and automatically sets borrow and due dates. For file rehydration (`loans.txt`), the constructor validates historical integrity by enforcing `due >= borrow`, throwing an `ArgumentException` if corrupted data is loaded.

**Q3: How does `CompleteReturn` encapsulate book returns to prevent invalid state changes?**
* **Answer:** `CompleteReturn` throws an `InvalidOperationException` if the loan is already settled (`Status == "Returned"`), throws an `ArgumentException` if `returnDate < BorrowDate`, and defaults `FineAmount` to `0m` if a negative fee is passed.

**Q4: How does `FileService` demonstrate abstraction in the persistence layer?**
* **Answer:** `FileService` hides all underlying file I/O operations (directory creation, pipe delimiter parsing, `#` comment skipping, type parsing). `LibraryService` operates entirely on high-level domain collections (`List<T>`) via abstract methods (`LoadBooks()`, `SaveBooks()`, `LoadLoans()`, `SaveLoans()`).

**Q5: How does `FileService` combine encapsulation and polymorphism when adding `Audiobook`?**
* **Answer:** When loading data, `FileService` parses `AUDIOBOOK` lines and delegates input validation directly to the `Audiobook` constructor. When saving data, `FileService` calls `item.ToDataString()`, relying on polymorphic method overriding so `Audiobook` formats its own persistent data string.
