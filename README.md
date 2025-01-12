# ORM-Library

## Spis Treści

| Sekcja                                                                                       |
| -------------------------------------------------------------------------------------------- |
| [1. Opis projektu](#opis-projektu)                                                           |
| [2. Jak korzystać z Biblioteki?](#jak-korzystać-z-biblioteki)                                |
| &nbsp;&nbsp;&nbsp;&nbsp;[2.1. Tworzenie klas do mapowania](#tworzenie-klas-do-mapowania)    |
| &nbsp;&nbsp;&nbsp;&nbsp;[2.2. Tworzenie klasy dziedziczącej po `DatabaseContext`](#tworzenie-klasy-dziedziczącej-po-databasecontext) |
| &nbsp;&nbsp;&nbsp;&nbsp;[2.3. Tworzenie obiektu reprezentującego fabrykę](#tworzenie-obiektu-reprezentującego-fabrykę) |
| &nbsp;&nbsp;&nbsp;&nbsp;[2.4. Tworzenie obiektu reprezentującego bazę](#tworzenie-obiektu-reprezentującego-bazę) |
| &nbsp;&nbsp;&nbsp;&nbsp;[2.5. Operacje na tabelach](#operacje-na-tabelach) |
| [3. Podsumowanie](#podsumowanie)   

## Opis projektu

Celem projektu jest stworzenie pakietu klas implementujących **Object-Relational Mapping (ORM)** dla dowolnego modelu domenowego, w oparciu o technologię **C#**. Projekt bazuje na podejściu "by exception", w którym konfiguracja modelu encji wymaga jedynie definiowania wyjątków od domyślnych ustawień.

### Główne funkcje biblioteki:

- **Obsługa popularnych baz danych:** Biblioteka wspiera dwa bardzo popularne systemy zarządzania relacyjnymi bazami danych: **PostgreSQL** oraz **MySQL**.
- **Mapowanie obiektów na tabele:** Automatyczne mapowanie klas na tabele w bazach danych.
- **Operacje CRUD:** Umożliwia wykonywanie operacji CRUD (tworzenie, odczyt, aktualizacja, usuwanie) na bazach danych.


## Jak korzystać z Biblioteki?
### Tworzenie klas do mapowania

Trzeba stworzyć klasy, które chce się zmapować według podanego wzoru:

```csharp
[Table("customers")]                            // "customers" będzie nazwą tabeli, która utworzy się w bazie
public class Customer
{
    [Key]                                       // Klucz główny
    public int Customer_id { get; set; }          

    [ForeignKey("orders")]                      // Klucz obcy, "orders" to nazwa tabeli, do której odnosi się klucz obcy
    public int Cusomer_order_id { get; set; }

    [MaxLength(64)]                             // Maksymalna długość VARCHAR
    public string Email { get; set; }  

    [MaxLength(64)]
    public string Name { get; set; }

    public string Adress { get; set; }
}
```

### Tworzenie klasy dziedziczącej po `DatabaseContext`

Następnie nalezy utworzyć klase, według wzoru podanego poniżej, która bedzie dziedziczyła po `DatabaseContext`. Następnie trzeba do pól tej klasy dodać tabele które chcemy dodać. 
Należy dodać konstruktor który do utorzonych pól bedzie wpisywał kolekcje z typem tabeli. 

```csharp
public class CompaniesContext : DatabaseContext
    {
        public Table<Company> companies {get; set;}
        public Table<Worker> workers {get; set;}
        public Table<Product> products {get; set;}
        public Table<Orders> orders {get; set;}
        public Table<Customer> customers {get; set;}

        public CompaniesContext(string connectionString, IDbConnectionFactory dbConnectionFactory) : base(connectionString, dbConnectionFactory)
        {
            companies = Set<Company>();
            workers = Set<Worker>();
            products = Set<Product>();
            orders = Set<Orders>();
            customers = Set<Customer>();
        }

        
    }
```

### Tworzenie obiektu reprezentującego fabrykę
Teraz w głównym pliku programu (`Program.cs`) należy utworzyć obiekt reprezentujący fabrykę tworzącą połączenie (PostgreSqlConnectionFactory lub MySqlConnectionFactory) zależnie od tego której bazy danych chcemy użyć. 
Następnie trzeba utorzyć connectionString który jest stringiem przechowującym informacje potrzebne do nawiązania połączenia z bazą danych.

```csharp
var postgreSqlConnectionFactory = new PostgreSqlConnectionFactory();
string connectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=TwojeHasło;";

var MySqlConnectionFactory = new MySqlConnectionFactory();
string connectionString_mySql = "Host=localhost;Port=3306;Database=ORMTest;Username=root;Password=root";
                
```
### Tworzenie obiektu reprezentującego bazę 
W kolejnym kroku tworzymy tworzymy obiekt reprezentujący naszą bazę, w tym przypadku CompaniesContext i do jego konstruktora wrzucamy connectionString i fabryke tworzącą połączenie. 


```csharp
var companiesContext = new CompaniesContext(connectionString_mySql, MySqlConnectionFactory);
       

```
                
### Operacje na tabelach
W tym momencie mamy juz utworzoną bazę. Teraz możemy dzialać na konkretnych tabelach używając obieku Table<T> gdzie T jest Klasą którą mapowaliśmy a argumentem jest połączenie z bazą danych.
```csharp
var companiesTable = new Table<Company>(companiesContext.GetDatabaseConnection());    
```

Na tym obiekcie możemy wykonać operacje takie jak
Add(T object) który dodaje do tabeli nowy rekord z danymi które ma object
Remove(T object) który usuwa object z bazy 
ToList() zwracający listę obiektów z danymi z bazy
First() który zwraca pierwszy element z bazy
Include(Func<T, bool> predicate) który wybiera dane z bazy według podanego predykatu. Przykład użycia:

```csharp
var includeExample = companiesTable.Include(r => r.Name == "Test3").Include(r => r.Company_id == 3);
 ```

### Podsumowanie

Aby skorzystać z tej biblioteki ORM, wykonaj następujące kroki:

1. **Twórz klasy mapujące tabele:** Każda klasa powinna reprezentować jedną tabelę w bazie danych. Używaj atrybutów takich jak `[Table]`, `[Key]`, `[ForeignKey]`, `[MaxLength]` do określenia struktury tabeli i relacji między encjami.

2. **Stwórz klasę dziedziczącą po `DatabaseContext`:** W tej klasie zadeklaruj właściwości dla każdej tabeli, która będzie mapowana, jako obiekt typu `Table<T>`, a w konstruktorze przypisz odpowiednią kolekcję danych.

3. **Skonfiguruj połączenie z bazą danych:** Utwórz fabrykę połączenia (np. `PostgreSqlConnectionFactory` lub `MySqlConnectionFactory`), a następnie użyj jej wraz z odpowiednim `connectionString` do skonfigurowania połączenia z bazą.

4. **Używaj obiektów `Table<T>`:** Z poziomu obiektu `Table<T>` możesz wykonać operacje CRUD, takie jak dodawanie, usuwanie, modyfikowanie oraz pobieranie danych z bazy (np. `Add()`, `Remove()`, `ToList()`, `First()`, `Include()`).

5. **Operacje na tabelach:** Używając obiektu `Table<T>`, możesz wykonywać bardziej zaawansowane operacje na tabelach, takie jak filtrowanie danych przy użyciu predykatów (`Include()`), dodawanie nowych rekordów (`Add()`), usuwanie rekordów (`Remove()`), pobieranie pojedynczych rekordów (`First()`) lub list (`ToList()`). Możesz także łatwo przeprowadzać operacje na danych, takich jak iterowanie po wynikach (np. `ToList()`, `First()`).


wo tworzyć aplikacje, które wykorzystują bazę danych w oparciu o mapowanie obiektowo-relacyjne (ORM).