# KorppiSync

Ohjelma joka synkronoi Korpin kalenterin ajanvaraukset Googlen kalenteriin.

## Käyttö

### 1.

Ota itsellesi käyttöön Googlen kalenteri-api:

https://developers.google.com/calendar/quickstart/dotnet

Suorita sivun ohjeiden kohta 1.

Tallenna ladattu `credentials.json` `korppiSync`-kansioon, missä ohjelman muutkin kooditiedostot sijaitsevat.

### 2.

Mene osoitteeseen calendar.google.com ja luo itsellesi uusi kalenteri johon tapahtumat lisätään.

Kalenterin nimellä ei ole väliä.

**ÄLÄ** käytä kalenteria johon esim. itse lisäät manuaalisesti tapahtumia. Voi tapahtua ikävyyksiä.

Luodun kalenterin asetuksista löytyy "Integrate calendar" otsikon alta kalenterin id, sitä tarvitaan seuraavaksi.

### 3. 

Luo sinne samaiseen `KorppiSync`-kansioon vielä `Settings.json`, jonka sisältö on mallia:

```
{
  "korppiUser": "Korpin käyttäjätunnus",
  "korppiPass": "Salasana",
  "calendarId": "Käytettävän googlekalenterin id"
}
```

### 4. Ensimmäinen ajo

Kun ensimmäisen kerran ajat ohjelman, kysyy google vielä että haluatko aivan varmasti sallia tämmöisen pelottavan ohjelman hallita sinun kalenteria, vastaile kyllä.

Sen jälkeen se lataa Korpin kalenterin hakuun käytettävä Chromium selaimen, tässä voi kestää hetki.

Näiden jälkeen Korpin kalenterin ajanvaraus (ja oletettavasti muutkin) tapahtumat kopioituvat Googlen kalenteriin. 
Jos ohjelma ajetaan uudestaan, päivitetään kalenterin tapahtumiin varatut ajat, sekä mahdollisesti varauksien perumiset.


## Puutteet

Toistaiseksi jos korpin tapahtuman poistaa, tai sen ajankohtaa muuttaa, ei tämä muutos päivity Googlen kalenteriin.

