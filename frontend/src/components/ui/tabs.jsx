import React, { createContext, useContext } from 'react';

const TabsContext = createContext();

export const Tabs = ({ value, onValueChange, children, className = '' }) => {
  return (
    <TabsContext.Provider value={{ value, onValueChange }}>
      <div className={className}>
        {children}
      </div>
    </TabsContext.Provider>
  );
};

export const TabsList = ({ children, className = '' }) => {
  return (
    <div className={`inline-flex h-10 items-center justify-center rounded-md bg-gray-100 p-1 text-gray-500 ${className}`}>
      {children}
    </div>
  );
};

export const TabsTrigger = ({ value, children, className = '' }) => {
  const { value: activeValue, onValueChange } = useContext(TabsContext);
  const isActive = activeValue === value;
  
  return (
    <button
      className={`inline-flex items-center justify-center whitespace-nowrap rounded-sm px-3 py-1.5 text-sm font-medium ring-offset-background transition-all focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50 ${
        isActive 
          ? 'bg-white text-gray-900 shadow-sm' 
          : 'text-gray-600 hover:text-gray-900'
      } ${className}`}
      onClick={() => onValueChange(value)}
    >
      {children}
    </button>
  );
};

export const TabsContent = ({ value, children, className = '' }) => {
  const { value: activeValue } = useContext(TabsContext);
  
  if (activeValue !== value) return null;
  
  return (
    <div className={`mt-2 ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 ${className}`}>
      {children}
    </div>
  );
};

