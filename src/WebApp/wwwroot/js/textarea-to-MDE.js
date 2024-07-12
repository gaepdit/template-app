const textarealist = document.querySelectorAll("textarea.EasyMDE");
textarealist.forEach((area) => {
  const easyMDEE = new EasyMDE({
    element: area,
  });
});
