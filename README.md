# MeteoApp

## Project for Mobile application development

# guida all'utilizzo
### Pagina home
All'avvio dell'applicazione viene aperta la pagina principale, in cui è possibile aggiungere varie città per monitorarne la meteo.

![alt text](https://github.com/JulianCummaudo/MeteoApp/blob/dev/HomePage.png?raw=true)

Premendo quindi sul tasto Add in alto a destra, diventa possibile cercare una città per nome, e aggiungere questa città alla pagina principale.
Una volta aggiunta una città, è possibile vedere la meteo per quella posizione, e in caso eliminarla.
Le città salvate sono persistenti tramite un database, e tramite il supporto a Appwrite, le città per ogni utente sono le stesse.

![alt text](https://github.com/JulianCummaudo/MeteoApp/blob/dev/AddcityPage.png?raw=true)

### Pagina con current location
In questa sezione dell'applicazione è necessario attivare il permesso della localizzazione, una volta ottenuto verrà mostrata la meteo nella posizione attuale.

![alt text](https://github.com/JulianCummaudo/MeteoApp/blob/dev/CurrentLocationPage.png?raw=true)

In caso di problemi, per esempio se il popup che richiede la posizione non appare, è probabile che android abbia bisogno il permesso direttamente dalle impostazioni dell'applicazione, in quanto puó essere stato negato in precedenza.

### Pagina della mappa 
L'applicazione offre una mappa con cui è possibile verificare velocemente la meteo in un qualsiasi paese del mondo.
É sufficiente premere su qualsiasi posizione della mappa per ottenere una visione della meteo attuale in quella posizione.


![alt text](https://github.com/JulianCummaudo/MeteoApp/blob/dev/MapPage.png?raw=true)


![alt text](https://github.com/JulianCummaudo/MeteoApp/blob/dev/CityDetailsPage.png?raw=true)

### Pagina web 
Quest'ultima pagina è dedicata al supporto di Blazor, allo stato finale del progetto ci siamo limitati a utilizzare html e css per fare una stampa piú moderna della meteo attuale

![alt text](https://github.com/JulianCummaudo/MeteoApp/blob/dev/BlazorPage.png?raw=true)


# feature previste 
# struttura del progetto 
# metodi e file particolari 