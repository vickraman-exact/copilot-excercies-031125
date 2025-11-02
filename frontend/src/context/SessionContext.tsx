import { useState, useEffect, createContext, useContext, ReactNode } from 'react';
import authService from '../api/authService';

export interface User {
  id: string;
  name: string;
  email: string;
  role: string;
}

interface SessionContextType {
  currentUser: User | null;
  isLoggedIn: boolean;
  login: (user: User, token: string) => void;
  logout: () => void;
}

const SessionContext = createContext<SessionContextType>({
  currentUser: null,
  isLoggedIn: false,
  login: () => {},
  logout: () => {},
});

export const useSession = () => useContext(SessionContext);

interface SessionProviderProps {
  children: ReactNode;
}

export const SessionProvider = ({ children }: SessionProviderProps) => {
  const [currentUser, setCurrentUser] = useState<User | null>(null);
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  // Load user from localStorage on component mount
  useEffect(() => {
    const savedUser = localStorage.getItem('user');
    if (savedUser) {
      try {
        const parsedUser = JSON.parse(savedUser);
        setCurrentUser(parsedUser);
        setIsLoggedIn(true);
      } catch (error) {
        console.error('Failed to parse saved user:', error);
      }
    }
  }, []);

  const login = (user: User, token: string) => {
    setCurrentUser(user);
    setIsLoggedIn(true);
    localStorage.setItem('user', JSON.stringify(user));
    localStorage.setItem('token', token);

    // Store session data for quick access
    sessionStorage.setItem('userEmail', user.email);
    sessionStorage.setItem('userName', user.name);

    // Create a session cookie (lasts until browser close)
    document.cookie = `user=${user.name}; path=/`;
  };

  const logout = () => {
    localStorage.removeItem('user');

    setCurrentUser(null);
    setIsLoggedIn(false);
  };

  return (
    <SessionContext.Provider value={{ currentUser, isLoggedIn, login, logout }}>
      {children}
    </SessionContext.Provider>
  );
};
