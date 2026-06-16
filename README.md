# Cartera Cripto - Trabajo Final Programacion III

## Sobre el proyecto

Hola! Soy Joaquin y este proyecto fue desarrollado como trabajo final para la materia **Programacion III**.

La aplicacion permite registrar clientes y administrar operaciones de compra y venta de criptomonedas, manteniendo un historial de movimientos y calculando automaticamente los montos en pesos argentinos utilizando cotizaciones obtenidas desde la API de CriptoYa.

Originalmente el frontend estaba hecho utilizando Vue por CDN, pero posteriormente lo migre a una estructura basada en **Vue 3 + Vite** para organizar mejor el codigo y trabajar con componentes de una forma mas ordenada.

---

## Tecnologias utilizadas

### Backend

* ASP.NET Core Web API
* Entity Framework Core

### Frontend

* Vue.js 3
* Vite
* HTML
* CSS
* JavaScript

### Base de datos

* SQLite

### Servicios externos

* API de CriptoYa

---

## Algunas decisiones del desarrollo

Para este proyecto utilice **SQLite** como motor de base de datos. Durante la carrera generalmente trabajamos con SQL Server, pero quise aprovechar este trabajo para probar algo diferente por curiosidad y aprender un poco mas sobre otra tecnologia.

Ademas, SQLite me parecio una buena opcion porque no requiere instalar ni configurar un servidor de base de datos, lo que facilita bastante ejecutar y probar el proyecto.

Tambien migre el frontend a **Vite** para trabajar con componentes `.vue` y tener una estructura mas organizada. Una vez compilado, el frontend se sirve directamente desde ASP.NET, por lo que toda la aplicacion puede ejecutarse desde un unico proyecto.

Para obtener las cotizaciones de las criptomonedas utilice la API de **CriptoYa**, consultando los precios del exchange **SatoshiTango**. De esta forma, los valores de compra y venta se calculan utilizando informacion real al momento de registrar cada operacion.

---

## Funcionalidades implementadas

### Gestion de clientes

* Alta de clientes.
* Validacion de nombre y correo electronico.

### Operaciones de criptomonedas

* Registro de compras utilizando el valor **ask**.
* Registro de ventas utilizando el valor **bid**.
* Validacion para impedir ventas superiores al saldo disponible.
* Calculo automatico del importe en pesos argentinos desde el backend.

### Historial de movimientos

* Consulta de todas las transacciones.
* Filtro por cliente.
* Visualizacion de detalles de cada operacion.
* Edicion de movimientos.
* Eliminacion de movimientos.

---

## Estructura del proyecto

```text
Backend/
└── CriptoCarteraApi/

Frontend/
```

* `Backend/CriptoCarteraApi/`: API desarrollada en ASP.NET Core junto con la carpeta `wwwroot` donde se aloja la version compilada del frontend.
* `Frontend/`: proyecto Vue 3 + Vite utilizado durante el desarrollo.

---

## Como ejecutar el proyecto

Al estar el frontend compilado e integrado dentro del backend, solamente es necesario ejecutar la API para utilizar toda la aplicacion.

### 1. Ejecutar el backend

Ubicarse dentro de:

```bash
cd Backend/CriptoCarteraApi
```

Restaurar dependencias:

```bash
dotnet restore
```

Iniciar la aplicacion:

```bash
dotnet run
```

### 2. Acceder al sistema

La consola mostrara la URL donde se encuentra ejecutandose la aplicacion.

Por ejemplo:

```text
http://localhost:5147
```

o

```text
https://localhost:7001
```

Al abrir esa direccion en el navegador se cargara automaticamente tanto el frontend como el backend.

### Base de datos

La aplicacion utiliza `EnsureCreated()`, por lo que la base de datos SQLite se genera automaticamente la primera vez que se ejecuta el proyecto.

El archivo generado es:

```text
cartera_cripto.db
```

No es necesario ejecutar migraciones manualmente para probar la aplicacion.

---

## Desarrollo del frontend

Si se desea trabajar sobre el codigo fuente del frontend:

```bash
cd Frontend
npm install
npm run dev
```

Una vez realizados los cambios:

```bash
npm run build
```

Luego copiar el contenido generado en `dist/` hacia la carpeta `Backend/CriptoCarteraApi/wwwroot/`.

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

## Contexto academico

Este proyecto fue realizado como trabajo final individual para la materia **Programacion III** de la **Tecnicatura Universitaria en Programacion**.

El objetivo principal fue aplicar conceptos de desarrollo full stack utilizando Vue.js, ASP.NET Core, bases de datos SQL, APIs REST y consumo de servicios externos.
