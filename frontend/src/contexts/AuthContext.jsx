import React, { createContext, useContext, useState } from 'react';

const AuthContext = createContext();

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth deve ser usado dentro de um AuthProvider');
  }
  return context;
};

export const AuthProvider = ({ children }) => {
  const [user] = useState({
    id: 1,
    nome: 'Usuário Demo',
    email: 'demo@acervo.com',
    tipoUsuario: 'Admin'
  });
  
  const [loading] = useState(false);
  const [isAuthenticated] = useState(true); // Sempre autenticado para demo

  const login = async (email, password) => {
    // Simulação de login para demo
    return { success: true };
  };

  const logout = () => {
    // Simulação de logout para demo
    console.log('Logout simulado');
  };

  const value = {
    user,
    loading,
    isAuthenticated,
    login,
    logout
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};

