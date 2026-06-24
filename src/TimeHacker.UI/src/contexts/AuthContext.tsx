import { createContext, useContext, useState, useEffect } from 'react';
import type { ReactNode } from 'react';
import { api, loadCsrfToken } from '../api/api';

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
            const response = await api.get<User>('/api/users/me');
            setUser(response.data);
            // Authenticated: prime the antiforgery token for subsequent mutations.
            await loadCsrfToken().catch(() => undefined);
        } catch {
            // A 401 here is the normal logged-out state, so don't treat it as an error.
            setUser(null);
        }
    };

    useEffect(() => {
        fetchCurrentUser().finally(() => setLoading(false));
    }, []);

    const login = (userData: User) => {
        setUser(userData);
        // Just logged in: prime the antiforgery token for subsequent mutations.
        void loadCsrfToken().catch(() => undefined);
    };

    const logout = () => {
        setUser(null);
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
