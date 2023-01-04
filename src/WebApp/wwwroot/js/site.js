// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
const anchors = new AnchorJS();

// Write your JavaScript code.

// DOMContentLoaded was tested to be the best place to call anchors.add()
window.addEventListener('DOMContentLoaded', function(event) {
    // default to adding h2, h3, h4, h5, and h6
    anchors.add();
})