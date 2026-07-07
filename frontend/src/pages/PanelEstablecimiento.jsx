import { useEffect, useState } from "react";
import client from "../api/client";

const ofertaVacia = {
  titulo: "",
  descripcion: "",
  categoria: "",
  cantidad: 1,
  precioSugerido: "",
  fotoUrl: "",
  disponibleHasta: "",
};

export default function PanelEstablecimiento() {
  const [ofertas, setOfertas] = useState([]);
  const [reservas, setReservas] = useState([]);
  const [form, setForm] = useState(ofertaVacia);
  const [error, setError] = useState("");
  const [mensaje, setMensaje] = useState("");

  const cargarOfertas = async () => {
    const { data } = await client.get("/ofertas/mias");
    setOfertas(data);
  };

  const cargarReservas = async () => {
    const { data } = await client.get("/reservas/establecimiento");
    setReservas(data);
  };

  useEffect(() => {
    cargarOfertas();
    cargarReservas();
  }, []);

  const actualizar = (campo) => (e) => setForm({ ...form, [campo]: e.target.value });

  const publicarOferta = async (e) => {
    e.preventDefault();
    setError("");
    setMensaje("");
    try {
      await client.post("/ofertas", {
        ...form,
        cantidad: Number(form.cantidad),
        precioSugerido: Number(form.precioSugerido),
        disponibleHasta: new Date(form.disponibleHasta).toISOString(),
      });
      setMensaje("Oferta publicada exitosamente.");
      setForm(ofertaVacia);
      cargarOfertas();
    } catch (err) {
      setError(err.response?.data ?? "No se pudo publicar la oferta.");
    }
  };

  const confirmarRetiro = async (reservaId) => {
    await client.post(`/reservas/${reservaId}/confirmar-establecimiento`);
    cargarReservas();
    cargarOfertas();
  };

  return (
    <div className="page">
      <h2>Panel del establecimiento</h2>

      <section>
        <h3>Publicar excedente alimenticio</h3>
        <form className="form-inline" onSubmit={publicarOferta}>
          <label>Título</label>
          <input value={form.titulo} onChange={actualizar("titulo")} required />

          <label>Descripción</label>
          <textarea value={form.descripcion} onChange={actualizar("descripcion")} />

          <label>Categoría</label>
          <input value={form.categoria} onChange={actualizar("categoria")} required />

          <label>Cantidad</label>
          <input type="number" min="1" value={form.cantidad} onChange={actualizar("cantidad")} required />

          <label>Precio sugerido (RD$)</label>
          <input type="number" min="0" step="0.01" value={form.precioSugerido} onChange={actualizar("precioSugerido")} required />

          <label>Foto (URL, opcional)</label>
          <input value={form.fotoUrl} onChange={actualizar("fotoUrl")} />

          <label>Disponible hasta</label>
          <input type="datetime-local" value={form.disponibleHasta} onChange={actualizar("disponibleHasta")} required />

          {error && <p className="error">{String(error)}</p>}
          {mensaje && <p className="exito">{mensaje}</p>}

          <button type="submit">Publicar oferta</button>
        </form>
      </section>

      <section>
        <h3>Mis ofertas publicadas</h3>
        <div className="tarjetas">
          {ofertas.map((o) => (
            <div className="tarjeta" key={o.ofertaId}>
              <h3>{o.titulo}</h3>
              <p>{o.descripcion}</p>
              <p className="precio">RD$ {o.precioSugerido}</p>
              <p><strong>Estado:</strong> {o.estado}</p>
              <p><strong>Disponible hasta:</strong> {new Date(o.disponibleHasta).toLocaleString()}</p>
            </div>
          ))}
        </div>
      </section>

      <section>
        <h3>Reservas recibidas</h3>
        <table className="tabla">
          <thead>
            <tr>
              <th>Oferta</th>
              <th>Consumidor</th>
              <th>Fecha reserva</th>
              <th>Estado</th>
              <th>Confirmación</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {reservas.map((r) => (
              <tr key={r.reservaId}>
                <td>{r.ofertaTitulo}</td>
                <td>{r.consumidorNombre}</td>
                <td>{new Date(r.fechaReserva).toLocaleString()}</td>
                <td>{r.estado}</td>
                <td>
                  {r.confirmadoPorConsumidor ? "✓ Consumidor" : "Pendiente consumidor"} / {r.confirmadoPorEstablecimiento ? "✓ Tú" : "Pendiente tú"}
                </td>
                <td>
                  {r.estado === "Pendiente" && !r.confirmadoPorEstablecimiento && (
                    <button onClick={() => confirmarRetiro(r.reservaId)}>Confirmar retiro</button>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </section>
    </div>
  );
}
