import { createContext, useContext, useState } from "react";

const AuthContext = createContext(null);

function leerUsuarioGuardado() {
  const raw = localStorage.getItem("foodsave_user");
  return raw ? JSON.parse(raw) : null;
}

export function AuthProvider({ children }) {
  const [usuario, setUsuario] = useState(leerUsuarioGuardado());

  const iniciarSesion = (authResponse) => {
    const { token, ...datosUsuario } = authResponse;
    localStorage.setItem("foodsave_token", token);
    localStorage.setItem("foodsave_user", JSON.stringify(datosUsuario));
    setUsuario(datosUsuario);
  };

  const cerrarSesion = () => {
    localStorage.removeItem("foodsave_token");
    localStorage.removeItem("foodsave_user");
    setUsuario(null);
  };

  return (
    <AuthContext.Provider value={{ usuario, iniciarSesion, cerrarSesion }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth debe usarse dentro de AuthProvider");
  return ctx;
}
