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
        assertions: {
            "categories:performance": ["error", {"minScore": 0.9}],
            "categories:accessibility": ["error", {"minScore": 0.9}],
            "categories:best-practices": ["error", {"minScore": 0.9}],
            "categories:seo": ["error", {"minScore": 0.8}],
        },
      },
      upload: {
        target: 'temporary-public-storage',
      },
    },
  };