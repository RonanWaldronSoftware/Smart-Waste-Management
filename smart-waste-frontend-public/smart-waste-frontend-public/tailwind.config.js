/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    colors: {
      transparent: "transparent",
      "white": "#ffffff",
      "black": "#000",
      themeColors: {
        bgColor: "#333",
        textColor: "#16181b",
        textColor2: "#212529",
        textColorLight: "#666",
        bgLightColor: "#f2f2f2",
        borderColor: "#BCBCBC",
        accentColor: "#6295AC;",
        accentColorDark: "#436c7f;",
        successColor: "#2ABC2A",
        dangerColor: "#B70202",
        warningColor: "#D8C11D",
        
      }
    },
    extend: {
      boxShadow: {
        customShadow: '0 .5rem 1rem rgba(0,0,0,.15)',
        btnShadow: '0 0 4px rgba(0,0,0,.12)'
      }
    },
  },
  plugins: [],
}

