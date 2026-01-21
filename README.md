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

- Invoicing (Billing) Context: Responsabil de primirea comenzilor deja validate (OrderPlacedEvent), calcularea sumelor financiare (TVA, total) pe baza liniilor de comandă și transformarea lor într‑o factură fiscală completă (PaidInvoice). De asemenea, se ocupă de generarea și publicarea evenimentului InvoicePaidEvent către celălalt context (Shipping), precum și de încărcarea facturilor în baza de date.
  
- Shipping Context: Responsabil de generarea AWB-urilor, calculul costului de transport si finalizarea livrarii.

## Event Storming Results
[Link către diagrama Event Storming sau imagine exportată din Miro/Lucid] (Aici ar trebui să apară fluxul: OrderPlaced -> InvoiceGenerated -> PaymentSucceeded -> ShippingRequested -> PackageShipped)

## Implementare
### Value Objects
Utilizăm Value Objects pentru a preveni "Primitive Obsession" și a garanta validitatea datelor la nivelul cel mai jos.
- Context Vanzari:
  - ProductCode: Format strict validat prin Regex (^PROD-[0-9]{4}$ - ex PROD-0001)
  - Quantity: Număr întreg strict pozitiv (>0) limitat la un maxim per comanda.
  - Money: Valoare zecimală + Monedă, nu permite valori negative.
  - Address: Structură imutabilă ce grupează datele de livrare (Oraș, Județ, Stradă), validată la creare.

- Context Facturare:
  - BillingAddress: Obiect imutabil care grupează toate câmpurile de adresă de facturare (județ, oraș, stradă, cod poștal).
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
  - ValidatedOrder: Datele au fost convertite în Value Objects și validate (ex: stocul există, cantitățile sunt corecte).
  - CalculatedOrder: S-au aplicat prețurile unitare din baza de date și s-a calculat totalul comenzii.
  - PlacedOrder: Starea finală în acest context. Comanda este salvată și confirmată, fiind emis un eveniment pentru a notifica Billing Context.
  - InvalidOrder: Stare rezultată în cazul eșuării oricărei validări anterioare, conținând lista motivelor de eroare (fără a arunca excepții în flow).

- Context Facturare:
  - UnvalidatedInvoice: Reprezintă proiectul brut de factură obținut direct din comandă; poate conține date lipsă sau invalide (adresă, prețuri, cantități) și nu este încă pregătit pentru emitere.
  - ValidatedInvoice: Stare în care toate datele au fost verificate și convertite în Value Objects (BillingAddress, Money, TaxRate); factura este coerentă din punct de vedere fiscal și poate intra în etapa de calcul.
  - CalculatedInvoice: Factură pentru care s-au calculat corect subtotalul, TVA și totalul final pe baza liniilor de comandă și a cotei de TVA; este pregătită pentru a fi emisă clientului sau trimisă spre plată.
  - PaidInvoice: Starea finală în contextul de facturare, în care plata a fost confirmată pentru factura respectivă; la acest punct se poate publica evenimentul InvoicePaidEvent și se pot declanșa fluxuri din alte contexte (ex. livrare).

- Context Livrare:
  - UnvalidatedShipment: Cererea de livrare brută, venită după plata facturii.
  - ValidatedShipment: Adresa de livrare e validă, 
  - CalculatedShipment: Costul de transport calculat.
  - ManifestedShipment: AWB-ul a fost generat si livrarea finalizată.

### Operations
Operațiile sunt funcții pure (pe cât posibil) care transformă o stare în alta.

- Context Vanzari:
  - ValidateOrderOperation: UnvalidatedOrder → IOrder (Verifică formatul codurilor de produs și disponibilitatea acestora în baza de date prin IProductRepository). Returnează ValidatedOrder sau InvalidOrder.
  - CalculatePricesOperation: ValidatedOrder → IOrder (Preia prețurile actuale din DB și calculează totalul per linie și totalul general). Returnează CalculatedOrder sau InvalidOrder.
  - PlaceOrderOperation: CalculatedOrder → IOrder (Persistă comanda în SQL Server folosind IOrderRepository, decrementează stocul și publică mesajul asincron OrderConfirmedMessage). Returnează PlacedOrder.

- Context Facturare:
  - GenerateInvoiceDraftOperation: GenerateInvoiceDraftCommand → UnvalidatedInvoice (Primește datele venite din contextul de vânzări: OrderId, CustomerId, BillingAddress, Lines, Amount, PlacedDate și le proiectează într-un obiect de domeniu UnvalidatedInvoice, fără să facă încă validări complexe; practic traduce DTO-ul extern în model intern de facturare.)
  - ValidateInvoiceOperation: UnvalidatedInvoice → ValidatedInvoice (Verifică integritatea datelor de facturare: validează BillingAddress, convertește prețurile și cantitățile în Money și alte value object‑uri, și se asigură că structura facturii respectă regulile domeniului; rezultatul este un ValidatedInvoice gata pentru calculul sumelor.)
  - CalculateInvoiceTotalsOperation: ValidatedInvoice → CalculatedInvoice (Parcurge liniile facturii aplică TaxRate pentru a obține TVA și construiește totalul de plată ca Money; rezultatul este CalculatedInvoice, care conține toate valorile financiare necesare emiterii facturii.)
  - MarkInvoiceAsPaidOperation: CalculatedInvoice + PaymentConfirmedEvent → PaidInvoice (Combină factura calculată cu evenimentul de plată confirmată: sumă plătită, momentul plății; și produce PaidInvoice, starea finală în care factura este marcată ca plătită și din care se generează evenimentul InvoicePaidEvent ce va fi trimis către contextul Livrare.

- Context Livrare:
  - ProcessShipmentOperation: command -> UnvalidatedShipment 
  - ValidateShipmentOperation: UnvalidatedShipment -> ValidatedShipment
  - CalculateShippingCostOperation: ValidatedShipment -> CalculatedShipment
  - ManifestShipmentOperation: CalculatedShipment -> ManifestedShipment

### Workflow
- PlaceOrderWorkflow: Acest workflow orchestrează procesul de cumpărare (Pipeline):
  - Primește starea inițială UnvalidatedOrder (convertită din PlaceOrderRequest în Controller).
  - Execută ValidateOrderOperation (returnează ValidatedOrder sau InvalidOrder).
  - Dacă rezultatul e valid, execută CalculatePricesOperation (returnează CalculatedOrder).
  - Execută PlaceOrderOperation (salvează comanda în DB și returnează PlacedOrder).
  - Ca efect al ultimei operații, publică mesajul de integrare OrderConfirmedMessage pe topicul orders-confirmed din Azure Service Bus.

- BillingWorkflow:
  - Primește un GenerateInvoiceDraftCommand construit din OrderPlacedEvent, împreună cu PaymentConfirmedEvent.
  - Execută GenerateInvoiceDraftOperation pentru a crea UnvalidatedInvoice, adică draftul brut de factură.
  - Rulează ValidateInvoiceOperation, care transformă UnvalidatedInvoice în ValidatedInvoice după ce verifică adresa de facturare, sumele și regulile fiscale.
  - Rulează CalculateInvoiceTotalsOperation, care produce CalculatedInvoice calculând subtotalul, TVA și totalul de plată.
  - Rulează MarkInvoiceAsPaidOperation, care combină CalculatedInvoice cu confirmarea de plată și generează PaidInvoice. Din această stare finală se construiește și se publică evenimentul InvoicePaidEvent, folosit de contextul Livrare.
    
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
- Sugestii pentru implementare.
- Idei pentru structurarea mesajelor JSON pentru comunicarea asincronă.

### Limitări ale AI identificate
- Dificultate în a înțelege nuanțele specifice ale comunicării asincrone între cele 3 contexte (uneori sugera apeluri directe HTTP în loc de mesagerie).
- Codul generat uneori ignora/uita celelalte script-uri deja existente.

### Prompturi Utile
```
"Generează o clasă C# record 'Money.cs'..."
```

## Design Decisions
- Am ales să folosim Azure Service Bus pentru comunicarea asincronă între contexte.
- Logica de domeniu este pură, fără dependențe de baza de date.
