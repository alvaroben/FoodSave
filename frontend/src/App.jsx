import { Routes, Route } from "react-router-dom";
import NavBar from "./components/NavBar";
import RutaProtegida from "./components/RutaProtegida";
import Login from "./pages/Login";
import Registro from "./pages/Registro";
import Ofertas from "./pages/Ofertas";
import OfertaDetalle from "./pages/OfertaDetalle";
import MisReservas from "./pages/MisReservas";
import PanelEstablecimiento from "./pages/PanelEstablecimiento";
import "./App.css";

export default function App() {
  return (
    <>
      <NavBar />
      <Routes>
        <Route path="/" element={<Ofertas />} />
        <Route path="/ofertas/:id" element={<OfertaDetalle />} />
        <Route path="/login" element={<Login />} />
        <Route path="/registro" element={<Registro />} />
        <Route
          path="/mis-reservas"
          element={
            <RutaProtegida rolRequerido="Consumidor">
              <MisReservas />
            </RutaProtegida>
          }
        />
        <Route
          path="/panel"
          element={
            <RutaProtegida rolRequerido="Establecimiento">
              <PanelEstablecimiento />
            </RutaProtegida>
          }
        />
      </Routes>
    </>
  );
}
