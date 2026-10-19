| Member | Assigned Tasks |
| --- | --- |
| **Phạm Nguyễn Thùy Trâm** | Implement the item hierarchy: abstract class `LibraryItem` and two derived classes `PhysicalBook` and `EBook`, applying Polymorphism to `CalculateOverdueFee`.Implement the search feature to query items by Title, Author, or ISBN using appropriate data structures.Design and build a command-line interface (CLI) to demonstrate the borrow/return lifecycle end-to-end.Author the Self-Explanation section for Inheritance and Polymorphism, detailing the exact classes and methods applied in the codebase.|
| **Lê Thiện Luân** | Implement the domain entities `Member` and `Loan`, applying Encapsulation to manage internal state and borrowing rules (maximum 3 items per member, 14-day loan duration).Build the file persistence layer `FileService`.Code and execute the Minimum Test Cases (Happy Path, validation/error rejection, and Polymorphism verification), exporting test execution logs as evidence.Author the Self-Explanation section for Encapsulation and compile the final Week 2 Report.|
---
Phạm Nguyễn Thùy Trâm report
---
## **I. SYLLABUS MAPPING**

| Core OOP Concept | Specific Class & Method Applied | Design Purpose & Technical Solution | Primary Contributor |
| :--- | :--- | :--- | :--- |
| **Inheritance** | • Base class: `public abstract class LibraryItem`<br>• Derived classes: `PhysicalBook : LibraryItem`, `EBook : LibraryItem`<br>• Base constructor invocation: `: base(id, title, author, isAvailable)` | Eliminates code duplication by centralizing common cataloging properties (`Id`, `Title`, `Author`, `IsAvailable`) in the abstract base class. Extends specialized attributes: `ShelfLocation`, `ISBN` for physical books; `FileSizeMB` for e-books. | **Phạm Nguyễn Thùy Trâm** |
| **Polymorphism** | • Abstract method: `public abstract decimal CalculateOverdueFee(int overdueDays);` in `LibraryItem`<br>• Overridden in: `PhysicalBook.CalculateOverdueFee(...)`<br>• Overridden in: `EBook.CalculateOverdueFee(...)`<br>• Dispatch site: `item.CalculateOverdueFee(...)` | Implements runtime polymorphism to differentiate overdue penalty rules without using procedural `switch` or type-checking statements (`is`). Physical books incur 5,000 VND/day, while digital e-books incur 0 VND penalty. | **Phạm Nguyễn Thùy Trâm** |
| **Collections & Search** | • Collection: `List<LibraryItem>`<br>• Method: `LibraryService.SearchBooks(keyword)` | Stores library items in an in-memory generic list. Traverses elements to match keywords against Title, Author, or ISBN (case-insensitive) without mutating the catalog. | **Phạm Nguyễn Thùy Trâm** |
| **CLI & User Interaction** | • `Program.Main()` interactive menu loop<br>• Formatted tabular display: `DisplayInfo()` in `PhysicalBook` and `EBook` | Provides a clean, numbered console interface allowing users to browse, search, borrow, return items, and view formatted tables. | **Phạm Nguyễn Thùy Trâm** |

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
