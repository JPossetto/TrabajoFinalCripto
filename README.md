# Cartera Cripto - Trabajo Final Programacion III

## Sobre el proyecto

Hola! Soy Joaquin y este proyecto fue desarrollado como trabajo final para la materia **Programacion III**.

La aplicacion permite registrar clientes y administrar operaciones de compra y venta de criptomonedas, manteniendo un historial de movimientos y calculando automaticamente los montos en pesos argentinos utilizando cotizaciones obtenidas desde la API de CriptoYa.

La idea del sistema es simular una herramienta sencilla para llevar el control de una cartera de criptomonedas de distintos clientes, cumpliendo con los requisitos planteados en la consigna de la materia.

---

## Tecnologias utilizadas

### Backend

* ASP.NET Web API
* Entity Framework Core

### Frontend

* Vue.js 3
* HTML
* JavaScript

### Base de datos

* SQLite

### Servicios externos

* API de CriptoYa

---

## Algunas decisiones del desarrollo

Para este proyecto utilice **SQLite** como motor de base de datos. Durante la carrera generalmente trabajamos con SQL Server, pero quise aprovechar este trabajo para probar una algo diferente por curiosidad y para aprender un poco mas sobre otra tecnologia.

Ademas, tuvo sus beneficios ya que SQLite resulta practico para este proyecto porque no requiere instalar ni configurar un servidor de base de datos, lo que facilita ejecutar y probar la aplicacion.

Para obtener las cotizaciones de las criptomonedas utilice la API de **CriptoYa**, consultando los precios del exchange **SatoshiTango**. De esta forma, los valores de compra y venta se calculan utilizando informacion real al momento de registrar cada operacion.

---

## Funcionalidades implementadas

### Gestion de clientes

* Alta de clientes.
* Validacion de nombre y correo electronico.

### Operaciones de criptomonedas

* Registro de compras.
* Registro de ventas.
* Validacion para impedir ventas superiores al saldo disponible.
* Calculo automatico del importe en pesos argentinos.

### Historial de movimientos

* Consulta de todas las transacciones.
* Filtro por cliente.
* Visualizacion de detalles de cada operacion.
* Edicion de movimientos.
* Eliminacion de movimientos.

### Criptomonedas utilizadas

El sistema trabaja con tres criptomonedas:

* Bitcoin (BTC)
* Ethereum (ETH)
* USDC

---

## Como ejecutar el proyecto

### 1. Ejecutar el backend

Ubicarse dentro de:

```bash
Backend/CriptoCarteraApi
```

Restaurar dependencias:

```bash
dotnet restore
```

Iniciar la API:

```bash
dotnet run
```

La aplicacion genera automaticamente la base de datos utilizando Entity Framework.

El archivo SQLite se crea localmente como:

```bash
cartera_cripto.db
```

---

### 2. Configurar la URL de la API

Si el backend se ejecuta en un puerto diferente, actualizar la variable:

```javascript
apiUrl
```

ubicada en:

```bash
Frontend/app.js
```

Por defecto se encuentra configurada como:

```javascript
apiUrl: "http://localhost:5147"
```

---

### 3. Ejecutar el frontend

Abrir el archivo:

```bash
Frontend/index.html
```

en el navegador.

---

## Endpoints principales

### Clientes

```http
GET    /client
GET    /client/{id}
POST   /client
```

### Transacciones

```http
GET     /transactions
GET     /transactions/{id}
GET     /transactions?clientId={id}
POST    /transactions
PATCH   /transactions/{id}
DELETE  /transactions/{id}
```

---

## Consideraciones

* Las compras utilizan el valor **ask** obtenido desde CriptoYa.
* Las ventas utilizan el valor **bid**.
* Los montos en pesos se calculan en el backend para evitar inconsistencias.
* No se permite vender mas criptomonedas de las que posee un cliente.

---

## Contexto academico

Este proyecto fue realizado como trabajo final individual para la materia **Programacion III** de la **Tecnicatura Universitaria en Programacion**.

El objetivo principal fue aplicar conceptos de desarrollo full stack utilizando Vue.js, ASP.NET, bases de datos SQL, APIs REST y consumo de servicios externos.
