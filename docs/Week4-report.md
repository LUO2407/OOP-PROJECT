# WEEK 4 REPORT

| Member | Assigned Tasks | 
| :--- | :--- |
| **Phạm Nguyễn Thùy Trâm** | • Implement the `Audiobook` class inheriting from `LibraryItem` (3,000 VND/day overdue fee) *without modifying any legacy class code*.<br>• Draw the final UML Class Diagram matching 100% of the actual C# codebase.<br>• Draft 5 Q&A pairs (focused on OCP, Polymorphism, and UML) and complete the Inheritance & Polymorphism mapping table. | 
| **Lê Thiện Luân** | • Integrate unit tests for `Audiobook`.<br>• Write the self-reflection, and finalize the Week 4 Report.<br>• Draft 5 Q&A pairs (focused on encapsulation, data invariants, and file persistence) and complete the encapsulation and abstraction mapping table. | 
---
## PHAM NGUYEN THUY TRAM REPORT 
## 1. Task

- Add `Audiobook` (extends `LibraryItem`, overdue fee 3,000 VND/day) without changing legacy classes.
- Provide a UML Class Diagram (separate file).
- Write 5 Q&A pairs and the Inheritance & Polymorphism mapping table.

## 2. Implementation

`Audiobook` is a new class in `Program.cs`, built like `PhysicalBook` and `EBook`.

- **Members:** `DailyFine = 3000m`, `Narrator`, `DurationMinutes` (validated in the constructor).
- **Overrides:** `MatchesKeyword` (also matches Narrator), `CalculateOverdueFee` (days × 3000m), `GetItemType`, `ToDataString`, `DisplayInfo`.
- **Integration:** existing code calls these through `LibraryItem`, so it works unchanged.

## 3. Changes to the Original File

95 lines added, none edited or removed.

| Where | Change | Lines |
|---|---|---|
| `Audiobook` class | New | 83 |
| `FileService.LoadBooks` | New `AUDIOBOOK` branch | 10 |
| `SeedInitialData` | Two sample audiobooks | 2 |

## 4. Inheritance & Polymorphism Mapping

| Member | `LibraryItem` | `PhysicalBook` | `EBook` | `Audiobook` |
|---|---|---|---|---|
| `Id`, `Title`, `Author`, `IsAvailable`, `MarkAs...()` | Concrete | Inherited | Inherited | Inherited |
| `MatchesKeyword` | `virtual` | + ISBN | Uses base | + Narrator |
| `CalculateOverdueFee` | `abstract` | days × 5,000 | 0 | days × 3,000 |
| `GetItemType`, `ToDataString`, `DisplayInfo` | `abstract` | override | override | override |

## 5. Q&A

**Q1 (OCP). How does `Audiobook` show OCP?**
It was added as a new subclass without editing existing item classes; the system is open for extension, closed for modification.

**Q2 (Polymorphism). How does `ReturnBook` charge different fees per type?**
It calls `item.CalculateOverdueFee(days)` on a `LibraryItem`; the CLR runs the override of the actual object type.

**Q3 (Polymorphism). Why `virtual` for `MatchesKeyword` but `abstract` for `CalculateOverdueFee`?**
Searching by Title/Author has a sensible default; fee rules differ per type and have none.

**Q4 (UML). Which relationships are used?**
Generalization (subclasses to `LibraryItem`), composition (`LibraryService` to its lists and `FileService`), dependency (class that only uses another). `Loan` stores only string IDs, so it has no arrow to `Member` or `LibraryItem`.

**Q5 (OCP/UML). Why did `LoadBooks` need a new branch?**
It only recognised `PHYSICAL` and `EBOOK`, so saved audiobooks were lost on restart. The `if/else if` on a type string is a hidden OCP violation; a factory or parser registry would avoid it.

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
