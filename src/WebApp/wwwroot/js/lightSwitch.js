(function () {
  let lightSwitch = document.getElementById('lightSwitch');
  if (!lightSwitch) {
    return;
  }

  /**
   * @function darkMode
   * @summary: changes the theme to 'dark mode' and save settings to local storage.
   */
  function darkMode() {
    document.documentElement.setAttribute('data-bs-theme', 'dark')
    // set light switch input to dark
    if (lightSwitch.checked) {
      lightSwitch.checked = false;
    }
    localStorage.setItem('theme', 'dark');
  }

  /**
   * @function lightMode
   * @summary: changes the theme to 'light mode' and save settings to local storage.
   */
  function lightMode() {
    document.documentElement.setAttribute('data-bs-theme', 'light')
    // set light switch input to light
    if (!lightSwitch.checked) {
      lightSwitch.checked = true;
    }
    localStorage.setItem('theme', 'light');
  }

  /**
   * @function setTheme
   * @summary: calling @darkMode or @lightMode depending on the checked state.
   */
  function setTheme() {
    if (!lightSwitch.checked) {
      darkMode();
    } else {
      lightMode();
    }
  }

  /**
   * @function getTheme
   * @summary: get system default theme by media query
   */
  function getTheme() {
    const storedTheme = localStorage.getItem('theme')
    if (storedTheme) {
      return storedTheme
    }
    return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'
  }

  /**
   * @function showActiveTheme
   * @param theme light or dark mode
   */
  function showActiveTheme(theme) {
    const activeThemeIcon = document.querySelector('.theme-icon-active use')
    const btnToActive = document.querySelector(`[data-bs-theme-value="${theme}"]`)
    const svgOfActiveBtn = btnToActive.querySelector('svg use').getAttribute('href')

    document.querySelectorAll('[data-bs-theme-value]').forEach(element => {
      element.classList.remove('active')
    })

    btnToActive.classList.add('active')
    activeThemeIcon.setAttribute('href', svgOfActiveBtn)
  }

  /**
   * @summary A function called in the beginning. Add event listener and determine which mode to use.
   */
  function setup() {
    let settings = localStorage.getItem('theme');
    if (settings == null) {
      settings = getTheme();
    }
    if (settings === 'light') {
      lightSwitch.checked = true;
    }

    lightSwitch.addEventListener('change', setTheme);
    
    // Change the default theme on launch of page
    window.addEventListener('DOMContentLoaded', () => {
      setTheme()

      document.querySelectorAll('[data-bs-theme-value]')
        .forEach(toggle => {
          toggle.addEventListener('click', () => {
            const theme = toggle.getAttribute('data-bs-theme-value')
            localStorage.setItem('theme', theme)
            document.documentElement.setAttribute('data-bs-theme', theme)
            showActiveTheme(theme)
          })
        })
    })
    setTheme();
  }

  setup();
})();
