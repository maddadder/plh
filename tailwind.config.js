/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./**/*.{razor,html,cshtml}"],
  purge: {
      enabled: true,
      content: [
          './**/*.html',
          './**/*.razor'
      ],
  },
  theme: {
    extend: {},
  },
  plugins: [],
}
