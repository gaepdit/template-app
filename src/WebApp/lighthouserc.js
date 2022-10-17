module.exports = {
    ci: {
      collect: {
        url: ['https://localhost:7229/'],
        startServerCommand: 'dotnet run --no-build',
      },
      upload: {
        target: 'temporary-public-storage',
      },
    },
  };