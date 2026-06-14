const { createApp } = Vue;

createApp({
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
}).mount('#app');