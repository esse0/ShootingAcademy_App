import React, { createContext, useContext } from 'react';
import { useWebSocket } from './useWebSocket';

type WebSocketType = ReturnType<typeof useWebSocket>;
const WebSocketContext = createContext<WebSocketType | null>(null);

export const WebSocketProvider = ({ children }: { children: React.ReactNode }) => {
    const ws = useWebSocket();
    return (
        <WebSocketContext.Provider value={ws}>
            {children}
        </WebSocketContext.Provider>
    );
};

export const useWebSocketContext = () => useContext(WebSocketContext);