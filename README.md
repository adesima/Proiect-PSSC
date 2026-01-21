# E-Commerce System (Order, Billing & Shipping) 

## Echipa
- Isac Lucas-Horatiu
- Lemnaru Alin-Gabriel
- Sima Adelin-Sebastian

## Domeniul Ales
Sistem de Gestiune Comenzi Magazin Online (Retail / E-commerce)

## Descriere
Proiectul vizează implementarea unui sistem distribuit pentru gestionarea fluxului complet al unei comenzi într-un magazin online. Sistemul este împărțit în trei bounded context-uri distincte care comunică asincron prin mesaje (events).

Obiectivul principal este modelarea corectă a domeniului folosind principii DDD (Domain-Driven Design) și o abordare funcțională în C# (sistem de tipuri bogat, imutabilitate, pipeline-uri de operații).

## Bounded Contexts Identificate
Fiecare membru al echipei este responsabil de unul dintre următoarele contexte:

- Ordering Context: Responsabil de preluarea comenzilor, validarea datelor de intrare (produs, cantitate, adresă livrare), verificarea disponibilității stocului și calculul valorii totale a comenzii. Output-ul său este evenimentul OrderPlacedEvent care declanșează procesul de facturare.

- Invoicing (Billing) Context: Responsabil de procesarea plăților, generarea facturilor fiscale pe baza comenzilor validate și calculul taxelor (TVA).

- Shipping Context: Responsabil de generarea AWB-urilor, calculul costului de transport si finalizarea livrarii.

## Event Storming Results
[Link către diagrama Event Storming sau imagine exportată din Miro/Lucid] (Aici ar trebui să apară fluxul: OrderPlaced -> InvoiceGenerated -> PaymentSucceeded -> ShippingRequested -> PackageShipped)

## Implementare
### Value Objects
Utilizăm Value Objects pentru a preveni "Primitive Obsession" și a garanta validitatea datelor la nivelul cel mai jos.
- Context Vanzari:
  - ProductCode: Format specific (ex: PROD-12345).
  - Quantity: Număr întreg strict pozitiv (>0) limitat la un maxim per comanda.
  - Money/Price: Valoare zecimală + Monedă, nu permite valori negative.
  - ShippingAddress: Structură complexă (stradă, oraș, cod poștal validat).

- Context Facturare:
  - InvoiceNumber: cod unic al facturii. 
  - BillingAddress: adresa de facturare (stradă, oraș, cod poștal validat).
  - Money/Price: Valoare zecimală + Monedă, nu permite valori negative.
  - TaxRate: procent TVA (ex: 19%).

- Context Livrare:
  - AwbCode: Format specific curierului.
  - Moneu:  Valoare zecimală + Monedă, nu permite valori negative.
  - ShippingAddress: Structură complexă (județ, stradă, oraș, cod poștal)
   
### Entity States (Exemplu pentru Workflow-ul "Preluare Comandă")
Stările sunt modelate ca tipuri distincte (clase/record-uri) pentru a forța verificarea lor la compilare.

- Context Vanzari:
  - UnvalidatedOrder: Comanda brută primită de la client (poate avea stoc lipsă, adresă invalidă).
  - ValidatedOrder: Comanda a trecut validările, stocul este rezervat.
  - CalculatedOrder: Prețul total (inclusiv discount-uri) a fost aplicat.
  - PlacedOrder: Starea finală în acest context. Comanda este salvată și confirmată, fiind emis un eveniment pentru a notifica Billing Context.

- Context Facturare:
  - UnvalidatedInvoice: Factură inițială derivată dintr-o comandă, care poate avea date fiscale sau adresă de facturare invalide / incomplete.
  - ValidatedInvoice: Factură pentru care au fost verificate datele clientului, adresa de facturare și cotele de TVA, gata pentru calculul sumelor.
  - CalculatedInvoice: Factură cu subtotal, TVA și total final calculat, pregătită de emis către client.
  - PaidInvoice: Factură pentru care plata a fost confirmată și poate declanșa workflow-ul de livrare, similar cu PaidOrder.

- Context Livrare:
  - UnvalidatedShipment: Cererea de livrare brută, venită după plata facturii.
  - ValidatedShipment: Adresa de livrare e validă, 
  - CalculatedShipment: Costul de transport calculat.
  - ManifestedShipment: AWB-ul a fost generat si livrarea finalizată.

### Operations
Operațiile sunt funcții pure (pe cât posibil) care transformă o stare în alta.

- Context Vanzari:
  - ValidateOrder: UnvalidatedOrder -> Result<ValidatedOrder> (Verifică formatul adresei și disponibilitatea stocului prin interogarea unui Read Model).
  - CalculatePrices: ValidatedOrder -> CalculatedOrder (Aplică prețurile unitare din catalog și calculează totalul).
  - PlaceOrder: CalculatedOrder -> PlacedOrder (Finalizează comanda, persistă starea și publică evenimentul OrderPlacedEvent).

- Context Facturare:
  - GenerateInvoiceDraft: UnvalidatedOrder -> Result<UnvalidatedInvoice> (Construiește un draft de factură pe baza comenzii, fără garanții de validitate fiscală).
  - ValidateInvoice: UnvalidatedInvoice -> Result<ValidatedInvoice> (Verifică datele clientului, adresa de facturare și regulile de TVA).
  - CalculateInvoiceTotals: ValidatedInvoice -> CalculatedInvoice (Calculează subtotalul, TVA și totalul de plată).
  - MarkInvoiceAsPaid: CalculatedInvoice -> PaidInvoice (Actualizează starea facturii la plătită pe baza confirmării de plată).

- Context Livrare:
  - ProcessShipmentOperation: command -> UnvalidatedShipment 
  - ValidateShipmentOperation: UnvalidatedShipment -> ValidatedShipment
  - CalculateShippingCostOperation: ValidatedShipment -> CalculatedShipment
  - ManifestShipmentOperation: CalculatedShipment -> ManifestedShipment

### Workflow
- PlaceOrderWorkflow: Acest workflow orchestrează procesul de cumpărare:
  - Primește PlaceOrderCommand.
  - Execută ValidateOrder.
  - Dacă e valid, execută CalculatePrices.
  - Execută PlaceOrder.
  - Publică evenimentul OrderPlacedEvent pe Service Bus (trigger pentru Billing).

- BillingWorkflow:
  - Primește eveniment OrderPlacedEvent.
  - Creează UnvalidatedInvoice din comandă.
  - Rulează ValidateInvoice → ValidatedInvoice.
  - Rulează CalculateInvoiceTotals → CalculatedInvoice.
  - La confirmarea plății, marchează factura ca PaidInvoice și publică evenimentul InvoicePaidEvent pentru Shipping.
    
- ShippingWokflow
  - Primește evenimentul InvoicePaidEvent.
  - Creează UnvalidatedShipment.
  - Validează adresa
  - Calculează costul transportului și generează AWB.
  - Publică evenimentul ManifestedShipment.
  
## Rulare
```bash

# Compile solution
dotnet build

# Run Ordering Context (Console App)
dotnet run --project src/Ordering.ConsoleApp

# Run Tests
dotnet test
```

## Lecții Învățate
### Ce a funcționat bine cu AI
- Generarea rapidă a structurilor de tip record (C# 9+) pentru stările imutabile.
- Sugestii pentru implementarea pattern-ului Result<T> pentru a evita excepțiile în flow control.
- Idei pentru structurarea mesajelor JSON pentru comunicarea asincronă.

### Limitări ale AI identificate
- Dificultate în a înțelege nuanțele specifice ale comunicării asincrone între cele 3 contexte (uneori sugera apeluri directe HTTP în loc de mesagerie).
- Codul generat uneori ignora tratarea cazurilor de eroare (failure paths) în pipeline-uri complexe.

### Prompturi Utile
```
"Generează o clasă C# record 'Quantity' care să nu permită valori negative 
și să returneze un 'Result' la creare, în stil funcțional."
```

## Design Decisions
- Am ales să folosim Azure Service Bus pentru comunicarea asincronă între contexte.
- Logica de domeniu este pură, fără dependențe de baza de date (Persistence Ignorance).
- Detalii complete în [docs/DesignDecisions.md].
