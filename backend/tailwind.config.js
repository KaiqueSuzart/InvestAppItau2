/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        itau: {
          blue: "#004080",
          blue2: "#0066CC",
          orange: "#FF6600",
          gray: "#F5F5F5",
          black: "#333333"
        }
      },
      fontFamily: {
        sans: ["Inter", "Roboto", "sans-serif"]
      }
    },
  },
  plugins: [],
} 