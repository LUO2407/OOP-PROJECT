# WEEK 4 REPORT

| Member | Assigned Tasks | 
| :--- | :--- |
| **Phạm Nguyễn Thùy Trâm** | • Implement the `Audiobook` class inheriting from `LibraryItem` (3,000 VND/day overdue fee) *without modifying any legacy class code*.<br>• Draw the final UML Class Diagram matching 100% of the actual C# codebase.<br>• Draft 5 Q&A pairs (focused on OCP, Polymorphism, and UML) and complete the Inheritance & Polymorphism mapping table. | 
| **Lê Thiện Luân** | • Integrate unit tests for `Audiobook`.<br>• Write the self-reflection, and finalize the Week 4 Report.<br>• Draft 5 Q&A pairs (focused on encapsulation, data invariants, and file persistence) and complete the encapsulation and abstraction mapping table. | 
---
## PHAM NGUYEN THUY TRAM REPORT 
## 1. Task

- Add an `Audiobook` class inheriting from `LibraryItem` (overdue fee 3,000 VND/day) without modifying legacy class code.
- Provide a UML Class Diagram matching the C# code (separate file).
- Write 5 Q&A pairs (OCP, Polymorphism, UML) and complete the Inheritance & Polymorphism mapping table.

## 2. Implementation

`Audiobook` is a new class in `Program.cs`, following the same structure as `PhysicalBook` and `EBook`.

- **Members:** `const decimal DailyFine = 3000m`, `Narrator` (string), `DurationMinutes` (int). The constructor validates the narrator (not empty) and duration (> 0), throwing `ArgumentException` otherwise.
- **Overrides:**
  - `MatchesKeyword`: base match (Title/Author) or Narrator contains the keyword.
  - `CalculateOverdueFee`: `overdueDays × 3000m`.
  - `GetItemType`: `"Audiobook"`.
  - `ToDataString`: `AUDIOBOOK|Id|Title|Author|IsAvailable|Narrator|DurationMinutes`.
  - `DisplayInfo`: prints one line with the item details.
- **Integration:** `SearchBooks`, `ReturnBook`, `SaveBooks` and the menu already call these methods through a `LibraryItem` reference, so they reach `Audiobook` unchanged.

## 3. Changes Compared with the Original File

95 lines were added; no existing line was edited or removed.

| Where | Change | Lines |
|---|---|---|
| New class `Audiobook` | Added between `EBook` and `Member` | 83 |
| `FileService.LoadBooks` | New `else if (type == "AUDIOBOOK")` branch | 10 |
| `LibraryService.SeedInitialData` | Two sample audiobooks (A001, A002) | 2 |

The last two go slightly beyond a strict reading of "no legacy changes"; see section 6.

## 4. Inheritance & Polymorphism Mapping Table

| Member | `LibraryItem` | `PhysicalBook` | `EBook` | `Audiobook` (new) |
|---|---|---|---|---|
| `Id`, `Title`, `Author`, `IsAvailable` | Properties | Inherited | Inherited | Inherited |
| `MarkAsBorrowed()`, `MarkAsReturned()` | Concrete | Inherited | Inherited | Inherited |
| `MatchesKeyword` | `virtual` | override: + ISBN | Uses base | override: + Narrator |
| `CalculateOverdueFee` | `abstract` | days × 5,000 | always 0 | days × 3,000 |
| `GetItemType` | `abstract` | "PhysicalBook" | "EBook" | "Audiobook" |
| `ToDataString` | `abstract` | PHYSICAL\|... | EBOOK\|... | AUDIOBOOK\|... |
| `DisplayInfo` | `abstract` | Shelf, ISBN | Size (MB) | Narrator, Duration |
| Own members | | `DailyFine`, `ShelfLocation`, `ISBN` | `FileSizeMB` | `DailyFine`, `Narrator`, `DurationMinutes` |

## 5. Five Q&A Pairs  

**Q1 (OCP). How does adding `Audiobook` show the Open/Closed Principle?**
It was added as a new subclass; no line in `LibraryItem`, `PhysicalBook`, `EBook`, `Member`, `Loan` or `Program` changed. Search, fees, saving and display already work through `LibraryItem`, so the system is open for extension and closed for modification.

**Q2 (Polymorphism). How does `ReturnBook` charge 3,000 VND for an `Audiobook` but 5,000 VND for a `PhysicalBook`?**
It calls `item.CalculateOverdueFee(days)` on a `LibraryItem` reference. At run time the CLR dispatches to the override of the object's actual type, so no `if (item is ...)` check is needed.

**Q3 (Polymorphism). Why is `MatchesKeyword` `virtual` but `CalculateOverdueFee` `abstract`?**
An `abstract` member has no body and must be overridden; a `virtual` member has a default that may be overridden. Fee rules differ per type with no sensible default, while Title/Author search works for all items, so subclasses only extend it via `base.MatchesKeyword(...)`.

**Q4 (UML). Which UML relationships describe this design?**
Generalization (hollow triangle) from `PhysicalBook`, `EBook` and `Audiobook` to `LibraryItem`; composition from `LibraryService` to its `Items`, `Members`, `Loans` lists and `FileService`; dependency (dashed arrow) where one class only creates or uses another. `Loan` has no arrow to `Member` or `LibraryItem` because it stores only their string IDs.

**Q5 (OCP/UML). Why did `LoadBooks` need a new branch, and what does it show?**
It only recognised `PHYSICAL` and `EBOOK` and skipped other types, so saved audiobooks were lost on restart. The `if/else if` chain on a type string is a hidden OCP violation: every new type forces an edit. A factory or a registry of parsers (e.g. `Dictionary<string, Func<string[], LibraryItem>>`) would avoid this.

## 6. Difficulties

- **Audiobooks lost after restart:** `LoadBooks` skipped `AUDIOBOOK` lines. Fixed by adding the branch.
- **Sample audiobooks missing:** `SeedInitialData` only runs on an empty list, so an old `Data/books.txt` hides them. Delete the `Data` folder and rerun.

## 7. How to Verify

1. Delete the `Data` folder and run; menu 1 lists A001 and A002 as "[Audiobook]".
2. Search "Michael" or "Emma" (narrator); the audiobook is found.
3. Borrow A001 and return it late; fine = overdue days × 3,000 VND.
4. Restart; A001 and A002 are still listed (confirms the `LoadBooks` branch).

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
