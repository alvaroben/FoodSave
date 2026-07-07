import { useEffect, useState } from "react";
import client from "../api/client";

export default function MisReservas() {
  const [reservas, setReservas] = useState([]);
  const [cargando, setCargando] = useState(true);

  const cargar = async () => {
    setCargando(true);
    const { data } = await client.get("/reservas/mias");
    setReservas(data);
    setCargando(false);
  };

  useEffect(() => {
    cargar();
  }, []);

  const confirmarRetiro = async (reservaId) => {
    await client.post(`/reservas/${reservaId}/confirmar-consumidor`);
    cargar();
  };

  const cancelar = async (reservaId) => {
    await client.post(`/reservas/${reservaId}/cancelar`);
    cargar();
  };

  if (cargando) return <p className="page">Cargando...</p>;

  return (
    <div className="page">
      <h2>Mis reservas</h2>
      {reservas.length === 0 ? (
        <p>Aún no tienes reservas.</p>
      ) : (
        <table className="tabla">
          <thead>
            <tr>
              <th>Oferta</th>
              <th>Establecimiento</th>
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
                <td>{r.nombreComercial}</td>
                <td>{new Date(r.fechaReserva).toLocaleString()}</td>
                <td>{r.estado}</td>
                <td>
                  {r.confirmadoPorConsumidor ? "✓ Tú" : "Pendiente tú"} / {r.confirmadoPorEstablecimiento ? "✓ Establecimiento" : "Pendiente establecimiento"}
                </td>
                <td>
                  {r.estado === "Pendiente" && (
                    <>
                      {!r.confirmadoPorConsumidor && (
                        <button onClick={() => confirmarRetiro(r.reservaId)}>Confirmar retiro</button>
                      )}
                      <button onClick={() => cancelar(r.reservaId)}>Cancelar</button>
                    </>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}
