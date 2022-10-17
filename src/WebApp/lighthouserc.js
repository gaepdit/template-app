module.exports = {
    ci: {
      collect: {
        url: ['https://localhost:7229/'],
        startServerCommand: 'dotnet run --no-build',
        numberOfRuns:5,
        settings: {
            "chromeFlags": "--ignore-certificate-errors --no-sandbox"
        },
      },
      assert: {
        preset: 'lighthouse:no-pwa',
      },
      upload: {
        target: 'temporary-public-storage',
      },
    },
  };