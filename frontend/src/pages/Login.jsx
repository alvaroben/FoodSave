import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import client from "../api/client";
import { useAuth } from "../context/AuthContext";

export default function Login() {
  const [correoElectronico, setCorreo] = useState("");
  const [contrasena, setContrasena] = useState("");
  const [error, setError] = useState("");
  const { iniciarSesion } = useAuth();
  const navigate = useNavigate();

  const enviar = async (e) => {
    e.preventDefault();
    setError("");
    try {
      const { data } = await client.post("/auth/login", { correoElectronico, contrasena });
      iniciarSesion(data);
      navigate(data.rol === "Establecimiento" ? "/panel" : "/");
    } catch (err) {
      setError(err.response?.data ?? "No se pudo iniciar sesión.");
    }
  };

  return (
    <div className="form-container">
      <h2>Iniciar sesión</h2>
      <form onSubmit={enviar}>
        <label>Correo electrónico</label>
        <input type="email" value={correoElectronico} onChange={(e) => setCorreo(e.target.value)} required />

        <label>Contraseña</label>
        <input type="password" value={contrasena} onChange={(e) => setContrasena(e.target.value)} required />

        {error && <p className="error">{String(error)}</p>}

        <button type="submit">Entrar</button>
      </form>
      <p>¿No tienes cuenta? <Link to="/registro">Regístrate aquí</Link></p>
    </div>
  );
}
