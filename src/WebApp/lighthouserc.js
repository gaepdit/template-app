module.exports = {
    ci: {
      collect: {
        url: ['https://localhost:7229/'],
        startServerCommand: 'dotnet run --no-build',
        settings: {
            "chromeFlags": "--ignore-certificate-errors --no-sandbox"
        },
      },
      upload: {
        target: 'temporary-public-storage',
      },
    },
  };