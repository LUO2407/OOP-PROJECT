Week 1


| Member | Assigned Tasks |
| :--- | :--- |
| **Thùy trâm** | Research and do the Syllabus Mapping (align course learning outcomes with project deliverables).Design Draft UML Class Diagram (specify core classes, attributes, operations, and relationships).|
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
