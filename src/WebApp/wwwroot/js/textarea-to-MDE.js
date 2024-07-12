const textarealist = document.querySelectorAll("textarea.EasyMDE");
textarealist.forEach((area) => {
  const easyMDEE = new EasyMDE({
    element: area,
    placeholder: "Type here...",
    toolbar: [
      "bold",
      "italic",
      "strikethrough",
      "|",
      "heading-1",
      "heading-2",
      "heading-3",
      "|",
      "quote",
      "unordered-list",
      "ordered-list",
      "|",
      "preview",
      "fullscreen",
      "guide",
    ],
  });
});
