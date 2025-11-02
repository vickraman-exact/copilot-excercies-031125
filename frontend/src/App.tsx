import { createContext, useState } from 'react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { BrowserRouter as Router } from 'react-router-dom';
import AppRoutes from './routes/AppRoutes';
import { SessionProvider, User } from './context/SessionContext';
import './styles/globals.css';

// Define auth context types
interface AuthContextType {
  isAuthenticated: boolean;
  user: User | null;
  login: (user: User) => void;
  logout: () => void;
}

// Create a client
const queryClient = new QueryClient();

export const AuthContext = createContext<AuthContextType>({
  isAuthenticated: false,
  user: null,
  login: () => {},
  logout: () => {},
});

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [user, setUser] = useState<User | null>(null);


  const login = (userData: User) => {
    setUser(userData);
    setIsAuthenticated(true);

  };

  const logout = () => {
    setUser(null);
    setIsAuthenticated(false);
  };

  return (
    <QueryClientProvider client={queryClient}>
      <SessionProvider>
        <AuthContext.Provider
          value={{
            isAuthenticated,
            user,
            login,
            logout,
          }}
        >
          <Router>
            <AppRoutes />
          </Router>
        </AuthContext.Provider>
      </SessionProvider>
    </QueryClientProvider>
  )
}

export default App
