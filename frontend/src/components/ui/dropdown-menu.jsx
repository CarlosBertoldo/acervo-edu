import React, { useState, createContext, useContext } from 'react';

const DropdownContext = createContext();

export const DropdownMenu = ({ children }) => {
  const [isOpen, setIsOpen] = useState(false);
  
  return (
    <DropdownContext.Provider value={{ isOpen, setIsOpen }}>
      <div className="relative inline-block text-left">
        {children}
      </div>
    </DropdownContext.Provider>
  );
};

export const DropdownMenuTrigger = ({ children, asChild = false }) => {
  const { isOpen, setIsOpen } = useContext(DropdownContext);
  
  if (asChild) {
    return React.cloneElement(children, {
      onClick: () => setIsOpen(!isOpen)
    });
  }
  
  return (
    <button onClick={() => setIsOpen(!isOpen)}>
      {children}
    </button>
  );
};

export const DropdownMenuContent = ({ children, align = 'start', className = '' }) => {
  const { isOpen, setIsOpen } = useContext(DropdownContext);
  
  if (!isOpen) return null;
  
  const alignClasses = {
    start: 'left-0',
    end: 'right-0',
    center: 'left-1/2 transform -translate-x-1/2'
  };
  
  return (
    <>
      <div 
        className="fixed inset-0 z-40" 
        onClick={() => setIsOpen(false)}
      />
      <div className={`absolute z-50 mt-1 w-56 rounded-md border border-gray-200 bg-white shadow-lg ${alignClasses[align]} ${className}`}>
        <div className="py-1">
          {children}
        </div>
      </div>
    </>
  );
};

export const DropdownMenuItem = ({ children, onClick, className = '' }) => {
  const { setIsOpen } = useContext(DropdownContext);
  
  return (
    <button
      className={`flex w-full items-center px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 focus:bg-gray-100 focus:outline-none ${className}`}
      onClick={(e) => {
        onClick?.(e);
        setIsOpen(false);
      }}
    >
      {children}
    </button>
  );
};

export const DropdownMenuSeparator = ({ className = '' }) => {
  return <div className={`my-1 h-px bg-gray-200 ${className}`} />;
};

