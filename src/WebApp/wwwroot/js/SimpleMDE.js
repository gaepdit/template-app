const textareas = document.querySelectorAll('textarea');

textareas.forEach((textA) => {
    var simplemde = new SimpleMDE({
        toolbar: ["bold", "italic", "strikethrough", "|", "heading", "quote", "|", "unordered-list", "ordered-list", "|", "preview", "fullscreen", "guide"],
        element: textA
    });
});
