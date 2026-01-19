# GestionNegocio
sismeta de gestion de negocios terminada version 1.0.2

📦 Sistema de Gestión de Negocio y Punto de Venta
Descripción general

Esta aplicación es un sistema de gestión de negocio y punto de venta (POS) orientado a controlar ventas, compras, stock, cuentas corrientes y movimientos de caja, manteniendo trazabilidad histórica de precios y operaciones.

El foco principal del sistema es registrar correctamente el estado del negocio en el tiempo, permitiendo auditoría, control financiero y seguimiento de clientes y proveedores.

🧱 Módulos principales
👥 Clientes

Registro y gestión de clientes.

Asociación de ventas y cuentas corrientes.

Seguimiento de saldos, pagos y deudas.

🏭 Proveedores

Registro y gestión de proveedores.

Asociación de órdenes de pedido y compras.

Integración con stock y cuentas corrientes.

🧾 Órdenes de Venta (Órdenes de Compra)

Representan las ventas realizadas a clientes.

Cada orden queda asociada a:

Cliente

Fecha

Turno de caja

Productos vendidos

Precios históricos del momento de la venta

Impactan en:

Stock (egreso)

Caja

Cuenta corriente del cliente (si aplica)

📦 Órdenes de Pedido a Proveedor

Representan pedidos de mercadería a proveedores.

Funcionan en dos etapas:

Pedido abierto: no impacta stock.

Pedido cerrado: al cerrarse, suma stock automáticamente.

Asociadas a precios históricos del momento de la compra.

Impactan en:

Stock (ingreso)

Cuenta corriente del proveedor

Caja (si se registra pago)

🏷️ Product Prices (Histórico de precios)

Entidad clave del sistema.

Registra el precio de un producto en un momento específico del tiempo.

Guarda:

Precio de lista

Precio de venta

Fecha

Producto

Orden asociada (venta o pedido a proveedor)

Esto permite:

Mantener coherencia histórica (los precios no cambian retroactivamente).

Analizar márgenes, variaciones de precio y rentabilidad.

Auditar ventas y compras pasadas sin depender del precio actual del producto.

📒 Sistema de Cuentas Corrientes

Manejo de cuentas corrientes para:

Clientes

Proveedores

Registro de:

Débitos (ventas, compras)

Créditos (pagos, cobros)

Cálculo automático de saldo.

Asociación directa con órdenes y movimientos de caja.

💰 Caja y Movimientos de Dinero

Registro de ingresos y egresos de dinero.

Cada movimiento incluye:

Tipo (ingreso / egreso)

Monto

Fecha

Concepto

Turno de caja asociado

Permite:

Control diario de caja

Cierre por turno

Seguimiento financiero por fecha

🕒 Turnos de Caja

Agrupan los movimientos de dinero por período operativo.

Cada turno puede incluir:

Ventas

Cobros

Pagos

Ingresos y egresos manuales

Facilita el control y cierre de caja por jornada o responsable.

🎯 Objetivos del sistema

Control integral del negocio.

Trazabilidad completa de precios y operaciones.

Separación clara entre:

Operaciones comerciales

Impacto en stock

Impacto financiero

Escalabilidad para distintos tipos de negocios.

🧠 Filosofía de diseño

Los precios nunca se modifican retroactivamente.

El stock solo cambia por eventos explícitos (ventas cerradas, pedidos cerrados).

La caja refleja únicamente movimientos reales de dinero.

Todo evento importante queda asociado a una fecha y a un turno.
