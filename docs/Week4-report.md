# WEEK 4 REPORT

| Member | Assigned Tasks | 
| :--- | :--- |
| **Phạm Nguyễn Thùy Trâm** | • Implement the `Audiobook` class inheriting from `LibraryItem` (3,000 VND/day overdue fee) *without modifying any legacy class code*.<br>• Draw the final UML Class Diagram matching 100% of the actual C# codebase.<br>• Draft 5 Q&A pairs (focused on OCP, Polymorphism, and UML) and complete the Inheritance & Polymorphism mapping table. | 
| **Lê Thiện Luân** | • Integrate unit tests for `Audiobook`.<br>• Write the self-reflection, and finalize the Week 4 Report.<br>• Draft 5 Q&A pairs (focused on encapsulation, data invariants, and file persistence) and complete the encapsulation and abstraction mapping table. | 
---
## PHAM NGUYEN THUY TRAM REPORT 
## I. Self-reflection and verification

### 1. Summary of completed work

- **Audiobook class:** Added `Audiobook : LibraryItem` with `DailyFine = 3000m`, `Narrator` and `DurationMinutes`. It overrides `MatchesKeyword` (also matches Narrator), `CalculateOverdueFee` (overdue days × 3,000 VND, or 0 when not overdue), `GetItemType`, `ToDataString` and `DisplayInfo`.
- **No change to legacy item classes:** `LibraryItem`, `PhysicalBook` and `EBook` are untouched. `SearchBooks`, `ReturnBook`, `SaveBooks` and the menu already work through a `LibraryItem` reference, so they handle `Audiobook` automatically.
- **Data invariants:** the constructor rejects an empty narrator and a duration ≤ 0 with `ArgumentException`, on top of the checks inherited from `LibraryItem` (ID, title, author).
- **Sample data:** added two sample audiobooks (A001, A002) to `SeedInitialData` for demonstration.
- **UML Class Diagram:** drawn from the C# code and submitted as a separate file.

### 2. Before / after comparison

| Comparison | Before | After |
|---|---|---|
| Item types | 2: `PhysicalBook`, `EBook` | 3: added `Audiobook` |
| Overdue fee per day | 5,000 VND (physical), 0 VND (e-book) | Added 3,000 VND (audiobook) |
| Keyword search | Title and Author, plus ISBN for physical books | Added Narrator for audiobooks, through `MatchesKeyword` |
| Seed data | 3 physical books, 2 e-books | Added 2 audiobooks (A001, A002) |
| Legacy item classes changed |  | None (`LibraryItem`, `PhysicalBook`, `EBook` untouched) |

## II. Five Q&A pairs

**Q1 (OCP). How does adding `Audiobook` follow the Open/Closed Principle?**
* **Answer:** `Audiobook` is a new subclass of `LibraryItem`; no line in `LibraryItem`, `PhysicalBook`, `EBook`, `Member`, `Loan` or `Program` was edited. Searching, fee calculation, saving and display already depend on the abstraction `LibraryItem`, so the system is open for extension and closed for modification.

**Q2 (Polymorphism). How does `ReturnBook` charge 3,000 VND for an `Audiobook` but 5,000 VND for a `PhysicalBook`?**
* **Answer:** `ReturnBook` calls `item.CalculateOverdueFee(overdueDays)` on a `LibraryItem` reference. At run time the CLR dispatches to the override of the object's actual type, so no `if (item is PhysicalBook)` check is needed.

**Q3 (Polymorphism). Why is `MatchesKeyword` `virtual` but `CalculateOverdueFee` `abstract`?**
* **Answer:** Searching by Title and Author makes sense for every item, so `MatchesKeyword` has a default body that subclasses may extend: `PhysicalBook` and `Audiobook` call `base.MatchesKeyword(keyword)` and then check ISBN or Narrator. Fee rules differ per type and have no sensible default, so `CalculateOverdueFee` is `abstract` and every concrete subclass must override it.

**Q4 (UML). Which UML relationships describe this design?**
* **Answer:** Generalization (solid line, hollow triangle) from `PhysicalBook`, `EBook` and `Audiobook` to the abstract class `LibraryItem`, which is shown with `<<abstract>>` and italic name. Composition from `LibraryService` to `FileService` and its `Items`, `Members` and `Loans` lists, because it creates and owns them. Association from `Program` to `LibraryService`. `Loan` keeps only the string IDs `MemberId` and `ItemId`, so it links to `Member` and `LibraryItem` by ID rather than by object reference.

**Q5 (OCP/UML). What changes in the UML diagram when `Audiobook` is added?**
* **Answer:** Only one new class box (`Audiobook`) and one new generalization arrow to `LibraryItem` appear. No existing class or relationship has to change, which shows visually that the design is closed for modification and open for extension.

## III. Inheritance & Polymorphism mapping table

| Member | `LibraryItem` | `PhysicalBook` | `EBook` | `Audiobook` (new) |
|---|---|---|---|---|
| `Id`, `Title`, `Author`, `IsAvailable` | Properties (validated in constructor) | Inherited | Inherited | Inherited |
| `MarkAsBorrowed()`, `MarkAsReturned()` | Concrete methods | Inherited | Inherited | Inherited |
| `MatchesKeyword(keyword)` | `virtual`: Title or Author | `override`: base or ISBN | Not overridden (uses base) | `override`: base or Narrator |
| `CalculateOverdueFee(days)` | `abstract` | `override`: days × 5,000 | `override`: always 0 | `override`: days × 3,000 |
| `GetItemType()` | `abstract` | `override`: "PhysicalBook" | `override`: "EBook" | `override`: "Audiobook" |
| `ToDataString()` | `abstract` | `override`: PHYSICAL\|... | `override`: EBOOK\|... | `override`: AUDIOBOOK\|... |
| `DisplayInfo()` | `abstract` | `override`: Shelf, ISBN | `override`: Size (MB) | `override`: Narrator, Duration |
| Own members |  | `DailyFine`, `ShelfLocation`, `ISBN` | `FileSizeMB` | `DailyFine`, `Narrator`, `DurationMinutes` |

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
