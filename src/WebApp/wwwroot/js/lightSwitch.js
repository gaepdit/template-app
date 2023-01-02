(function () {
  let lightSwitch = document.getElementById('lightSwitch');
  if (!lightSwitch) {
    return;
  }

  /**
   * @function darkMode
   * @summary: changes the theme to 'dark mode' and save settings to local storage.
   * Basically, replaces/toggles every CSS class that has '-light' class with '-dark'
   */
  function darkMode() {
    // set background color
    document.querySelectorAll('.bg-light').forEach((element) => {
      element.className = element.className.replace(/-light/g, '-dark');
    });
    document.body.classList.add('bg-dark');
    
    // flip the color of text in the body
    document.body.classList.remove('text-dark');
    document.body.classList.add('text-light');

    // dropdown-item (did not change text color in v5.3.0-alpha1)
    for (const element of document.getElementsByClassName("dropdown-item")) {
      element.classList.remove('text-dark');
      element.classList.add('text-light');
    }

    // Tables
    const tables = document.querySelectorAll('table');
    for (let i = 0; i < tables.length; i++) {
      // add table-dark class to each table
      tables[i].classList.add('table-dark');
    }

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
    // set background color
    document.querySelectorAll('.bg-dark').forEach((element) => {
      element.className = element.className.replace(/-dark/g, '-light');
    });
    document.body.classList.add('bg-light');
    
    // flip the color of text in the body
    document.body.classList.remove('text-light');
    document.body.classList.add('text-dark');
    
    // dropdown-item (did not change text color in v5.3.0-alpha1)
    for (const element of document.getElementsByClassName("dropdown-item")) {
      element.classList.remove('text-light');
      element.classList.add('text-dark');
    }

    // Tables
    var tables = document.querySelectorAll('table');
    for (var i = 0; i < tables.length; i++) {
      if (tables[i].classList.contains('table-dark')) {
        tables[i].classList.remove('table-dark');
      }
    }

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
      document.documentElement.setAttribute('data-bs-theme', 'dark')
    } else {
      lightMode();
      document.documentElement.setAttribute('data-bs-theme', 'light')
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
