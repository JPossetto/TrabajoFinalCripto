<template>
  <div id="aapp">
    <!-- cambio el id del div para evitar conflicto con el nombre del componente de vue -->
    <aside class="sidebar">
      <div class="brand">
        <div class="logo-icon"></div>
        <h1>CryptoWallet</h1>
      </div>
      <nav class="menu">
        <button :class="{activo: pantalla === 'cliente'}" @click="pantalla = 'cliente'">
          Nuevo Cliente
        </button>
        <button :class="{activo: pantalla === 'compra'}" @click="abrirMovimiento('compra')">
          Nueva Compra
        </button>
        <button :class="{activo: pantalla === 'venta'}" @click="abrirMovimiento('venta')">
          Nueva Venta
        </button>
        <button :class="{activo: pantalla === 'historial'}" @click="abrirHistorial">
          Historial de Movimientos
        </button>
      </nav>
    </aside>

    <main class="content">
      <section class="panel-card" v-if="pantalla === 'cliente'">
        <div class="panel-header">
          <h2>Registrar nuevo usuario</h2>
          <p class="texto-suave">Ingrese los datos correspondientes para dar de alta al cliente en la plataforma.</p>
        </div>

        <form @submit.prevent="guardarCliente" class="formulario">
          <div class="form-group">
            <label for="client-name">Nombre y apellido</label>
            <input id="client-name" v-model="cliente.name" type="text" placeholder="Ej: Pedro González" required>
          </div>

          <div class="form-group">
            <label for="client-email">Correo electrónico</label>
            <input id="client-email" v-model="cliente.email" type="email" placeholder="nombre@ejemplo.com" required>
          </div>

          <button class="btn btn-primary" type="submit" :disabled="cargando">
            {{ cargando ? 'Guardando...' : 'Registrar Cliente' }}
          </button>
        </form>
      </section>

      <section class="panel-card" v-if="pantalla === 'compra' || pantalla === 'venta'">
        <div class="panel-header">
          <h2>{{ pantalla === 'compra' ? 'Registrar Compra de Activos' : 'Registrar Venta de Activos' }}</h2>
          <p class="texto-suave">Los montos en ARS se cotizan en tiempo real mediante la API de CriptoYa.</p>
        </div>

        <form @submit.prevent="guardarTransaccion" class="formulario">
          <div class="form-group">
            <label>Cliente titular</label>
            <select v-model.number="transaccion.client_id" required>
              <option value="0" disabled>Seleccione un cliente de la lista</option>
              <option v-for="cliente in clientes" :key="cliente.id" :value="cliente.id">
                {{ cliente.name }}
              </option>
            </select>
          </div>

          <div class="form-group">
            <label>Activo digital (Criptomoneda)</label>
            <div class="crypto-selector-dynamic">
              <button type="button"
                      :class="['btn-crypto', 'btc', { seleccionado: transaccion.crypto_code === 'bitcoin' }]"
                      @click="transaccion.crypto_code = 'bitcoin'">
                <div class="crypto-icon">₿</div>
                <span class="crypto-name">Bitcoin</span>
              </button>
              <button type="button"
                      :class="['btn-crypto', 'usdc', { seleccionado: transaccion.crypto_code === 'usdc' }]"
                      @click="transaccion.crypto_code = 'usdc'">
                <div class="crypto-icon">$</div>
                <span class="crypto-name">USD Coin</span>
              </button>
              <button type="button"
                      :class="['btn-crypto', 'eth', { seleccionado: transaccion.crypto_code === 'ethereum' }]"
                      @click="transaccion.crypto_code = 'ethereum'">
                <div class="crypto-icon">Ξ</div>
                <span class="crypto-name">Ethereum</span>
              </button>
            </div>
          </div>

          <div class="form-group">
            <label>Cantidad de unidades</label>
            <input v-model.number="transaccion.crypto_amount" type="number" min="0.00000001" step="0.00000001" placeholder="0.00000000" required>
          </div>

          <div class="form-group">
            <label>Fecha y hora de operación</label>
            <input v-model="transaccion.datetime" type="datetime-local" required>
          </div>

          <button class="btn btn-primary" type="submit" :disabled="cargando">
            {{ cargando ? 'Procesando...' : (pantalla === 'compra' ? 'Ejecutar Compra' : 'Ejecutar Venta') }}
          </button>
        </form>
      </section>

      <section class="panel-card" v-if="pantalla === 'historial'">
        <div class="panel-header flex-header">
          <div>
            <h2>Historial de transacciones</h2>
            <p class="texto-suave">Listado auditable de operaciones registradas.</p>
          </div>
          <div class="filtro-box">
            <select v-model.number="clienteHistorial">
              <option value="0">Todos los clientes</option>
              <option v-for="cliente in clientes" :key="cliente.id" :value="cliente.id">
                {{ cliente.name }}
              </option>
            </select>
            <button class="btn btn-secondary" @click="cargarTransacciones">Filtrar</button>
          </div>
        </div>

        <div class="tabla-contenedor">
          <table class="tabla-custom">
            <thead>
              <tr>
                <th>Cliente</th>
                <th>Operación</th>
                <th>Activo</th>
                <th class="text-right">Cantidad</th>
                <th class="text-right">Monto Neto</th>
                <th>Fecha</th>
                <th class="text-center">Acciones</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="mov in transacciones" :key="mov.id">
                <td class="font-medium">{{ mov.client_name }}</td>
                <td>
                  <span :class="['badge', mov.action === 'purchase' ? 'badge-compra' : 'badge-venta']">
                    {{ mov.action === 'purchase' ? 'Compra' : 'Venta' }}
                  </span>
                </td>
                <td class="text-uppercase font-bold">{{ mov.crypto_code.substring(0,3) }}</td>
                <td class="text-right font-mono">{{ mov.crypto_amount }}</td>
                <td class="text-right font-mono font-bold text-success">$ {{ formatoDinero(mov.money) }}</td>
                <td class="text-muted">{{ formatoFecha(mov.datetime) }}</td>
                <td>
                  <div class="acciones-btn">
                    <button class="btn-action" @click="verTransaccion(mov)">Ver</button>
                    <button class="btn-action" @click="editarTransaccion(mov)">Editar</button>
                    <button class="btn-action btn-delete" @click="borrarTransaccion(mov)">Borrar</button>
                  </div>
                </td>
              </tr>
              <tr v-if="transacciones.length === 0">
                <td colspan="7" class="tabla-vacia">
                  No se encontraron movimientos registrados para este criterio.
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>
    </main>

    <div class="modal-overlay" v-if="modal.visible" @click.self="cerrarModal">
      <div class="modal-card">
        <div class="modal-header">
          <h3>{{ modal.modo === 'ver' ? 'Detalle de la Operación' : 'Modificar Registro Histórico' }}</h3>
          <button class="close-x" @click="cerrarModal">&times;</button>
        </div>

        <div v-if="modal.modo === 'ver'" class="modal-body detalle-lista">
          <div class="detalle-item"><span>ID Transacción:</span> <strong>#{{ modal.transaccion.id }}</strong></div>
          <div class="detalle-item"><span>Cliente:</span> <strong>{{ modal.transaccion.client_name }}</strong></div>
          <div class="detalle-item">
            <span>Tipo:</span>
            <span :class="['badge', modal.transaccion.action === 'purchase' ? 'badge-compra' : 'badge-venta']">
              {{ modal.transaccion.action === 'purchase' ? 'Compra' : 'Venta' }}
            </span>
          </div>
          <div class="detalle-item"><span>Criptoactivo:</span> <strong class="text-uppercase">{{ modal.transaccion.crypto_code }}</strong></div>
          <div class="detalle-item"><span>Cantidad:</span> <strong class="font-mono">{{ modal.transaccion.crypto_amount }}</strong></div>
          <div class="detalle-item"><span>Evaluación:</span> <strong class="font-mono text-success">$ {{ formatoDinero(modal.transaccion.money) }}</strong></div>
          <div class="detalle-item"><span>Fecha Registro:</span> <strong class="text-muted">{{ formatoFecha(modal.transaccion.datetime) }}</strong></div>
        </div>

        <form v-else @submit.prevent="guardarEdicion" class="modal-body formulario">
          <div class="form-group">
            <label>Titular</label>
            <select v-model.number="modal.transaccion.client_id">
              <option v-for="cliente in clientes" :key="cliente.id" :value="cliente.id">
                {{ cliente.name }}
              </option>
            </select>
          </div>

          <div class="form-group">
            <label>Tipo de movimiento</label>
            <select v-model="modal.transaccion.action">
              <option value="purchase">Compra</option>
              <option value="sale">Venta</option>
            </select>
          </div>

          <div class="form-group">
            <label>Criptoactivo</label>
            <div class="crypto-selector-dynamic">
              <button type="button"
                      :class="['btn-crypto', 'btc', { seleccionado: modal.transaccion.crypto_code === 'bitcoin' }]"
                      @click="modal.transaccion.crypto_code = 'bitcoin'">
                <div class="crypto-icon">₿</div>
                <span class="crypto-name">Bitcoin</span>
              </button>
              <button type="button"
                      :class="['btn-crypto', 'usdc', { seleccionado: modal.transaccion.crypto_code === 'usdc' }]"
                      @click="modal.transaccion.crypto_code = 'usdc'">
                <div class="crypto-icon">$</div>
                <span class="crypto-name">USDC</span>
              </button>
              <button type="button"
                      :class="['btn-crypto', 'eth', { seleccionado: modal.transaccion.crypto_code === 'ethereum' }]"
                      @click="modal.transaccion.crypto_code = 'ethereum'">
                <div class="crypto-icon">Ξ</div>
                <span class="crypto-name">Ethereum</span>
              </button>
            </div>
          </div>

          <div class="form-group">
            <label>Cantidad</label>
            <input v-model.number="modal.transaccion.crypto_amount" type="number" step="0.00000001" required>
          </div>

          <div class="form-group">
            <label>Importe Fijado (ARS)</label>
            <input v-model.number="modal.transaccion.money" type="number" step="0.01" required>
          </div>

          <div class="form-group">
            <label>Fecha Modificación</label>
            <input v-model="modal.transaccion.datetimeEditada" type="datetime-local" required>
          </div>

          <button class="btn btn-primary w-full" type="submit" :disabled="cargando">
            {{ cargando ? 'Actualizando...' : 'Confirmar Cambios' }}
          </button>
        </form>

        <div class="modal-footer" v-if="modal.modo === 'ver'">
          <button class="btn btn-secondary" @click="cerrarModal">Cerrar Ventana</button>
        </div>
      </div>
    </div>

    <Transition name="toast-fade">
      <div class="toast-notification" v-if="mensaje">{{ mensaje }}</div>
    </Transition>
  </div>
</template>

<script>
  export default {
    name: 'App',
    data() {
      return {
        apiUrl: 'http://localhost:5147',
        pantalla: 'cliente',
        mensaje: '',
        cargando: false,
        clientes: [],
        transacciones: [],
        clienteHistorial: 0,
        cliente: {
          name: '',
          email: ''
        },
        transaccion: {
          crypto_code: 'bitcoin',
          action: 'purchase',
          client_id: 0,
          crypto_amount: null,
          datetime: ''
        },
        modal: {
          visible: false,
          modo: 'ver',
          transaccion: {}
        }
      };
    },
    mounted() {
      this.transaccion.datetime = this.fechaParaInput(new Date());
      this.cargarClientes();
    },
    methods: {
      async cargarClientes() {
        try {
          const res = await fetch(`${this.apiUrl}/client`);
          if (!res.ok) throw new Error();
          this.clientes = await res.json();
        } catch {
          this.mostrarMensaje('Error de enlace: Compruebe que el servicio API local esté activo.');
        }
      },
      async guardarCliente() {
        if (!this.cliente.name.trim()) {
          this.mostrarMensaje('Operación cancelada: El campo de nombre es obligatorio.');
          return;
        }

        const emailRegex = /^[^@\s]+@[^@\s]+\.[^@\s]+$/;
        if (!emailRegex.test(this.cliente.email)) {
          this.mostrarMensaje('Estructura inválida: Introduzca un correo electrónico verificable.');
          return;
        }

        this.cargando = true;
        try {
          const res = await fetch(`${this.apiUrl}/client`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(this.cliente)
          });

          if (!res.ok) {
            const error = await res.json();
            throw new Error(error.mensaje || 'Error interno en el servidor.');
          }

          this.cliente.name = '';
          this.cliente.email = '';
          await this.cargarClientes();
          this.mostrarMensaje('Alta exitosa: Cliente registrado de manera correcta.');
        } catch (err) {
          this.mostrarMensaje(err.message || 'No se pudo completar el registro.');
        } finally {
          this.cargando = false;
        }
      },
      abrirMovimiento(tipo) {
        this.pantalla = tipo;
        this.transaccion.action = tipo === 'compra' ? 'purchase' : 'sale';
        this.transaccion.client_id = 0;
        this.transaccion.crypto_amount = null;
        this.transaccion.datetime = this.fechaParaInput(new Date());
        this.cargarClientes();
      },
      async guardarTransaccion() {
        if (this.transaccion.client_id === 0) {
          this.mostrarMensaje('Formulario incompleto: Asigne un titular a la transacción.');
          return;
        }

        if (!this.transaccion.crypto_amount || this.transaccion.crypto_amount <= 0) {
          this.mostrarMensaje('Monto no válido: El volumen operativo debe superar el cero.');
          return;
        }

        this.cargando = true;
        const body = {
          crypto_code: this.transaccion.crypto_code,
          action: this.transaccion.action,
          client_id: this.transaccion.client_id,
          crypto_amount: this.transaccion.crypto_amount,
          datetime: new Date(this.transaccion.datetime).toISOString()
        };

        try {
          const res = await fetch(`${this.apiUrl}/transactions`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body)
          });

          if (!res.ok) {
            const error = await res.json();
            throw new Error(error.mensaje || 'Error al procesar la cotización en backend.');
          }

          this.transaccion.crypto_amount = null;
          this.transaccion.datetime = this.fechaParaInput(new Date());
          this.mostrarMensaje('Transacción confirmada: Cotización calculada exitosamente.');
        } catch (err) {
          this.mostrarMensaje(err.message || 'Hubo un problema al procesar la transacción.');
        } finally {
          this.cargando = false;
        }
      },
      async abrirHistorial() {
        this.pantalla = 'historial';
        await this.cargarClientes();
        await this.cargarTransacciones();
      },
      async cargarTransacciones() {
        let url = `${this.apiUrl}/transactions`;
        if (this.clienteHistorial !== 0) {
          url += `?clientId=${this.clienteHistorial}`;
        }

        try {
          const res = await fetch(url);
          if (!res.ok) throw new Error();
          this.transacciones = await res.json();
        } catch {
          this.mostrarMensaje('Error de consulta: No se pudieron sincronizar las transacciones.');
        }
      },
      verTransaccion(mov) {
        this.modal.visible = true;
        this.modal.modo = 'ver';
        this.modal.transaccion = { ...mov };
      },
      editarTransaccion(mov) {
        this.modal.visible = true;
        this.modal.modo = 'editar';
        this.modal.transaccion = {
          ...mov,
          datetimeEditada: this.fechaParaInput(new Date(mov.datetime))
        };
      },
      async guardarEdicion() {
        this.cargando = true;
        const mov = this.modal.transaccion;

        const body = {
          crypto_code: mov.crypto_code,
          action: mov.action,
          client_id: mov.client_id,
          crypto_amount: mov.crypto_amount,
          money: mov.money,
          datetime: new Date(mov.datetimeEditada).toISOString()
        };

        try {
          const res = await fetch(`${this.apiUrl}/transactions/${mov.id}`, {
            method: 'PATCH',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body)
          });

          if (!res.ok) throw new Error();

          this.cerrarModal();
          await this.cargarTransacciones();
          this.mostrarMensaje('Registro actualizado con éxito.');
        } catch {
          this.mostrarMensaje('Error de escritura: Los cambios no fueron consolidados.');
        } finally {
          this.cargando = false;
        }
      },
      async borrarTransaccion(mov) {
        const seguro = confirm(`¿Desea eliminar la transacción permanentemente?\nID: ${mov.id}`);
        if (!seguro) return;

        try {
          const res = await fetch(`${this.apiUrl}/transactions/${mov.id}`, {
            method: 'DELETE'
          });

          if (!res.ok) throw new Error();

          await this.cargarTransacciones();
          this.mostrarMensaje('El registro ha sido removido de la base de datos.');
        } catch {
          this.mostrarMensaje('Error de remoción: No se pudo eliminar el registro.');
        }
      },
      cerrarModal() {
        this.modal.visible = false;
        this.modal.transaccion = {};
      },
      formatoDinero(valor) {
        return Number(valor).toLocaleString('es-AR', {
          minimumFractionDigits: 2,
          maximumFractionDigits: 2
        });
      },
      formatoFecha(valor) {
        return new Date(valor).toLocaleString('es-AR', {
          year: 'numeric', month: '2-digit', day: '2-digit',
          hour: '2-digit', minute: '2-digit'
        });
      },
      fechaParaInput(fecha) {
        const local = new Date(fecha.getTime() - fecha.getTimezoneOffset() * 60000);
        return local.toISOString().slice(0, 16);
      },
      mostrarMensaje(texto) {
        this.mensaje = texto;
        setTimeout(() => {
          if (this.mensaje === texto) this.mensaje = '';
        }, 4000);
      }
    }
  }
</script>

<style>
  :root {
    --bg-main: #0b0f19;
    --bg-sidebar: #111827;
    --bg-card: #1f2937;
    --bg-input: #111827;
    --text-main: #f3f4f6;
    --text-muted: #9ca3af;
    --primary: #2563eb;
    --primary-hover: #1d4ed8;
    --success: #10b981;
    --success-bg: rgba(16, 185, 129, 0.15);
    --danger: #ef4444;
    --danger-bg: rgba(239, 68, 68, 0.15);
    --border-color: #374151;
    --radius: 8px;
    font-family: Inter, system-ui, -apple-system, sans-serif;
    color: var(--text-main);
    background-color: var(--bg-main);
  }

  * {
    box-sizing: border-box;
  }

  body {
    margin: 0;
    background-color: var(--bg-main);
  }

  html, body {
    margin: 0;
    padding: 0;
    width: 100%;
    min-height: 100vh;
    background-color: var(--bg-main);
  }
  /*cambio el selector por la clase del div*/
  #aapp {
    min-height: 100vh;
    width: 100%; 
    display: grid;
    grid-template-columns: 280px 1fr;
  }

  .sidebar {
    background: var(--bg-sidebar);
    color: var(--text-main);
    padding: 30px 20px;
    display: flex;
    flex-direction: column;
    gap: 35px;
    border-right: 1px solid var(--border-color);
  }

  .brand {
    display: flex;
    align-items: center;
    gap: 12px;
  }

    .brand h1 {
      font-size: 20px;
      font-weight: 700;
      margin: 0;
      letter-spacing: -0.5px;
    }

  .logo-icon {
    width: 24px;
    height: 24px;
    background: var(--primary);
    border-radius: 4px;
    box-shadow: 0 0 10px var(--primary);
  }

  .menu {
    display: flex;
    flex-direction: column;
    gap: 8px;
  }

    .menu button {
      width: 100%;
      border: 0;
      padding: 14px 16px;
      border-radius: var(--radius);
      background: transparent;
      color: var(--text-muted);
      text-align: left;
      font-size: 14px;
      font-weight: 500;
      cursor: pointer;
      transition: all 0.2s ease;
    }

      .menu button:hover {
        background: #1f2937;
        color: #fff;
      }

      .menu button.activo {
        background: var(--primary);
        color: white;
        font-weight: 600;
      }

  .content {
    padding: 40px;
  }

  .panel-card {
    background: var(--bg-card);
    border: 1px solid var(--border-color);
    border-radius: var(--radius);
    padding: 32px;
    max-width: 1200px;
  }

  .panel-header {
    margin-bottom: 28px;
  }

    .panel-header h2 {
      font-size: 24px;
      margin: 0;
      font-weight: 700;
      letter-spacing: -0.5px;
    }

  .texto-suave {
    color: var(--text-muted);
    margin: 6px 0 0 0;
    font-size: 15px;
  }

  .formulario {
    display: grid;
    gap: 20px;
    max-width: 550px;
  }

  .form-group {
    display: flex;
    flex-direction: column;
    gap: 8px;
  }

    .form-group label {
      font-size: 14px;
      font-weight: 600;
      color: var(--text-muted);
    }

  input, select {
    min-height: 46px;
    border: 1px solid var(--border-color);
    border-radius: var(--radius);
    padding: 10px 14px;
    font-size: 15px;
    background: var(--bg-input);
    color: var(--text-main);
    outline: none;
    transition: border-color 0.2s;
  }

    input:focus, select:focus {
      border-color: var(--primary);
    }

  .btn {
    border-radius: var(--radius);
    font-size: 15px;
    font-weight: 600;
    padding: 12px 24px;
    cursor: pointer;
    transition: all 0.2s;
    border: 1px solid transparent;
  }

    .btn:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

  .btn-primary {
    background: var(--primary);
    color: white;
  }

    .btn-primary:hover:not(:disabled) {
      background: var(--primary-hover);
    }

  .btn-secondary {
    background: var(--bg-input);
    border-color: var(--border-color);
    color: var(--text-main);
  }

    .btn-secondary:hover {
      background: var(--bg-card);
    }

  .w-full {
    width: 100%;
  }

  .flex-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: 20px;
  }

  .filtro-box {
    display: flex;
    gap: 10px;
  }

  .tabla-contenedor {
    border: 1px solid var(--border-color);
    border-radius: var(--radius);
    overflow: hidden;
    background: var(--bg-card);
  }

  .tabla-custom {
    width: 100%;
    border-collapse: collapse;
    text-align: left;
    font-size: 14px;
  }

    .tabla-custom th {
      background: #111827;
      padding: 16px;
      font-weight: 600;
      color: var(--text-muted);
      border-bottom: 1px solid var(--border-color);
    }

    .tabla-custom td {
      padding: 16px;
      border-bottom: 1px solid var(--border-color);
      vertical-align: middle;
    }

    .tabla-custom tr:last-child td {
      border: 0;
    }

  .badge {
    padding: 4px 10px;
    border-radius: 12px;
    font-size: 12px;
    font-weight: 600;
    display: inline-block;
  }

  .badge-compra {
    background: var(--success-bg);
    color: var(--success);
  }

  .badge-venta {
    background: var(--danger-bg);
    color: var(--danger);
  }

  .text-right {
    text-align: right;
  }

  .text-center {
    text-align: center;
  }

  .text-uppercase {
    text-transform: uppercase;
  }

  .font-mono {
    font-family: ui-monospace, monospace;
  }

  .font-bold {
    font-weight: 700;
  }

  .font-medium {
    font-weight: 500;
  }

  .text-success {
    color: var(--success);
  }

  .acciones-btn {
    display: flex;
    gap: 6px;
  }

  .btn-action {
    background: var(--bg-input);
    border: 1px solid var(--border-color);
    color: var(--text-main);
    border-radius: 4px;
    padding: 6px 12px;
    cursor: pointer;
    font-size: 13px;
    transition: all 0.2s;
  }

    .btn-action:hover {
      background: var(--border-color);
    }

  .btn-delete {
    color: var(--danger);
    border-color: rgba(239, 68, 68, 0.4);
  }

    .btn-delete:hover {
      background: var(--danger-bg);
    }

  .tabla-vacia {
    text-align: center;
    color: var(--text-muted);
    padding: 40px !important;
  }

  .modal-overlay {
    position: fixed;
    inset: 0;
    background: rgba(0, 0, 0, 0.7);
    backdrop-filter: blur(4px);
    display: grid;
    place-items: center;
    z-index: 100;
    padding: 20px;
  }

  .modal-card {
    background: var(--bg-card);
    border: 1px solid var(--border-color);
    width: min(540px, 100%);
    border-radius: var(--radius);
    overflow: hidden;
    display: flex;
    flex-direction: column;
    max-height: 90vh;
  }

  .modal-header {
    padding: 20px 24px;
    border-bottom: 1px solid var(--border-color);
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

    .modal-header h3 {
      margin: 0;
      font-size: 18px;
      font-weight: 700;
    }

  .close-x {
    background: transparent;
    border: 0;
    font-size: 24px;
    cursor: pointer;
    color: var(--text-muted);
  }

  .modal-body {
    padding: 24px;
    overflow-y: auto;
  }

  .modal-footer {
    padding: 16px 24px;
    background: #111827;
    border-top: 1px solid var(--border-color);
    display: flex;
    justify-content: flex-end;
  }

  .detalle-lista {
    display: flex;
    flex-direction: column;
    gap: 14px;
  }

  .detalle-item {
    display: flex;
    justify-content: space-between;
    border-bottom: 1px solid var(--border-color);
    padding-bottom: 10px;
    font-size: 15px;
  }

    .detalle-item span {
      color: var(--text-muted);
    }

  .toast-notification {
    position: fixed;
    right: 24px;
    bottom: 24px;
    background: var(--primary);
    color: white;
    padding: 14px 24px;
    border-radius: var(--radius);
    box-shadow: 0 10px 15px -3px rgba(0,0,0,0.5);
    font-size: 14px;
    font-weight: 500;
    z-index: 200;
  }

  .toast-fade-enter-active, .toast-fade-leave-active {
    transition: all 0.3s ease;
  }

  .toast-fade-enter-from, .toast-fade-leave-to {
    opacity: 0;
    transform: translateY(10px);
  }

  /* Selector Dinámico de Monedas */
  .crypto-selector-dynamic {
    display: flex;
    gap: 16px;
    align-items: center;
    width: 100%;
    padding: 10px 0;
  }

  .btn-crypto {
    display: flex;
    align-items: center;
    width: 50px;
    height: 50px;
    padding: 0 13px;
    border: 0;
    border-radius: 50px;
    background: var(--bg-main);
    color: var(--text-muted);
    cursor: pointer;
    overflow: hidden;
    transition: all 0.5s cubic-bezier(0.4, 0, 0.2, 1);
    box-shadow: 0 4px 10px rgba(0, 0, 0, 0.3);
  }

  .crypto-icon {
    font-size: 20px;
    font-weight: 700;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
    width: 24px;
    height: 24px;
    transition: color 0.5s ease;
  }

  .crypto-name {
    font-size: 15px;
    font-weight: 600;
    margin-left: 12px;
    white-space: nowrap;
    opacity: 0;
    transform: translateX(10px);
    transition: opacity 0.3s ease, transform 0.5s cubic-bezier(0.4, 0, 0.2, 1);
  }

  .btn-crypto:hover:not(.seleccionado) {
    transform: translateY(-2px);
    box-shadow: 0 6px 14px rgba(0, 0, 0, 0.4);
    color: var(--text-main);
  }

  .btn-crypto.seleccionado {
    width: 150px;
    color: #ffffff;
  }

    .btn-crypto.seleccionado .crypto-name {
      opacity: 1;
      transform: translateX(0);
    }

  .btn-crypto.btc.seleccionado {
    background-color: #f59e0b;
    box-shadow: 0 0 15px #f59e0b, 0 0 30px #f59e0b;
  }

    .btn-crypto.btc.seleccionado .crypto-icon {
      color: #ffffff;
    }

  .btn-crypto.usdc.seleccionado {
    background-color: #2563eb;
    box-shadow: 0 0 15px #2563eb, 0 0 30px #2563eb;
  }

    .btn-crypto.usdc.seleccionado .crypto-icon {
      color: #ffffff;
    }

  .btn-crypto.eth.seleccionado {
    background-color: #7c3aed;
    box-shadow: 0 0 15px #7c3aed, 0 0 30px #7c3aed;
  }

    .btn-crypto.eth.seleccionado .crypto-icon {
      color: #ffffff;
    }

  @media (max-width: 850px) {
    #app {
      grid-template-columns: 1fr;
    }

    .sidebar {
      border-right: 0;
      border-bottom: 1px solid var(--border-color);
    }

    .content {
      padding: 20px;
    }

    .flex-header {
      display: flex;
      flex-direction: column;
      align-items: stretch;
    }

    .filtro-box {
      width: 100%;
    }

      .filtro-box select {
        flex-grow: 1;
      }
  }
</style>
