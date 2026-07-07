import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import client from "../api/client";
import { useAuth } from "../context/AuthContext";

export default function OfertaDetalle() {
  const { id } = useParams();
  const [oferta, setOferta] = useState(null);
  const [mensaje, setMensaje] = useState("");
  const [error, setError] = useState("");
  const { usuario } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    client.get(`/ofertas/${id}`).then(({ data }) => setOferta(data));
  }, [id]);

  const reservar = async () => {
    setError("");
    setMensaje("");
    if (!usuario) {
      navigate("/login");
      return;
    }
    try {
      await client.post("/reservas", { ofertaId: Number(id) });
      setMensaje("¡Reserva creada! Ve a 'Mis reservas' para confirmar el retiro.");
      const { data } = await client.get(`/ofertas/${id}`);
      setOferta(data);
    } catch (err) {
      setError(err.response?.data ?? "No se pudo crear la reserva.");
    }
  };

  if (!oferta) return <p className="page">Cargando...</p>;

  return (
    <div className="page">
      <div className="detalle">
        <h2>{oferta.titulo}</h2>
        <p className="establecimiento">{oferta.nombreComercial}</p>
        <p>{oferta.descripcion}</p>
        <p><strong>Categoría:</strong> {oferta.categoria}</p>
        <p><strong>Cantidad:</strong> {oferta.cantidad}</p>
        <p><strong>Precio sugerido:</strong> RD$ {oferta.precioSugerido}</p>
        <p><strong>Disponible hasta:</strong> {new Date(oferta.disponibleHasta).toLocaleString()}</p>
        <p><strong>Estado:</strong> {oferta.estado}</p>

        {oferta.estado === "Disponible" ? (
          <button onClick={reservar}>Reservar este excedente</button>
        ) : (
          <p className="aviso">Esta oferta ya no está disponible para reservar.</p>
        )}

        {mensaje && <p className="exito">{mensaje}</p>}
        {error && <p className="error">{String(error)}</p>}
      </div>
    </div>
  );
}
