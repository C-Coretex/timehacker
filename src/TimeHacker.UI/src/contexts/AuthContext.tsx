import { createContext, useContext, useState, useEffect } from 'react';
import type { ReactNode } from 'react';
import api from '../api/api';

interface User {
    name: string;
    phoneNumberForNotifications: string;
    emailForNotifications: string;
}

interface AuthContextType {
    user: User | null;
    isAuthenticated: boolean;
    loading: boolean;
    login: (userData: User) => void;
    logout: () => void;
    fetchCurrentUser: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
    const [user, setUser] = useState<User | null>(null);
    const [loading, setLoading] = useState(true);

    const fetchCurrentUser = async () => {
        try {
            const response = await api.get<User>('/api/User/GetCurrent');
            setUser(response.data);
        } catch (error) {
            console.error('Failed to fetch current user:', error);
            setUser(null);
        }
    };

    useEffect(() => {
        fetchCurrentUser().finally(() => setLoading(false));
    }, []);

    const login = (userData: User) => {
        setUser(userData);
        localStorage.setItem('user', JSON.stringify(userData));
    };

    const logout = () => {
        setUser(null);
        localStorage.removeItem('user');
    };

    return (
        <AuthContext.Provider
            value={{
                user,
                isAuthenticated: !!user,
                loading,
                login,
                logout,
                fetchCurrentUser,
            }}
        >
            {children}
        </AuthContext.Provider>
    );
};

// Hook for using auth context
export const useAuth = (): AuthContextType => {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error('useAuth must be used within AuthProvider');
    }
    return context;
};
