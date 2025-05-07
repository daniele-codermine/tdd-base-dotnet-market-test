## Esame Teorico

Rispondere alle domande teoriche del corso: 
https://forms.gle/m25tSJ7tmbtnmB346

## Specifiche del progetto

Si richiede di implementare la gestione della cassa di un supermercato. Lo scopo della cassa è generare correttamente uno scontrino sulla base dei prodotti inseriti e dei prerequisiti qui sotto indicati.

### Requisiti Base
- Aggiunta singolo prodotto con prezzo fisso
- Aggiunta di due prodotti con prezzi diversi
- Cancellazione di un prodotto
- Modifica della quantità di un prodotto già inserito
- Gestione prezzi con i decimali

### Requisiti Intermedi
- Modalità "Sconto Fisso", coupon applicato al totale: Verifica che un sconto fisso (es. 5€) venga sottratto correttamente dal totale.
- Modalità "Sconto Percentuale", coupon applicato al totale: Se il totale è >= 100€, sconto 10%, totale atteso: 90€.
- 3x2: Acquistando 3 prodotti, ne verrà scontato 1.
- Modalità "Bundle": Acquistando, ad esempio, una mela (3€), una banana (2€) ed una arancia (5€) verrà applicato il prezzo bundle "Macedonia" di 8€W 

### Requisiti Avanzati:
- Gestione IVA prodotti sui prodotti: ogni prodotto ha una aliquota IVA (20%, 10%, 4%, Esente Iva), il totale deve rispettare l'applicazione dell'IVA. Es. Arancia 5€ + 10% IVA = 5.5€
- Stampa dello scontrino a video: verifica formattazione dei prodotti, confronto con risultato di stampa atteso (Snapshot testing)