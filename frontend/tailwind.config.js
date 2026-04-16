export default {
  content: ['./index.html', './src/**/*.{js,jsx}'],
  theme: {
    extend: {
      colors: {
        ink: '#0f172a',
        paper: '#f8fafc',
        brand: {
          50: '#ecfeff',
          100: '#cffafe',
          500: '#06b6d4',
          700: '#0e7490',
          900: '#164e63'
        },
        accent: '#d97706'
      },
      boxShadow: {
        glow: '0 0 0 1px rgba(6,182,212,.15), 0 20px 50px rgba(15,23,42,.12)'
      },
      keyframes: {
        rise: {
          '0%': { opacity: 0, transform: 'translateY(18px)' },
          '100%': { opacity: 1, transform: 'translateY(0)' }
        }
      },
      animation: {
        rise: 'rise .55s ease-out both'
      }
    }
  },
  plugins: []
}
