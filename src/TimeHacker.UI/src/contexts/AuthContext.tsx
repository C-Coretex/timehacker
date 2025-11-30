import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import api from '../api/api';

interface User {
    name: string;
    phoneNumberForNotifications: string;
    emailForNotifications: string;
}

interface AuthContextType {
    user: User | null;
    isAuthenticated: boolean;
    login: (userData: User) => void;
    logout: () => void;
    fetchCurrentUser: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
    const [user, setUser] = useState<User | null>(null);

    const fetchCurrentUser = async () => {
        try {
            const response = await api.get<User>('/api/User/GetCurrent');
            setUser(response.data);
        } catch (error) {
            console.error('Failed to fetch current user:', error);
            setUser(null);
        }
    };

    // restore auth state from localStorage on app load
    useEffect(() => {
        fetchCurrentUser();
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
