import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import client from "../api/client";
import { useAuth } from "../context/AuthContext";

export default function Registro() {
  const [tipo, setTipo] = useState("Consumidor");
  const [form, setForm] = useState({
    nombreCompleto: "",
    correoElectronico: "",
    contrasena: "",
    telefono: "",
    nombreComercial: "",
    direccion: "",
    horarioAtencion: "",
    telefonoContacto: "",
    descripcion: "",
  });
  const [error, setError] = useState("");
  const { iniciarSesion } = useAuth();
  const navigate = useNavigate();

  const actualizar = (campo) => (e) => setForm({ ...form, [campo]: e.target.value });

  const enviar = async (e) => {
    e.preventDefault();
    setError("");
    try {
      const ruta = tipo === "Consumidor" ? "/auth/registro/consumidor" : "/auth/registro/establecimiento";
      const payload = tipo === "Consumidor"
        ? {
            nombreCompleto: form.nombreCompleto,
            correoElectronico: form.correoElectronico,
            contrasena: form.contrasena,
            telefono: form.telefono,
          }
        : form;

      const { data } = await client.post(ruta, payload);
      iniciarSesion(data);
      navigate(tipo === "Consumidor" ? "/" : "/panel");
    } catch (err) {
      setError(err.response?.data ?? "No se pudo completar el registro.");
    }
  };

  return (
    <div className="form-container">
      <h2>Crear cuenta</h2>

      <div className="tipo-selector">
        <button type="button" className={tipo === "Consumidor" ? "activo" : ""} onClick={() => setTipo("Consumidor")}>
          Soy consumidor
        </button>
        <button type="button" className={tipo === "Establecimiento" ? "activo" : ""} onClick={() => setTipo("Establecimiento")}>
          Soy establecimiento
        </button>
      </div>

      <form onSubmit={enviar}>
        <label>Nombre completo</label>
        <input value={form.nombreCompleto} onChange={actualizar("nombreCompleto")} required />

        <label>Correo electrónico</label>
        <input type="email" value={form.correoElectronico} onChange={actualizar("correoElectronico")} required />

        <label>Contraseña</label>
        <input type="password" value={form.contrasena} onChange={actualizar("contrasena")} required minLength={6} />

        <label>Teléfono</label>
        <input value={form.telefono} onChange={actualizar("telefono")} />

        {tipo === "Establecimiento" && (
          <>
            <label>Nombre comercial</label>
            <input value={form.nombreComercial} onChange={actualizar("nombreComercial")} required />

            <label>Dirección</label>
            <input value={form.direccion} onChange={actualizar("direccion")} required />

            <label>Horario de atención</label>
            <input value={form.horarioAtencion} onChange={actualizar("horarioAtencion")} placeholder="Ej. 8am - 6pm" />

            <label>Teléfono de contacto</label>
            <input value={form.telefonoContacto} onChange={actualizar("telefonoContacto")} />

            <label>Descripción</label>
            <textarea value={form.descripcion} onChange={actualizar("descripcion")} />
          </>
        )}

        {error && <p className="error">{String(error)}</p>}

        <button type="submit">Crear cuenta</button>
      </form>
      <p>¿Ya tienes cuenta? <Link to="/login">Inicia sesión</Link></p>
    </div>
  );
}
