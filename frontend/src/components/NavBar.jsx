import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export default function NavBar() {
  const { usuario, cerrarSesion } = useAuth();
  const navigate = useNavigate();

  const salir = () => {
    cerrarSesion();
    navigate("/login");
  };

  return (
    <nav className="navbar">
      <Link to="/" className="brand">FoodSave</Link>
      <div className="nav-links">
        <Link to="/">Ofertas</Link>
        {usuario?.rol === "Consumidor" && <Link to="/mis-reservas">Mis reservas</Link>}
        {usuario?.rol === "Establecimiento" && <Link to="/panel">Mi panel</Link>}
        {!usuario && <Link to="/login">Iniciar sesión</Link>}
        {!usuario && <Link to="/registro">Registrarse</Link>}
        {usuario && (
          <>
            <span className="usuario-actual">{usuario.nombreCompleto} ({usuario.rol})</span>
            <button onClick={salir}>Salir</button>
          </>
        )}
      </div>
    </nav>
  );
}
