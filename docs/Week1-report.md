Week 1


| Member | Assigned Tasks |
| :--- | :--- |
| **Phạm Nguyễn Thùy Trâm** | Research and do the Syllabus Mapping (align course learning outcomes with project deliverables).Design Draft UML Class Diagram (specify core classes, attributes, operations, and relationships).|
| **Lê Thiện Luân** | Research and construct Set of 5 CRC Cards (define responsibilities and collaborations for 5 key classes).Develop Minimum Test Plan (establish testing scope, core test cases, and validation criteria). |

## **I. SYLLABUS MAPPING**

| Core OOP Concept | Specific Class & Method Applied (To be detailed in Self-Explanation) | Design Purpose & Technical Solution |
| :---- | :---- | :---- |
| **Encapsulation** | - LibraryItem: MarkAsBorrowed(), MarkAsReturned()<br>- Member: MaxBorrowLimit (private set)<br>- Loan: CompleteReturn() | Protects internal state. Sets private/protected set for all sensitive properties. Forbids external code from directly overwriting fields. |
| **Inheritance** | - PhysicalBook : LibraryItem<br>- EBook : LibraryItem | Inherits and reuses common attributes (Id, Title, Author, IsAvailable). Extends domain-specific attributes (ShelfLocation, ISBN, FileSizeMB). |
| **Polymorphism** | - Base class: LibraryItem.CalculateOverdueFee(int days) (abstract)<br>- Subclasses: override CalculateOverdueFee(...) | Implements differentiated overdue fee calculation:<br>+ Physical books incur 5,000 VND/day<br>+ E-books never expire / incur 0 VND penalty |

## **II. Draft UML Class Diagram**

```text
                       +--------------------------------------------------------------+
                       |                  <<abstract>> LibraryItem                    |
                       +--------------------------------------------------------------+
                       | # Id : string                                                |
                       | # Title : string                                             |
                       | # Author : string                                            |
                       | # IsAvailable : bool                                         |
                       +--------------------------------------------------------------+
                       | + MarkAsBorrowed() : void                                    |
                       | + MarkAsReturned() : void                                    |
                       | + <<abstract>> CalculateOverdueFee(overdueDays: int): decimal|
                       | + <<abstract>> ToDataString() : string                       |
                       | + DisplayInfo() : void                                       |
                       +--------------------------------------------------------------+
                                      ^                                ^
                                      | (is-a:                         | (is-a:
            +-------------------------+  Inheritance)                  +-------------------------+
            |                                ^                                                   |
+--------------------------------------+     | (has-a: linked       +--------------------------------------+
|             PhysicalBook             |     |  by ItemId)          |                EBook                 |
+--------------------------------------+     |                      +--------------------------------------+
| - ShelfLocation : string             |     |                      | - FileSizeMB : double                |
| - ISBN : string                      |     |                      +--------------------------------------+
+--------------------------------------+     |                      | + CalculateOverdueFee(overdueDays)   |
| + CalculateOverdueFee(overdueDays)   |     |                      |   --> returns 0m                     |
|   --> returns days * 5000m           |     |                      | + ToDataString() : string            |
| + ToDataString() : string            |     |                      | + DisplayInfo() : void               |
| + DisplayInfo() : void               |     |                      +--------------------------------------+
+--------------------------------------+     |
                                             |
     +---------------------------------------+
     |
+----+----------------------------------+                   +--------------------------------------+
|                 Loan                  |                   |                Member                |
+---------------------------------------+                   +--------------------------------------+
| - LoanId : string                     |        MemberId   | - Id : string                        |
| - MemberId : string                   | * -------------- 1| - FullName : string                  |
| - ItemId : string                     |                   | - MaxBorrowLimit : int = 3           |
| - BorrowDate : DateTime               |                   +--------------------------------------+
| - DueDate : DateTime                  |                   | + ToDataString() : string            |
| - ReturnDate : DateTime?              |                   | + DisplayInfo(count: int) : void     |
| - Status : string ("Active"/"Returned")                   +--------------------------------------+
| - FineAmount : decimal                |
+---------------------------------------+
| + IsOverdue(atDate: DateTime) : bool  |
| + GetOverdueDays(atDate: DateTime):int|
| + CompleteReturn(date, fee) : void    |
| + ToDataString() : string             |
| + DisplayInfo() : void                |
+---------------------------------------+
```

## **III. Set of 5 CRC cards **

| Class | Responsibilities | Collaborations |
| :---- | :---- | :---- |
| **LibraryItem** | • Manage identity metadata (Id, Title, Author)<br>• Encapsulate availability changes via MarkAsBorrowed() and MarkAsReturned()<br>• Define the abstract template method for overdue fee calculation CalculateOverdueFee() | PhysicalBook, EBook, Loan |
| **PhysicalBook** | • Manage shelf location and ISBN code<br>• Implement overdue fee formula: days * 5,000 VND | LibraryItem (inheritance) |
| **EBook** | • Manage file size<br>• Implement overdue fee exemption policy: Always return 0 VND | LibraryItem (inheritance) |
| **Member** | • Manage patron information (Id, FullName)<br>• Set and enforce maximum borrow limit (MaxBorrowLimit = 3) | Loan |
| **Loan** | • Link patron and item via MemberId and ItemId<br>• Define loan duration (14 days) and detect overdue status (IsOverdue)<br>• Encapsulate return finalization (actual return date and fine) via CompleteReturn() | Member, LibraryItem |

---

## **IV. MINIMUM TEST PLAN**

In accordance with the **Minimum Test Cases** requirements:

| Test | Test Inputs | Expected Assertions |
| :---- | :---- | :---- |
| **1. Happy Path Scenario** | - Member M001 (borrow count = 0)<br>- Physical book B001 (IsAvailable = true)<br>- Borrow period: 14 days, returned after 7 days. | - Newly created Loan has status Active<br>- B001.IsAvailable transitions to false<br>- M001.CurrentlyBorrowedCount increments from 0 to 1<br>- Upon return: Loan status becomes returned, penalty = 0 VND, book unlocked (IsAvailable = true), M001 borrow count decrements to 0. |
| **2. Validation / Error Rejection** | **Scenario A:** Member M001 attempts to borrow B001 while IsAvailable == false<br>**Scenario B:** Member M002 currently holding 3 items attempts to borrow a 4th | - **Scenario A:** Throws InvalidOperationException: Item is already borrowed; no new Loan created<br>- **Scenario B:** Throws InvalidOperationException: Member limit exceeded (Max 3); item state remains unchanged |
| **3. Polymorphism / Inheritance Verification** | - Both overdue by 5 days<br>- Loan 1: LibraryItem is a PhysicalBook<br>- Loan 2: LibraryItem is an EBook | - Loan 1: item.CalculateOverdueFee(5) returns 25,000 VND<br>- Loan 2: item.CalculateOverdueFee(5) returns 0 VND |
