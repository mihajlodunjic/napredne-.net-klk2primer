# Uputstvo za izradu .NET konzolne aplikacije sa Entity Framework Core

Ovaj dokument opisuje osnovni tok rada za izradu konzolne .NET aplikacije koja koristi Entity Framework Core za rad sa relacionom bazom podataka.

Uputstvo je napisano kao opšti šablon i može se koristiti za različite ispitne zadatke u kojima se traži rad sa EF Core modelima, `DbContext` klasom, migracijama i bazom podataka.

---

## 1. Kreiranje projekta

U Visual Studio okruženju potrebno je napraviti novi projekat.

Koraci:

1. Otvoriti Visual Studio.
2. Kliknuti na `Create a new project`.
3. Izabrati tip projekta `Console App`.
4. Uneti naziv projekta.
5. Izabrati odgovarajuću verziju .NET framework-a.
6. Kliknuti na `Create`.

Nakon kreiranja projekta, osnovna struktura najčešće sadrži:

* `Program.cs`
* `.csproj` fajl projekta

---

## 2. Instaliranje Entity Framework Core paketa

Da bi aplikacija mogla da koristi Entity Framework Core, potrebno je instalirati odgovarajuće NuGet pakete.

U Visual Studio-u se paketi mogu instalirati na dva načina:

### Opcija 1: Kroz Manage NuGet Packages

1. Desni klik na projekat.
2. Izabrati `Manage NuGet Packages`.
3. Otvoriti tab `Browse`.
4. Pretražiti i instalirati sledeće pakete:

*  Microsoft.EntityFrameworkCore  
*  Microsoft.EntityFrameworkCore.SqlServer
*  Microsoft.EntityFrameworkCore.Tools  
*  Microsoft.EntityFrameworkCore.Design

### Opcija 2: Kroz Package Manager Console

Package Manager Console se otvara kroz meni:

`Tools` → `NuGet Package Manager` → `Package Manager Console`

Zatim se pokreću komande:

Install-Package Microsoft.EntityFrameworkCore

Install-Package Microsoft.EntityFrameworkCore.SqlServer

Install-Package Microsoft.EntityFrameworkCore.Tools

 Install-Package Microsoft.EntityFrameworkCore.Design  

Namena paketa:

* `Microsoft.EntityFrameworkCore` predstavlja osnovni EF Core paket.
* `Microsoft.EntityFrameworkCore.SqlServer` omogućava povezivanje sa SQL Server bazom.
* `Microsoft.EntityFrameworkCore.Tools` omogućava rad sa migracijama kroz Package Manager Console.
*  `Microsoft.EntityFrameworkCore.Design` — design-time podrška koju EF alati koriste za migracije, scaffolding i kreiranje/učitavanje `DbContext`-a u toku razvoja. Microsoft opisuje design-time alate kao deo EF-a koji pokreće operacije kao što su scaffolding modela i upravljanje migracijama.  

---

## 3. Predložena struktura foldera

U projektu je korisno napraviti sledeće foldere:

* `Models`
* `Data`

Folder `Models` služi za klase koje predstavljaju entitete, odnosno tabele u bazi.

Folder `Data` služi za klasu koja nasleđuje `DbContext` i preko koje aplikacija komunicira sa bazom.

Primer strukture:

ProjectName
├── Data
├── Models
├── Program.cs
└── ProjectName.csproj

---

## 4. Kreiranje model klasa

U folderu `Models` prave se klase koje predstavljaju podatke koji se čuvaju u bazi.

Svaka model klasa najčešće ima:

* primarni ključ, najčešće `Id`
* osobine koje odgovaraju kolonama u tabeli
* navigaciona svojstva za relacije sa drugim entitetima

Primer opšte strukture model klase:

public class EntityName
{
public int Id { get; set; }

```
// Ostala svojstva entiteta
```

}

Ako postoji relacija između entiteta, dodaju se strani ključevi i navigaciona svojstva.

Primer veze jedan-prema-više:

public class ChildEntity
{
public int Id { get; set; }

```
public int ParentEntityId { get; set; }

public ParentEntity ParentEntity { get; set; } = null!;
```

}

public class ParentEntity
{
public int Id { get; set; }

```
public List<ChildEntity> ChildEntities { get; set; } = new();
```

}

---

## 5. Kreiranje DbContext klase

U folderu `Data` pravi se klasa koja nasleđuje `DbContext`.

Ta klasa predstavlja centralno mesto preko kojeg Entity Framework Core zna:

* koje tabele postoje u bazi,
* kako su entiteti povezani,
* koja baza se koristi,
* kako se podešavaju relacije i ograničenja.

Primer opšte strukture:

using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
public DbSet EntityNames { get; set; }

```
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseSqlServer(
        @"Server=(localdb)\MSSQLLocalDB;Database=DatabaseName;Trusted_Connection=True;TrustServerCertificate=True;"
    );
}

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // FluentAPI konfiguracija ide ovde
}
```

}

---

## 6. DbSet svojstva

U `DbContext` klasi za svaki entitet koji treba da postoji kao tabela u bazi dodaje se `DbSet`.

Primer:

public DbSet EntityNames { get; set; }

`DbSet` predstavlja tabelu u bazi.

Na primer:

* model klasa predstavlja jedan red u tabeli,
* `DbSet` predstavlja celu tabelu.

U programu se preko `DbSet` svojstava dodaju, čitaju, menjaju i brišu podaci.

---

## 7. Podešavanje konekcije ka bazi

U konzolnoj aplikaciji se konekcija ka bazi najjednostavnije podešava u metodi `OnConfiguring`.

Primer:

protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
optionsBuilder.UseSqlServer(
@"Server=(localdb)\MSSQLLocalDB;Database=DatabaseName;Trusted_Connection=True;TrustServerCertificate=True;"
);
}

U ovom primeru koristi se SQL Server LocalDB.

Deo:

Server=(localdb)\MSSQLLocalDB

označava lokalni SQL Server.

Deo:

Database=DatabaseName

označava naziv baze koja će biti kreirana ili korišćena.

---

## 8. FluentAPI konfiguracija

FluentAPI konfiguracija se piše u metodi `OnModelCreating`.

Koristi se za precizno podešavanje modela, na primer:

* primarni ključevi,
* obavezna polja,
* maksimalna dužina stringova,
* relacije između tabela,
* strani ključevi.

Primer konfiguracije entiteta:

modelBuilder.Entity(entity =>
{
entity.HasKey(e => e.Id);

```
entity.Property(e => e.SomeProperty)
    .IsRequired()
    .HasMaxLength(100);
```

});

Primer relacije jedan-prema-više:

modelBuilder.Entity(entity =>
{
entity.HasMany(e => e.ChildEntities)
.WithOne(e => e.ParentEntity)
.HasForeignKey(e => e.ParentEntityId);
});

Ovo znači:

* jedan roditeljski entitet ima više povezanih entiteta,
* svaki povezani entitet pripada jednom roditeljskom entitetu,
* strani ključ se nalazi u povezanoj tabeli.

Ako zadatak traži da se relacije eksplicitno definišu putem FluentAPI-ja, relacije treba obavezno napisati u `OnModelCreating`.

---

## 9. Migracije

Migracije predstavljaju način na koji Entity Framework Core prati promene modela i pretvara ih u promene u bazi podataka.

Migracija se kreira kada se napravi ili promeni model.

Package Manager Console se otvara kroz:

`Tools` → `NuGet Package Manager` → `Package Manager Console`

Pre pokretanja migracija, preporučljivo je uraditi:

`Build` → `Build Solution`

Ako projekat ima greške, migracija neće moći da se kreira.

---

## 10. Kreiranje prve migracije

Prva migracija se najčešće naziva `Init` ili `InitialCreate`.

Komanda:

Add-Migration Init

Ova komanda ne pravi odmah bazu.

Ona pravi folder `Migrations` i fajlove koji opisuju kako baza treba da izgleda.

Nakon pokretanja komande, u projektu se pojavljuje folder:

Migrations

U njemu se nalaze fajlovi migracije i snapshot trenutnog modela.

---

## 11. Primena migracije na bazu

Da bi se migracija stvarno primenila na bazu, koristi se komanda:

Update-Database

Ova komanda kreira bazu i tabele, ili ažurira postojeću bazu na osnovu migracija.

Dakle:

* `Add-Migration` pravi opis promene.
* `Update-Database` primenjuje promenu na bazu.

---

## 12. Dodavanje nove migracije nakon izmene modela

Ako se kasnije promeni model, na primer dodavanjem nove klase, novog svojstva ili nove relacije, potrebno je napraviti novu migraciju.

Koraci su:

1. Izmeniti model klase.
2. Izmeniti `DbContext` ako je potrebno.
3. Dodati ili izmeniti FluentAPI konfiguraciju.
4. Uraditi `Build Solution`.
5. Napraviti novu migraciju.
6. Primeniti migraciju na bazu.

Komande:

Add-Migration NazivMigracije

Update-Database

Naziv migracije treba da opisuje promenu koja je urađena.

---

## 13. Provera baze

Baza se može proveriti kroz SQL Server Object Explorer.

U Visual Studio-u:

`View` → `SQL Server Object Explorer`

Zatim se pronađe:

`(localdb)\MSSQLLocalDB`

U okviru njega treba da se vidi kreirana baza.

U bazi se mogu proveriti tabele, kolone i podaci.

Entity Framework Core takođe pravi tabelu:

__EFMigrationsHistory

Ova tabela čuva informacije o migracijama koje su primenjene na bazu.

---

## 14. Rad sa podacima u Program.cs

U konzolnoj aplikaciji se konkretan rad sa podacima najčešće piše u `Program.cs`.

Prvo se napravi objekat context klase:

using var context = new AppDbContext();

Dodavanje podataka:

var entity = new EntityName
{
// postavljanje vrednosti svojstava
};

context.EntityNames.Add(entity);
context.SaveChanges();

Dodavanje više podataka:

context.EntityNames.AddRange(entity1, entity2);
context.SaveChanges();

Važno:

* `Add` i `AddRange` samo pripremaju podatke za dodavanje.
* `SaveChanges` stvarno upisuje promene u bazu.

---

## 15. Čitanje podataka

Podaci se čitaju preko `DbSet` svojstava iz context klase.

Primer:

var entities = context.EntityNames.ToList();

Sortiranje:

var entities = context.EntityNames
.OrderBy(e => e.SomeProperty)
.ToList();

Filtriranje:

var entity = context.EntityNames
.FirstOrDefault(e => e.SomeProperty == someValue);

`FirstOrDefault` vraća prvi pronađeni rezultat ili `null` ako rezultat ne postoji.

---

## 16. Učitavanje povezanih podataka

Ako je potrebno učitati povezane entitete, koristi se `Include`.

Za `Include` je potrebno dodati:

using Microsoft.EntityFrameworkCore;

Primer:

var entities = context.EntityNames
.Include(e => e.RelatedEntity)
.ToList();

Ako entitet ima kolekciju povezanih entiteta:

var entities = context.EntityNames
.Include(e => e.RelatedEntities)
.ToList();

`Include` se koristi kada su potrebni podaci iz povezane tabele.

---

## 17. Izmena postojećih podataka

Za izmenu postojećeg podatka prvo se pronađe entitet u bazi.

Primer:

var entity = context.EntityNames
.FirstOrDefault(e => e.Id == id);

if (entity == null)
{
Console.WriteLine("Podatak nije pronađen.");
}
else
{
entity.SomeProperty = newValue;

```
context.SaveChanges();

Console.WriteLine("Promena je sačuvana.");
```

}

Važno je da se nova vrednost računa ili postavlja na osnovu trenutnog objekta iz baze, ako zadatak to traži.

Promena se upisuje u bazu tek nakon poziva:

context.SaveChanges();

---

## 18. Brisanje podataka

Brisanje jednog entiteta:

var entity = context.EntityNames
.FirstOrDefault(e => e.Id == id);

if (entity != null)
{
context.EntityNames.Remove(entity);
context.SaveChanges();
}

Brisanje više entiteta:

context.EntityNames.RemoveRange(context.EntityNames);
context.SaveChanges();

Kod brisanja treba voditi računa o redosledu ako postoje relacije između tabela. Prvo se brišu zavisni entiteti, a zatim entiteti od kojih oni zavise.

---

## 19. Najčešće komande u Package Manager Console

Instaliranje EF Core paketa:

Install-Package Microsoft.EntityFrameworkCore

Install-Package Microsoft.EntityFrameworkCore.SqlServer

Install-Package Microsoft.EntityFrameworkCore.Tools

Kreiranje migracije:

Add-Migration NazivMigracije

Primena migracija na bazu:

Update-Database

Vraćanje baze na prethodnu migraciju:

Update-Database NazivPrethodneMigracije

Brisanje poslednje migracije ako nije primenjena na bazu:

Remove-Migration

---

## 20. Tipičan tok rada na EF Core zadatku

Opšti redosled rada:

1. Kreirati Console App projekat.
2. Instalirati EF Core pakete.
3. Napraviti foldere `Models` i `Data`.
4. Napraviti model klase.
5. Napraviti `DbContext` klasu.
6. Dodati `DbSet` svojstva u context.
7. Podesiti konekciju ka bazi u `OnConfiguring`.
8. Definisati relacije i ograničenja kroz FluentAPI u `OnModelCreating`.
9. Uraditi `Build Solution`.
10. Kreirati migraciju komandom `Add-Migration`.
11. Primeniti migraciju komandom `Update-Database`.
12. Po potrebi proširiti model.
13. Za svaku veću izmenu modela napraviti novu migraciju.
14. U `Program.cs` napisati logiku za dodavanje, čitanje, izmenu ili brisanje podataka.
15. Pokrenuti aplikaciju i proveriti rezultate u konzoli i bazi.

---

## 21. Najčešće greške

### Build failed

Migracija ne može da se kreira ako projekat ima greške.

Rešenje:

`Build` → `Build Solution`

Zatim ispraviti greške prikazane u Error List prozoru.

### No database provider has been configured

Ova greška znači da nije podešen provider baze.

Rešenje je proveriti da li u `OnConfiguring` postoji:

optionsBuilder.UseSqlServer(...);

Takođe treba proveriti da li je instaliran paket:

Microsoft.EntityFrameworkCore.SqlServer

### UseSqlServer nije prepoznat

Ako metoda `UseSqlServer` nije prepoznata, najčešći razlog je da nije instaliran paket:

Microsoft.EntityFrameworkCore.SqlServer

Takođe treba proveriti da li postoji:

using Microsoft.EntityFrameworkCore;

### Migracija ne vidi promene

Ako migracija ne registruje očekivane promene, treba proveriti:

* da li su klase sačuvane,
* da li su dodata `DbSet` svojstva,
* da li su relacije pravilno napisane,
* da li projekat uspešno prolazi build.

### Dupliranje podataka pri svakom pokretanju programa

Ako se u `Program.cs` svaki put dodaju isti podaci, pri svakom pokretanju aplikacije mogu nastati duplikati.

Rešenja su:

* obrisati postojeće podatke pre dodavanja novih,
* proveriti da li podatak već postoji pre dodavanja,
* koristiti seed logiku pažljivo.

---

## 22. Kratko pravilo za pamćenje

Model klase opisuju podatke.

`DbContext` opisuje bazu i relacije.

`DbSet` predstavlja tabelu.

FluentAPI definiše pravila i veze između tabela.

`Add-Migration` pravi opis promene baze.

`Update-Database` primenjuje promenu na bazu.

`Program.cs` u konzolnoj aplikaciji sadrži konkretnu logiku rada sa podacima.
