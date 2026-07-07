import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import client from "../api/client";

function minutosRestantesTexto(disponibleHasta) {
  const ms = new Date(disponibleHasta).getTime() - Date.now();
  if (ms <= 0) return "Vencida";
  const minutos = Math.floor(ms / 60000);
  if (minutos < 60) return `${minutos} min restantes`;
  const horas = Math.floor(minutos / 60);
  return `${horas} h restantes`;
}

export default function Ofertas() {
  const [ofertas, setOfertas] = useState([]);
  const [filtros, setFiltros] = useState({ categoria: "", ubicacion: "", precioMax: "" });
  const [cargando, setCargando] = useState(true);

  const cargarOfertas = async (filtrosActuales = filtros) => {
    setCargando(true);
    const params = {};
    if (filtrosActuales.categoria) params.categoria = filtrosActuales.categoria;
    if (filtrosActuales.ubicacion) params.ubicacion = filtrosActuales.ubicacion;
    if (filtrosActuales.precioMax) params.precioMax = filtrosActuales.precioMax;

    const { data } = await client.get("/ofertas", { params });
    setOfertas(data);
    setCargando(false);
  };

  useEffect(() => {
    cargarOfertas();
  }, []);

  const buscar = (e) => {
    e.preventDefault();
    cargarOfertas();
  };

  return (
    <div className="page">
      <h2>Excedentes disponibles</h2>

      <form className="filtros" onSubmit={buscar}>
        <input
          placeholder="Categoría (ej. Panaderia)"
          value={filtros.categoria}
          onChange={(e) => setFiltros({ ...filtros, categoria: e.target.value })}
        />
        <input
          placeholder="Ubicación"
          value={filtros.ubicacion}
          onChange={(e) => setFiltros({ ...filtros, ubicacion: e.target.value })}
        />
        <input
          type="number"
          placeholder="Precio máximo"
          value={filtros.precioMax}
          onChange={(e) => setFiltros({ ...filtros, precioMax: e.target.value })}
        />
        <button type="submit">Filtrar</button>
      </form>

      {cargando ? (
        <p>Cargando...</p>
      ) : ofertas.length === 0 ? (
        <p>No hay ofertas disponibles con esos filtros.</p>
      ) : (
        <div className="tarjetas">
          {ofertas.map((o) => (
            <Link to={`/ofertas/${o.ofertaId}`} key={o.ofertaId} className="tarjeta">
              <h3>{o.titulo}</h3>
              <p className="establecimiento">{o.nombreComercial}</p>
              <p>{o.descripcion}</p>
              <p className="precio">RD$ {o.precioSugerido}</p>
              <p className="tiempo">{minutosRestantesTexto(o.disponibleHasta)}</p>
            </Link>
          ))}
        </div>
      )}
    </div>
  );
}
