# MeteoApp

## Project for Mobile application development

# Guida all'utilizzo
### Pagina home
All'avvio dell'applicazione viene aperta la pagina principale, in cui è possibile aggiungere varie città per monitorarne la meteo.

![alt text](https://github.com/JulianCummaudo/MeteoApp/blob/dev/Assets/HomePage.png?raw=true)

Premendo quindi sul tasto Add in alto a destra, diventa possibile cercare una città per nome, e aggiungere questa città alla pagina principale.
Una volta aggiunta una città, è possibile vedere la meteo per quella posizione, e in caso eliminarla.
Le città salvate sono persistenti tramite un database, e tramite il supporto a Appwrite, le città per ogni utente sono le stesse.

![alt text](https://github.com/JulianCummaudo/MeteoApp/blob/dev/Assets/AddcityPage.png?raw=true)

### Pagina con current location
In questa sezione dell'applicazione è necessario attivare il permesso della localizzazione, una volta ottenuto verrà mostrata la meteo nella posizione attuale.

![alt text](https://github.com/JulianCummaudo/MeteoApp/blob/dev/Assets/CurrentLocationPage.png?raw=true)

In caso di problemi, per esempio se il popup che richiede la posizione non appare, è probabile che android abbia bisogno il permesso direttamente dalle impostazioni dell'applicazione, in quanto puó essere stato negato in precedenza.

### Pagina della mappa 
L'applicazione offre una mappa con cui è possibile verificare velocemente la meteo in un qualsiasi paese del mondo.
É sufficiente premere su qualsiasi posizione della mappa per ottenere una visione della meteo attuale in quella posizione.


![alt text](https://github.com/JulianCummaudo/MeteoApp/blob/dev/Assets/MapPage.png?raw=true)

![alt text](https://github.com/JulianCummaudo/MeteoApp/blob/dev/Assets/CityDetailsPage.png?raw=true)

### Pagina web 
Quest'ultima pagina è dedicata al supporto di Blazor, allo stato finale del progetto ci siamo limitati a utilizzare html e css per fare una stampa piú moderna della meteo attuale

![alt text](https://github.com/JulianCummaudo/MeteoApp/blob/dev/Assets/BlazorPage.png?raw=true)


# Feature previste 
Per la realizzazione di questo progetto erano previste diverse feature:

- [x] Creazione di un progetto MAUI 
- [x] Utilizzo di OpenWeatherMap
- [x] Esplorare layout diversi
- [x] Utilizzare la posizione corrente dell'utente
- [x] Utilizzo di SQLite per persistere le città salvate
- [x] Rendere l'applicazione multilingua
- [x] Ricavare le mappe di default del sistema operativo e utilizzarle
- [x] Utilizzare notifiche locali e remote tramite Firebase
- [x] Sperimentare con Blazor per visualizzare html dentro un container
- [x] Sincronizzare le città salvate tramite Appwrite
- [x] Distribuire il risultato tramite APK

# Struttura del progetto 
L'applicazione segue una struttura organizzata secondo il pattern MVVM (Model - View - ViewModel). Questo approccio permette di separare la logica applicativa dall'interfaccia grafica, rendendo il progetto più ordinato, manutenibile e facilmente estendibile.

## Cartelle principali
├── Models
├── ViewModels
├── Views
├── Services
├── Components
├── Resources
├── Platforms
└── wwwroot

### Models
Contiene le classi che rappresentano i dati utilizzati dall'applicazione: dati sulle città, configurazioni del database, e dati meteo ricevuti dalle API di OpenWeatherMap

### ViewModels
Gestiscono la logica di presentazione tra View e dati, recuperando dati dai servizi, aggiornando l'interfaccia e principalmente separando la UI dalla logica

### Views
Raccoglie tutte le pagine visibili all’utente, costruite con XAML e C#.

### Services
Contiene i servizi responsabili delle funzionalità principali dell'applicazione.

### Components
Cartella dedicata all’integrazione con Blazor Hybrid, contiene pagine .razor e componenti web usati dentro l’applicazione MAUI tramite BlazorWebView.
In questo progetto è stata utilizzata per sperimentare interfacce HTML/CSS integrate in un’app mobile nativa.

### Resources
Include tutte le risorse grafiche dell'app:
- icone
- immagini
- font

### Platforms
Codice specifico per ogni sistema operativo supportato.

### wwwroot
Cartella utilizzata dalla parte Blazor per contenere file statici web.

# Possibili miglioramenti futuri

Nonostante l’applicazione risulti già pressocchè completa, esistono diversi margini di miglioramento che potrebbero renderla più moderna e interessante dal punto di vista dell’esperienza utente.

### Sistema notifiche più avanzato

Attualmente le notifiche locali vengono inviate quando la temperatura scende sotto una soglia prefissata. Una possibile evoluzione sarebbe permettere all’utente di personalizzare questo valore direttamente dalle impostazioni dell’app, scegliendo ad esempio la temperatura minima desiderata oppure condizioni specifiche come pioggia, neve o vento forte.

Le notifiche remote, implementate principalmente a scopo didattico, mostrano attualmente solo un semplice messaggio testuale. In futuro potrebbero essere utilizzate per inviare allerte meteo reali, avvisi personalizzati o aggiornamenti automatici relativi alle città salvate.

### Integrazione Blazor più completa

La pagina realizzata con Blazor dimostra il corretto utilizzo della tecnologia all’interno di un progetto MAUI Hybrid, ma al momento ha una funzione limitata. Sarebbe interessante espanderla con contenuti realmente utili.

### Mappa interattiva con overlay meteo

La sezione mappa è già funzionante e consente di ottenere informazioni premendo su una località. Un miglioramento particolarmente interessante sarebbe l’aggiunta di un overlay meteo direttamente sulla mappa.
In questo modo l’utente potrebbe visualizzare rapidamente la situazione meteorologica senza dover selezionare manualmente un punto preciso.

### Restyling grafico generale

Un aggiornamento dell’interfaccia grafica renderebbe l’applicazione più moderna, intuitiva e adatta a un utilizzo quotidiano anche da parte di utenti esterni al progetto. Alcuni possibili interventi:

* animazioni leggere
* palette colori dinamica in base al meteo
* layout più puliti

Un design più rifinito aumenterebbe notevolmente la percezione di qualità complessiva dell’applicazione.
