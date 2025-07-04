import React from 'react';

export const Switch = ({ 
  checked = false, 
  onCheckedChange, 
  disabled = false,
  className = '',
  ...props 
}) => {
  return (
    <button
      type="button"
      role="switch"
      aria-checked={checked}
      className={`peer inline-flex h-6 w-11 shrink-0 cursor-pointer items-center rounded-full border-2 border-transparent transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 focus-visible:ring-offset-background disabled:cursor-not-allowed disabled:opacity-50 ${
        checked ? 'bg-blue-600' : 'bg-gray-200'
      } ${className}`}
      onClick={() => !disabled && onCheckedChange(!checked)}
      disabled={disabled}
      {...props}
    >
      <span
        className={`pointer-events-none block h-5 w-5 rounded-full bg-white shadow-lg ring-0 transition-transform ${
          checked ? 'translate-x-5' : 'translate-x-0'
        }`}
      />
    </button>
  );
};

