/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./app/**/*.{js,jsx,ts,tsx}",
    "./components/**/*.{js,jsx,ts,tsx}",
    "./lib/**/*.{js,jsx,ts,tsx}",
  ],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        primary: '#16a3b2',
        'background-dark': '#0d1117',
        'background-light': '#e5e7eb',
        'card-light': '#ffffff',
        'card-dark': 'rgb(31, 41, 55)',
        'card-darker': '#161b22',
      },
      fontFamily: {
        display: ['Poppins', 'sans-serif'],
        auth: ['Poppins', 'sans-serif'],
        sans: ['Inter', 'system-ui', 'sans-serif'],
      },
      animation: {
        'fade-in-down': 'fade-in-down 0.3s ease-out',
        'heart-beat': 'heartBeat 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275)',
      },
      keyframes: {
        'fade-in-down': {
          '0%': { opacity: '0', transform: 'translateY(-10px)' },
          '100%': { opacity: '1', transform: 'translateY(0)' },
        },
        heartBeat: {
          '0%': { transform: 'scale(1) translateY(1px)' },
          '40%': { transform: 'scale(1.25) translateY(1px)' },
          '100%': { transform: 'scale(1) translateY(1px)' },
        },
      },
    },
  },
  plugins: [],
};
